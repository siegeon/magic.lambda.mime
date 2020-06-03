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

        /// <summary>
        /// Secret key rings, if statically provided.
        /// </summary>
        public PgpSecretKeyRing SecretKeyRings { get; set; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        protected override string GetPasswordForKey(PgpSecretKey key)
        {
            return Password;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override IEnumerable<PgpSecretKeyRing> EnumerateSecretKeyRings()
        {
            yield return SecretKeyRings;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override IEnumerable<PgpSecretKeyRing> EnumerateSecretKeyRings(MailboxAddress mailbox)
        {
            return base.EnumerateSecretKeyRings(mailbox);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override IEnumerable<PgpSecretKey> EnumerateSecretKeys()
        {
            return base.EnumerateSecretKeys();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override IEnumerable<PgpSecretKey> EnumerateSecretKeys(MailboxAddress mailbox)
        {
            return base.EnumerateSecretKeys(mailbox);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override IEnumerable<PgpPublicKeyRing> EnumeratePublicKeyRings()
        {
            return base.EnumeratePublicKeyRings();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override IEnumerable<PgpPublicKeyRing> EnumeratePublicKeyRings(MailboxAddress mailbox)
        {
            return base.EnumeratePublicKeyRings(mailbox);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override IEnumerable<PgpPublicKey> EnumeratePublicKeys()
        {
            return base.EnumeratePublicKeys();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override IEnumerable<PgpPublicKey> EnumeratePublicKeys(MailboxAddress mailbox)
        {
            return base.EnumeratePublicKeys(mailbox);
        }
    }
}
