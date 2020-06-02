/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Collections.Generic;
using MimeKit;
using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace magic.lambda.mime.helpers
{
    /// <summary>
    /// PGP context to help decrypt and cryptographically sign MIME messages.
    /// Required by MimeKit.
    /// </summary>
    public class PgpContext : OpenPgpContext
    {
        /// <summary>
        /// Password to release private key.
        /// </summary>
        public string Password { get; set; }

        public PgpSecretKeyRing SecretKeyRings { get; set; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        protected override string GetPasswordForKey(PgpSecretKey key)
        {
            return Password;
        }

        public override IEnumerable<PgpSecretKeyRing> EnumerateSecretKeyRings(MailboxAddress mailbox)
        {
            yield return SecretKeyRings;
        }

        public override IEnumerable<PgpSecretKeyRing> EnumerateSecretKeyRings()
        {
            yield return SecretKeyRings;
        }
    }
}
