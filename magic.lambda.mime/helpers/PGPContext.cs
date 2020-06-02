/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MimeKit;
using MimeKit.Cryptography;
using Org.BouncyCastle.Bcpg.OpenPgp;
using magic.node;
using magic.signals.contracts;

namespace magic.lambda.mime.helpers
{
    public class PGPContext : OpenPgpContext
    {
        readonly ISignaler _signaler;

        public PGPContext()
        { }

        public PGPContext(ISignaler signaler)
        {
            _signaler = signaler ?? throw new ArgumentNullException(nameof(signaler));
        }

        public Node ImportPrivateLambda { get; set; }

        protected override string GetPasswordForKey(PgpSecretKey key)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<PgpPublicKey> EnumeratePublicKeys(MailboxAddress mailbox)
        {
            return base.EnumeratePublicKeys(mailbox);
        }

        public override IEnumerable<PgpSecretKey> EnumerateSecretKeys(MailboxAddress mailbox)
        {
            return base.EnumerateSecretKeys(mailbox);
        }

        public override IEnumerable<PgpPublicKeyRing> EnumeratePublicKeyRings(MailboxAddress mailbox)
        {
            return base.EnumeratePublicKeyRings(mailbox);
        }

        public override IEnumerable<PgpPublicKeyRing> EnumeratePublicKeyRings()
        {
            return base.EnumeratePublicKeyRings();
        }

        public override IEnumerable<PgpPublicKey> EnumeratePublicKeys()
        {
            return base.EnumeratePublicKeys();
        }

        public override IEnumerable<PgpSecretKeyRing> EnumerateSecretKeyRings()
        {
            return base.EnumerateSecretKeyRings();
        }

        public override IEnumerable<PgpSecretKeyRing> EnumerateSecretKeyRings(MailboxAddress mailbox)
        {
            return base.EnumerateSecretKeyRings(mailbox);
        }

        public override IEnumerable<PgpSecretKey> EnumerateSecretKeys()
        {
            return base.EnumerateSecretKeys();
        }

        public override DigestAlgorithm GetDigestAlgorithm(string micalg)
        {
            return base.GetDigestAlgorithm(micalg);
        }

        public override string GetDigestAlgorithmName(DigestAlgorithm micalg)
        {
            return base.GetDigestAlgorithmName(micalg);
        }

        protected override PgpPublicKey GetPublicKey(MailboxAddress mailbox)
        {
            return base.GetPublicKey(mailbox);
        }

        protected override IList<PgpPublicKey> GetPublicKeys(IEnumerable<MailboxAddress> mailboxes)
        {
            return base.GetPublicKeys(mailboxes);
        }

        protected override PgpSecretKey GetSigningKey(MailboxAddress mailbox)
        {
            return base.GetSigningKey(mailbox);
        }

        public override DigitalSignatureCollection Verify(Stream content, Stream signatureData, CancellationToken cancellationToken = default)
        {
            return base.Verify(content, signatureData, cancellationToken);
        }

        public override Task<DigitalSignatureCollection> VerifyAsync(Stream content, Stream signatureData, CancellationToken cancellationToken = default)
        {
            return base.VerifyAsync(content, signatureData, cancellationToken);
        }

        #region [ -- Import keys -- ]

        public override void Import(PgpPublicKeyRing keyring)
        {
            base.Import(keyring);
        }

        public override void Import(PgpPublicKeyRingBundle bundle)
        {
            base.Import(bundle);
        }

        public override void Import(PgpSecretKeyRing keyring)
        {
            var clone = ImportPrivateLambda.Clone();
            var keyNode = new Node(".key");
            var fingerprint = new StringBuilder();

            var data = keyring.GetPublicKey().GetFingerprint();
            for (int idx = 0; idx < data.Length; idx++)
            {
                fingerprint.Append(data[idx].ToString("x2"));
                if (idx % 2 != 0)
                    fingerprint.Append("-");
            }

            keyNode.Add(new Node("fingerprint", fingerprint.ToString().TrimEnd('-')));
            clone.Insert(0, keyNode);
            _signaler.Signal("eval", clone);
        }

        public override void Import(PgpSecretKeyRingBundle bundle)
        {
            base.Import(bundle);
        }

        #endregion
    }
}
