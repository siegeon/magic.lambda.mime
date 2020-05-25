/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using MimeKit;
using magic.node;
using magic.signals.contracts;
using magic.lambda.mime.helpers;

namespace magic.lambda.mime
{
    /// <summary>
    /// Parses a MimeEntity message and returns its as a hierarchical object of lambda to caller.
    /// </summary>
    [Slot(Name = ".mime.parse")]
    public class MimeParsePrivate : ISlot
    {
        /// <summary>
        /// Implementation of your slot.
        /// </summary>
        /// <param name="signaler">Signaler that raised the signal.</param>
        /// <param name="input">Arguments to your slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            var message = input.Value as MimeEntity;
            MimeBuilder.Parse(input, message);
            input.Value = null;
        }
    }
}
