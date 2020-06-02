/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace magic.lambda.mime.helpers
{
    public class PgpContext : OpenPgpContext
    {
        protected override string GetPasswordForKey(PgpSecretKey key)
        {
            return "8pr4ms";
        }
    }
}
