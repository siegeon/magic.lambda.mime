﻿/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace magic.lambda.mime.helpers
{
    /// <summary>
    /// PGP context to help cryptographically sign MIME messages.
    /// </summary>
    public class CreatePgpMimeContext : OpenPgpContext
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
