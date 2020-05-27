/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Security;
using MimeKit;
using MimeKit.IO;
using MimeKit.Cryptography;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.mime.helpers
{
    /*
     * Internal helper class to create a MimeEntity.
     */
    public static class MimeBuilder
    {
        /*
         * Creates a MimeEntity given the structured input node, and returns MimeEntity to caller.
         */
        public static MimeEntity Create(ISignaler signaler, Node input)
        {
            var messageNodes = input.Children.Where(x => x.Name == "entity");
            if (messageNodes.Count() != 1)
                throw new ArgumentException("Too many [entity] nodes found for slot to handle.");
            return CreateEntity(signaler, messageNodes.First());
        }

        /*
         * Parses a MimeEntity and returns as lambda to caller.
         */
        public static void Parse(Node node, MimeEntity entity)
        {
            var tmp = new Node("entity", entity.ContentType.MimeType);
            ProcessHeaders(tmp, entity);

            // TODO: Implement PGP Context (somehow) - Reading public keys from database.
            if (entity is MultipartSigned signed)
            {
                // Multipart content.
                var signatures = new Node("signatures");
                foreach (var idx in signed.Verify())
                {
                    if (!idx.Verify())
                        throw new SecurityException("Signature of MIME message was not valid");
                    signatures.Add(new Node("fingerprint", idx.SignerCertificate.Fingerprint.ToLower()));
                }
                tmp.Add(signatures);

                // Then traversing content of multipart/signed message.
                foreach (var idx in signed)
                {
                    Parse(tmp, idx);
                }
            }
            else if (entity is Multipart multi)
            {
                // Multipart content.
                foreach (var idx in multi)
                {
                    Parse(tmp, idx);
                }
            }
            else if (entity is TextPart text)
            {
                // Singular content type.
                // Notice! We don't really care about the encoding the text was encoded with.
                tmp.Add(new Node("content", text.GetText(out var encoding)));
            }
            node.Add(tmp);
        }

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
            signaler.Signal("io.folder.root", rootPath);
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

        /*
         * Process MIME entity's headers, and adds up into node collection.
         */
        static void ProcessHeaders(Node node, MimeEntity entity)
        {
            var headers = new Node("headers");
            foreach (var idx in entity.Headers)
            {
                if (idx.Id == HeaderId.ContentType)
                    continue; // Ignored, since it's part of main "entity" node.

                headers.Add(new Node(idx.Field, idx.Value));
            }

            // We only add headers node if there are any headers.
            if (headers.Children.Any())
                node.Add(headers);
        }

        #endregion
    }
}
