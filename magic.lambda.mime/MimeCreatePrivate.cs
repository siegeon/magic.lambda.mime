/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Collections.Generic;
using MimeKit;
using magic.node;
using magic.signals.contracts;
using magic.lambda.mime.helpers;

namespace magic.lambda.mime
{
    /// <summary>
    /// Creates a MIME message and returns it as a MimeKit MimeMessage to caller (hidden)
    /// </summary>
    [Slot(Name = ".mime.create")]
    public class MimeCreatePrivate : ISlot
    {
        /// <summary>
        /// Implementation of your slot.
        /// </summary>
        /// <param name="signaler">Signaler that raised the signal.</param>
        /// <param name="input">Arguments to your slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            var streams = new List<Stream>();
            try
            {
                input.Value = new Tuple<MimeEntity, List<Stream>>(
                    MimeBuilder.CreateMimeMessage(input, streams),
                    streams);
                input.Clear();
            }
            catch
            {
                foreach (var idx in streams)
                {
                    idx.Dispose();
                }
                throw;
            }
        }
    }
}
