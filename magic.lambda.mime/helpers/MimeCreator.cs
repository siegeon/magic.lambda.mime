/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using MimeKit;
using MimeKit.IO;
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
        /// Creates a MimeEntity from the specified lambda object and returns the
        /// result as a MimeEntity to caller.
        /// </summary>
        /// <param name="signaler">Signaler used to construct message.</param>
        /// <param name="input">Hierarchical node structure representing the MIME message in lambda format.</param>
        /// <returns>A MIME entity object encapsulating the specified lambda object</returns>
        public static MimeEntity Create(ISignaler signaler, Node input)
        {
            // Finding Content-Type of entity.
            var type = input.GetEx<string>();
            if (!type.Contains("/"))
                throw new HyperlambdaException($"'{type}' is an unknown MIME Content-Type. Please provide a valid MIME type as the value of your node.");

            var tokens = type.Split('/');
            if (tokens.Length != 2)
                throw new HyperlambdaException($"'{type}' is an unknown MIME Content-Type. Please provide a valid MIME type as the value of your node.");

            var mainType = tokens[0];
            var subType = tokens[1];
            switch (mainType)
            {
                case "application":
                case "text":
                    return CreateLeafPart(signaler, mainType, subType, input);

                case "multipart":
                    return CreateMultipart(signaler, subType, input);

                default:
                    throw new HyperlambdaException($"I don't know how to handle the '{type}' MIME type.");
            }
        }

        /*
         * Internal helper method to dispose all streams inside all entities.
         */
        internal static void DisposeEntity(MimeEntity entity)
        {
            if (entity is MimePart part)
            {
                part.Content?.Stream?.Dispose();
            }
            else if (entity is Multipart multi)
            {
                foreach (var idx in multi)
                {
                    DisposeEntity(idx);
                }
            }
        }

        #region [ -- Private helper methods -- ]

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
            var contentNode = messageNode.Children.FirstOrDefault(x => x.Name == "content" || x.Name == "filename") ??
                throw new HyperlambdaException("No [content] or [filename] provided for your entity");

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

        /*
         * Creates a multipart of some sort.
         */
        static Multipart CreateMultipart(
            ISignaler signaler,
            string subType,
            Node messageNode)
        {
            var result = new Multipart(subType);
            DecorateEntityHeaders(result, messageNode);

            foreach (var idxPart in messageNode.Children.Where(x => x.Name == "entity"))
            {
                result.Add(Create(signaler, idxPart));
            }
            return result;
        }

        /*
         * Creates ContentObject from value found in node.
         */
        static void CreateContentObjectFromObject(Node contentNode, MimePart part)
        {
            var stream = new MemoryBlockStream();
            var content = contentNode.GetEx<string>() ??
                throw new HyperlambdaException("No actual [content] supplied to message");
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            var encoding = ContentEncoding.Default;
            var encodingNode = contentNode.Children.FirstOrDefault(x => x.Name == "Content-Encoding");
            if (encodingNode != null)
                encoding = (ContentEncoding)Enum.Parse(typeof(ContentEncoding), encodingNode.GetEx<string>(), true);
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
            var filename = contentNode.GetEx<string>() ?? throw new HyperlambdaException("No [filename] value provided");

            // Checking if explicit encoding was supplied.
            ContentEncoding encoding = ContentEncoding.Default;
            var encodingNode = contentNode.Children.FirstOrDefault(x => x.Name == "Content-Encoding");
            if (encodingNode != null)
                encoding = (ContentEncoding)Enum.Parse(typeof(ContentEncoding), encodingNode.GetEx<string>(), true);

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
            part.Content = new MimeContent(
                File.OpenRead(
                    rootPath.GetEx<string>() +
                    filename.TrimStart('/')),
                encoding);
        }

        /*
         * Decorates MimeEntity with headers specified in Node children collection.
         */
        static void DecorateEntityHeaders(MimeEntity entity, Node messageNode)
        {
            var headerNode = messageNode.Children.FirstOrDefault(x => x.Name == "headers");
            if (headerNode == null)
                return; // No headers

            foreach (var idx in headerNode.Children.Where(ix => ix.Name != "Content-Type"))
            {
                entity.Headers.Replace(idx.Name, idx.GetEx<string>());
            }
        }

        #endregion
    }
}
