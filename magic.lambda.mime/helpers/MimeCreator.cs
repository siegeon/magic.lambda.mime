/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using MimeKit;
using MimeKit.IO;
using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg.OpenPgp;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.mime.helpers
{
    /// <summary>
    /// Helper class to create MIME messages.
    /// </summary>
    public static class MimeCreator
    {
        /// <summary>
        /// Creates a MimeEntity given the structured input node, and returns MimeEntity to caller.
        /// </summary>
        /// <param name="signaler">Signaler used to construct message.</param>
        /// <param name="input">Hierarchical node structure representing the MIME message in lambda format.</param>
        /// <returns></returns>
        public static MimeEntity Create(ISignaler signaler, Node input)
        {
            var messageNodes = input.Children.Where(x => x.Name == "entity");
            if (messageNodes.Count() != 1)
                throw new ArgumentException("Too many [entity] nodes found for slot to handle.");
            return CreateEntity(signaler, messageNodes.First());
        }

        /// <summary>
        /// Helper method to dispose a MimeEntity's streams.
        /// </summary>
        /// <param name="entity">Entity to iterate over to dispose all associated streams.</param>
        public static void Dispose(MimeEntity entity)
        {
            if (entity is MimePart part)
            {
                part.Content?.Stream?.Dispose();
            }
            else if (entity is Multipart multi)
            {
                foreach (var idx in multi)
                {
                    Dispose(idx);
                }
            }
        }

        #region [ -- Private helper methods -- ]

        /*
         * Create MimeEntity, or MIME part to be specific.
         */
        static MimeEntity CreateEntity(ISignaler signaler, Node input)
        {
            MimeEntity result = null;

            // Finding Content-Type of entity.
            var type = input.GetEx<string>();
            if (!type.Contains("/"))
                throw new ArgumentException($"'{type}' is an unknown MIME Content-Type. Please provide valid Content-Type as value of node.");
            var tokens = type.Split('/');
            if (tokens.Length != 2)
                throw new ArgumentException($"'{type}' is an unknown MIME Content-Type. Please provide valid Content-Type as value of node.");
            var mainType = tokens[0];
            var subType = tokens[1];
            switch (mainType)
            {
                case "text":
                    result = CreateLeafPart(signaler, mainType, subType, input);
                    break;
                case "multipart":
                    result = CreateMultipart(signaler, subType, input);
                    break;
            }
            var encryptionKey = input.Children.FirstOrDefault(x => x.Name == "encrypt")?.GetEx<string>();
            var signingKey = input.Children.FirstOrDefault(x => x.Name == "sign")?.GetEx<string>();
            var signingKeyPassword = input.Children.FirstOrDefault(x => x.Name == "sign")?.Children.FirstOrDefault(x => x.Name == "password")?.GetEx<string>();
            if (!string.IsNullOrEmpty(encryptionKey) && !string.IsNullOrEmpty(signingKey))
                result = SignAndEncrypt(result, encryptionKey, signingKey, signingKeyPassword);
            else if (!string.IsNullOrEmpty(encryptionKey))
                result = Encrypt(result, encryptionKey);
            else if (!string.IsNullOrEmpty(signingKey))
                result = Sign(result, signingKey, signingKeyPassword); // Signing entity.
            return result;
        }

        /*
         * Creates a leaf part, implying no MimePart children.
         */
        static MimePart CreateLeafPart(
            ISignaler signaler,
            string mainType,
            string subType,
            Node messageNode)
        {
            // Retrieving [content] node.
            var contentNode = messageNode.Children.FirstOrDefault(x => x.Name == "content") ??
                messageNode.Children.FirstOrDefault(x => x.Name == "filename") ??
                throw new ArgumentNullException("No [content] provided in [message]");

            var result = new MimePart(ContentType.Parse(mainType + "/" + subType));
            DecorateEntityHeaders(result, messageNode);
            switch (contentNode.Name)
            {
                case "content":
                    CreateContentObjectFromObject(contentNode, result);
                    break;
                case "filename":
                    CreateContentObjectFromFilename(signaler, contentNode, result);
                    break;
            }
            return result;
        }

        static Multipart CreateMultipart(
            ISignaler signaler,
            string subType,
            Node messageNode)
        {
            // Retrieving [content] node.
            var contentNode = messageNode.Children.FirstOrDefault(x => x.Name == "content") ??
                throw new ArgumentNullException("No [content] provided in [message]");

            var result = new Multipart(subType);
            DecorateEntityHeaders(result, messageNode);
            foreach (var idxPart in contentNode.Children)
            {
                result.Add(CreateEntity(signaler, idxPart));
            }
            return result;
        }

        /*
         * Creates ContentObject from value found in node.
         */
        static void CreateContentObjectFromObject(
            Node contentNode,
            MimePart part)
        {
            var stream = new MemoryBlockStream();
            var content = contentNode.GetEx<string>() ??
                throw new ArgumentNullException("Noe actual [content] supplied to message");
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            ContentEncoding encoding = ContentEncoding.Default;
            var encodingNode = contentNode.Children.FirstOrDefault(x => x.Name == "Content-Encoding");
            if (encodingNode != null)
                encoding = (ContentEncoding)Enum.Parse(typeof(ContentEncoding), encodingNode.GetEx<string>());
            part.Content = new MimeContent(stream, encoding);
        }

        /*
         * Creates ContentObject from filename.
         */
        static void CreateContentObjectFromFilename(
            ISignaler signaler,
            Node contentNode,
            MimePart part)
        {
            var filename = contentNode.GetEx<string>() ?? throw new ArgumentNullException("No [filename] value provided");

            // Checking if explicit encoding was supplied.
            ContentEncoding encoding = ContentEncoding.Default;
            var encodingNode = contentNode.Children.FirstOrDefault(x => x.Name == "Content-Encoding");
            if (encodingNode != null)
                encoding = (ContentEncoding)Enum.Parse(typeof(ContentEncoding), encodingNode.GetEx<string>());

            // Checking if explicit disposition was specified.
            if (part.ContentDisposition == null)
            {
                // Defaulting Content-Disposition to; "attachment; filename=whatever.xyz"
                part.ContentDisposition = new ContentDisposition("attachment")
                {
                    FileName = Path.GetFileName(filename)
                };
            }
            var rootPath = new Node();
            signaler.Signal(".io.folder.root", rootPath);
            part.Content = new MimeContent(File.OpenRead(rootPath.GetEx<string>() + filename.TrimStart('/')), encoding);
        }

        /*
         * Decorates MimeEntity with headers specified in Node children collection.
         */
        static void DecorateEntityHeaders(MimeEntity entity, Node messageNode)
        {
            var headerNode = messageNode.Children.FirstOrDefault(x => x.Name == "headers");
            if (headerNode == null)
                return; // No headers
            foreach (var idx in headerNode.Children.Where(ix => ix.Name != "Content-Type" && ix.Name != "content"))
            {
                entity.Headers.Replace(idx.Name, idx.GetEx<string>());
            }
        }

        static MultipartEncrypted SignAndEncrypt(
            MimeEntity entity,
            string encryptionKey,
            string signingKey,
            string password)
        {
            var algo = DigestAlgorithm.Sha256;
            using (var ctx = new PgpContext { Password = password })
            {
                return MultipartEncrypted.SignAndEncrypt(
                    ctx,
                    PgpHelpers.GetSecretKeyFromAsciiArmored(signingKey),
                    algo,
                    new PgpPublicKey[] { PgpHelpers.GetPublicKeyFromAsciiArmored(encryptionKey) },
                    entity);
            }
        }

        static MultipartSigned Sign(
            MimeEntity entity,
            string key,
            string password)
        {
            var algo = DigestAlgorithm.Sha256;
            using (var ctx = new PgpContext { Password = password })
            {
                return MultipartSigned.Create(
                    ctx,
                    PgpHelpers.GetSecretKeyFromAsciiArmored(key),
                    algo,
                    entity);
            }
        }

        static MultipartEncrypted Encrypt(MimeEntity entity, string key)
        {
            using (var ctx = new PgpContext())
            {
                return MultipartEncrypted.Encrypt(
                    ctx,
                    new PgpPublicKey[] { PgpHelpers.GetPublicKeyFromAsciiArmored(key) },
                    entity);
            }
        }

        #endregion
    }
}
