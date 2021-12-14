/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.IO;
using System.Linq;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.mime.helpers;

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
            // Figuring out if caller wants a structured result.
            var structured = input.Children
                .FirstOrDefault(x => x.Name == "structured")?
                .GetEx<bool>() ?? false;

            // Creating entity.
            using (var entity = MimeCreator.Create(signaler, input))
            {
                // House cleaning.
                input.Value = null;
                input.Clear();

                // Serialising entity into temporary stream such that we can correctly return it to caller.
                using (var stream = new MemoryStream())
                {
                    entity.WriteTo(stream, structured);
                    stream.Position = 0;

                    // Reading back entity's content again.
                    using (var reader = new StreamReader(stream))
                    {
                        // Checking if caller wants a structured result or not.
                        if (structured)
                        {
                            input.AddRange(entity.Headers.Select(x => new Node(x.Field, x.Value)));
                            input.Add(new Node("content", reader.ReadToEnd()));
                        }
                        else
                        {
                            input.Value = reader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
