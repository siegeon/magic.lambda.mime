/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.IO;
using System.Text;
using System.Linq;
using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.mime.helpers;

namespace magic.lambda.mime
{
    /// <summary>
    /// Parses a MIME message and returns its as a hierarchical object of lambda to caller.
    /// </summary>
    [Slot(Name = "pgp.import")]
    public class PgpImport : ISlot
    {
        static PgpImport()
        {
            CryptographyContext.Register(typeof(PGPContext));
        }

        /// <summary>
        /// Implementation of your slot.
        /// </summary>
        /// <param name="signaler">Signaler that raised the signal.</param>
        /// <param name="input">Arguments to your slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            var keyPlainText = input.GetEx<string>();
            using (var ctx = new PGPContext(signaler)
            {
                ImportPrivateLambda = input.Children.First(x => x.Name == ".lambda"),
            })
            {
                using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(keyPlainText.Replace("\r\n", "\n"))))
                {
                    using (var armored = new ArmoredInputStream(memStream))
                    {
                        var key = new PgpSecretKeyRing(armored);
                        ctx.Import(key);
                    }
                }
            }
        }
    }
}
