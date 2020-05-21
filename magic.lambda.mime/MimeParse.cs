/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.IO;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using MimeKit;

namespace magic.lambda.mime
{
    /// <summary>
    /// Parses a MIME message and returns its as a hierarchical object of lambda to caller.
    /// </summary>
    [Slot(Name = "mime.parse")]
    public class MimeParse : ISlot
    {
        /// <summary>
        /// Implementation of your slot.
        /// </summary>
        /// <param name="signaler">Signaler that raised the signal.</param>
        /// <param name="input">Arguments to your slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(input.GetEx<string>());
                    writer.Flush();
                    stream.Position = 0;
                    var message = MimeMessage.Load(stream);
                    Traverse(input, message.Body);
                }
            }
        }

        #region [ -- Private helper methods -- ]

        void Traverse(Node node, MimeEntity entity)
        {
            var tmp = new Node("message", entity.ContentType.MimeType);
            if (entity is Multipart multi)
            {
                // Multipart content.
                foreach (var idx in multi)
                {
                    Traverse(tmp, idx);
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

        #endregion
    }
}
