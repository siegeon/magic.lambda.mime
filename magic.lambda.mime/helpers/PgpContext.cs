/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

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
        /// <inheritdoc />
        /// </summary>
        protected override string GetPasswordForKey(PgpSecretKey key)
        {
            return Password;
        }
    }
}
