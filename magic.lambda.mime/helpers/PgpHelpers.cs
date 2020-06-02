/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.IO;
using System.Text;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace magic.lambda.mime.helpers
{
    public static class PgpHelpers
    {
        public static string GetFingerprint(PgpPublicKey key)
        {
            var builder = new StringBuilder();
            var data = key.GetFingerprint();
            for (int idx = 0; idx < data.Length; idx++)
            {
                builder.Append(data[idx].ToString("x2"));
            }
            return builder.ToString().ToUpperInvariant();
        }

        public static string GetKey(PgpPublicKey key)
        {
            using (var memStream = new MemoryStream())
            {
                using (var armored = new ArmoredOutputStream(memStream))
                {
                    key.Encode(armored);
                    armored.Flush();
                }
                memStream.Flush();
                memStream.Position = 0;
                using (var sr = new StreamReader(memStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static string GetKey(PgpSecretKey key)
        {
            using (var memStream = new MemoryStream())
            {
                using (var armored = new ArmoredOutputStream(memStream))
                {
                    key.Encode(armored);
                    armored.Flush();
                }
                memStream.Flush();
                memStream.Position = 0;
                using (var sr = new StreamReader(memStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
