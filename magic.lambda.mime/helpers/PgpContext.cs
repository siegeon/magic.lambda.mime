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

        public override IEnumerable<PgpSecretKeyRing> EnumerateSecretKeyRings()
        {
            yield return SecretKeyRings;
        }

        public override IEnumerable<PgpSecretKeyRing> EnumerateSecretKeyRings(MailboxAddress mailbox)
        {
            return base.EnumerateSecretKeyRings(mailbox);
        }

        public override IEnumerable<PgpSecretKey> EnumerateSecretKeys()
        {
            return base.EnumerateSecretKeys();
        }

        public override IEnumerable<PgpSecretKey> EnumerateSecretKeys(MailboxAddress mailbox)
        {
            return base.EnumerateSecretKeys(mailbox);
        }

        public override IEnumerable<PgpPublicKeyRing> EnumeratePublicKeyRings()
        {
            return base.EnumeratePublicKeyRings();
        }

        public override IEnumerable<PgpPublicKeyRing> EnumeratePublicKeyRings(MailboxAddress mailbox)
        {
            return base.EnumeratePublicKeyRings(mailbox);
        }

        public override IEnumerable<PgpPublicKey> EnumeratePublicKeys()
        {
            return base.EnumeratePublicKeys();
        }

        public override IEnumerable<PgpPublicKey> EnumeratePublicKeys(MailboxAddress mailbox)
        {
            return base.EnumeratePublicKeys(mailbox);
        }
    }
}
