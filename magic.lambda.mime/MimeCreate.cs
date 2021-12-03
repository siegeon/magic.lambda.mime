/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Linq;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.mime.helpers;
using System.IO;

namespace magic.lambda.mime
{
    /// <summary>
    /// Creates a MIME message and returns it as a MIME message to caller.
    /// </summary>
    [Slot(Name = "mime.create")]
    public class MimeCreate : ISlot
    {
        /// <summary>
        /// Implementation of your slot.
        /// </summary>
        /// <param name="signaler">Signaler that raised the signal.</param>
        /// <param name="input">Arguments to your slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            // Figuring out if caller wants a structures result or not.
            var structuredNode = input.Children.FirstOrDefault(x => x.Name == "structured");
            var structured = structuredNode?.Get<bool>() ?? false;
            structuredNode?.UnTie();

            // Creating entity
            var entity = MimeCreator.Create(signaler, input);
            try
            {
                using (var stream = new MemoryStream())
                {
                    entity.WriteTo(stream, structured);
                    input.Clear();
                    stream.Position = 0;
                    using (var reader = new StreamReader(stream))
                    {
                        var result = reader.ReadToEnd();
                        if (structured)
                        {
                            var contentType = entity.ContentType.ToString();
                            contentType = contentType.Substring(contentType.IndexOf(':') + 1).TrimStart();
                            input.Add(new Node("Content-Type", contentType));
                            input.Add(new Node("content", result));
                            System.Console.WriteLine(input.ToHyperlambda());
                        }
                        else
                        {
                            input.Value = result;
                        }
                    }
                }
            }
            finally
            {
                MimeCreator.DisposeEntity(entity);
            }
        }
    }
}
