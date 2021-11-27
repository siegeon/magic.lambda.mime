﻿/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Linq;
using MimeKit;
using MimeKit.IO;
using magic.node;

namespace magic.lambda.mime.helpers
{
    /// <summary>
    /// Helper class to parse MIME messages.
    /// </summary>
    public static class MimeParser
    {
        /// <summary>
        /// Parses a MimeEntity and returns as lambda to caller.
        /// </summary>
        /// <param name="node">Node containing the MIME message as value,
        /// and also where the lambda structure representing the parsed message will be placed.</param>
        /// <param name="entity">MimeEntity to parse.</param>
        public static void Parse(Node node, MimeEntity entity)
        {
            node.Value = entity.ContentType.MimeType;
            ProcessHeaders(node, entity);

            if (entity is Multipart multi)
            {
                // Multipart content.
                foreach (var idx in multi)
                {
                    var idxNode = new Node("entity");
                    Parse(idxNode, idx);
                    node.Add(idxNode);
                }
            }
            else if (entity is TextPart text)
            {
                // Singular content type.
                // Notice! We don't really care about the encoding the text was encoded with.
                node.Add(new Node("content", text.GetText(out var _)));
            }
            else if (entity is MimePart part)
            {
                using (var stream = new MemoryBlockStream())
                {
                    // Decoding content to memory.
                    part.Content.DecodeTo(stream);

                    // Resetting position and setting up a buffer object to hold content.
                    stream.Position = 0;

                    // Putting content into return node for MimeEntity.
                    node.Add(new Node("content", stream.ToArray()));
                }
            }
        }

        /// <summary>
        /// Helper method to dispose a MimeEntity's streams.
        /// </summary>
        /// <param name="entity">Entity to iterate over to dispose all associated streams.</param>
        public static void DisposeEntity(MimeEntity entity)
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
