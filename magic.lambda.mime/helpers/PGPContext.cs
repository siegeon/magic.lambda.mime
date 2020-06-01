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
    public class PGPContext : OpenPgpContext
    {
        protected override string GetPasswordForKey(PgpSecretKey key)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<PgpPublicKey> EnumeratePublicKeys(MailboxAddress mailbox)
        {
            return base.EnumeratePublicKeys(mailbox);
        }

        public override IEnumerable<PgpSecretKey> EnumerateSecretKeys(MailboxAddress mailbox)
        {
            return base.EnumerateSecretKeys(mailbox);
        }
    }
}
