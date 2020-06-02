/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
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
    [Slot(Name = "pgp.public.import")]
    public class PgpPublicImport : ISlot
    {
        static PgpPublicImport()
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
            // Sanity checking invocation.
            var keyPlainText = input.GetEx<string>() ?? 
                throw new ArgumentNullException("No value provided to [pgp.public.import]");
            var lambda = input.Children.FirstOrDefault(x => x.Name == ".lambda") ??
                throw new ArgumentNullException("No [.lambda] provided to [pgp.public.import]");

            // Unwrapping key(s) and iterating through them, importing them one at the time.
            using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(keyPlainText)))
            {
                using (var armored = new ArmoredInputStream(memStream))
                {
                    var key = new PgpPublicKeyRing(armored);
                    foreach (PgpPublicKey idxKey in key.GetPublicKeys())
                    {
                        var keyNode = new Node(".key");
                        keyNode.Add(new Node("fingerprint", PgpHelpers.GetFingerprint(idxKey)));
                        keyNode.Add(new Node("content", PgpHelpers.GetKey(idxKey)));
                        keyNode.Add(new Node("created", idxKey.CreationTime));
                        keyNode.Add(new Node("valid-seconds", idxKey.GetValidSeconds()));
                        keyNode.Add(new Node("algorithm", idxKey.Algorithm.ToString()));
                        keyNode.Add(new Node("bit-strength", idxKey.BitStrength));
                        keyNode.Add(new Node("is-encryption", idxKey.IsEncryptionKey));
                        keyNode.Add(new Node("is-master", idxKey.IsMasterKey));
                        keyNode.Add(new Node("is-revoked", idxKey.IsRevoked()));

                        var ids = new Node("ids");
                        foreach (var idxId in idxKey.GetUserIds())
                        {
                            ids.Add(new Node(".", idxId.ToString()));
                        }
                        if (ids.Children.Any())
                            keyNode.Add(ids);

                        var exe = lambda.Clone();
                        lambda.Insert(0, keyNode);
                        signaler.Signal("eval", lambda);
                        lambda.Clear();
                        lambda.AddRange(exe.Children.ToList());
                    }
                }
            }
        }
    }
}
