/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using Xunit;
using MimeKit;
using magic.node;
using magic.node.extensions;

namespace magic.lambda.mime.tests
{
    public class PgpTests
    {
        [Fact]
        public void ImportPgpKeyPair()
        {
            string mimeMessage = @"-----BEGIN PGP PUBLIC KEY BLOCK-----
Comment: User-ID:	Thomas Hansen <thomas@gaiasoul.com>
Comment: Created:	02/06/2020 9:54 AM
Comment: Expires:	02/06/2022 12:00 PM
Comment: Type:	2048-bit RSA (secret key available)
Comment: Usage:	Signing, Encryption, Certifying User-IDs
Comment: Fingerprint:	463E0818186DDB6BF846D22492AC250C49CBEAF9


mQENBF7V98IBCADE5IzvcgltfoDIU60ahHpfdjiht1ja+vIyCsq/N2BAa0JAItOM
a/1tUux+2bSuaHc8e4c8VKUJvS5KJC1ElmoISnFnvpNSb/xTPsga2auP8IsptWRU
BWXIGRoztFUoK22le4KaKQDJL+Icrqi31DKo0TGgg628EOa0SlR8YPGctpqQ54Bi
FCg123bD1X8UL6QG1xS8KuXbTbzWHNOGisa8d+7mSxcnLlwliJBNCCGBxD+zwuus
5+hXIVI1OllApmUggoxDnqukpA6agOneISnEzX1teoBwBnUDUZLt4lcsTRr0OzrQ
QDIxZPCcBT0FUkFXBTCRxzyHi+WF9btpfYpTABEBAAG0I1Rob21hcyBIYW5zZW4g
PHRob21hc0BnYWlhc291bC5jb20+iQFUBBMBCAA+FiEERj4IGBht22v4RtIkkqwl
DEnL6vkFAl7V98ICGwMFCQPChE4FCwkIBwIGFQoJCAsCBBYCAwECHgECF4AACgkQ
kqwlDEnL6vlfNwf/UwVQpXoY98tAeHGFtlO9SHHFUCdzbsfnt3y3Cp+JmUp45O90
p9QpVNvWo4K9aKs5DHOePdEdcXuAC3taZxRFhFmQe9RdRrjl0fcEoaeuX3ATfsA7
a7KwAOiMl7QesAPY4Sln4pCARfh7NwFA94rGy8HDk3ja3j8kFRR+4ck8pRYizwOr
oNyFcO8YGgvs8EBB6FCHsWAr8z5aGB2jl9AGH64vIFEie1bvrq1v4XRBS/uSzIL0
Y4RcF3VohHyI0IqXAaSo4oNKPsr4PVmTqAajtWoBkMSRhjgim1neoYNRdXemRNul
zY1PHUHz8WKHxyTqG4JJJMFNQTjKB+I3H3YWdLkBDQRe1ffCAQgAqypNmcHGddys
HzerdnpeAkyjAlwByGfNs6O1X7FDKXsUHQXthH8sjYIpwIq1JHBCxlRBuzfEHSLR
AoK1Hndc8vL3dc1M6Ay9lFy3vWapaBH4XgLjT/kimanf12hQnYO2LNmwxw9U0gf6
sKjMhKnX+qu5jjKAm+jVZW37AJbcraEqpeb0INdkaOq1v1rp9g/8rKoCgQ6ZwwEh
5sIHHvYsAN2PqZ34SQJ6joRJjwjo/oUsYgpMFmPtTFiatxaDuM5ra9+Lt2j2du+S
1Jw4L1ISvglANYuhdk0M0wJ+3Fl+vZEC8/TGQ1X2A1kDNbS6kgRGEUuCLSyxIoO+
z1RSgPZmsQARAQABiQE8BBgBCAAmFiEERj4IGBht22v4RtIkkqwlDEnL6vkFAl7V
98ICGwwFCQPChE4ACgkQkqwlDEnL6vnMSwf+KkmoIB6D0cr35EC0xKs3SjJ+kImm
kXMl64qyEfaK/SrGC4iG5upt1TevIH97NERBiiYtguoc+4CZSacDvF5ONUxOd2ZK
8jOrpghkxj7Omlv4JLVBFqWZzznhR70M8Y7+V2SFQm6oDZkYJfaOs4vftCYVEEeh
HBHk0a98KkMo7mB9o4A96b4F8jeK0TNXmN4mm2sM6sWYMPdqAkMo3nIn9eQGc7z2
QU5h7Kf7ysFTdT5+XvfisiJRjYHwfm1+SeN7VwyXz8rDRflv9HUUK4iHOyJw5xXh
Vlw0KvRqBVBcqWr29a4QydWFZmfBs0C9GiFNcIo3zkEsf4MQFRIPAXbeMA==
=X3uP
-----END PGP PUBLIC KEY BLOCK-----";
            var lambda = Common.Evaluate(@"
.keys
pgp.keys.public.import:@""" + mimeMessage + @"""
   .lambda
      add:x:@.keys
         get-nodes:x:@.key");
            var data = lambda.Children.First(x => x.Name == ".keys");

            Assert.Equal(2, data.Children.Count());

            // Checking first key
            Assert.Equal("463E0818186DDB6BF846D22492AC250C49CBEAF9", data.Children.First().Children.First(x => x.Name == "fingerprint").Value);
            Assert.Contains("-----BEGIN PGP PUBLIC KEY BLOCK-----", data.Children.First().Children.First(x => x.Name == "content").Get<string>());
            Assert.Equal(typeof(DateTime), data.Children.First().Children.First(x => x.Name == "created").Value.GetType());
            Assert.True(data.Children.First().Children.First(x => x.Name == "valid-seconds").Get<long>() > 1000);
            Assert.Equal("RsaGeneral", data.Children.First().Children.First(x => x.Name == "algorithm").Value);
            Assert.Equal(2048, data.Children.First().Children.First(x => x.Name == "bit-strength").Value);
            Assert.True(data.Children.First().Children.First(x => x.Name == "is-encryption").Get<bool>());
            Assert.True(data.Children.First().Children.First(x => x.Name == "is-master").Get<bool>());
            Assert.False(data.Children.First().Children.First(x => x.Name == "is-revoked").Get<bool>());
            Assert.Contains("Thomas Hansen <thomas@gaiasoul.com>", data.Children.First().Children.First(x => x.Name == "ids").Children.First().Get<string>());

            // Checking second key
            Assert.Equal("686C520F02716F59C9694AFE5A95B76FEC21441C", data.Children.Skip(1).First().Children.First(x => x.Name == "fingerprint").Value);
            Assert.Contains("-----BEGIN PGP MESSAGE-----", data.Children.Skip(1).First().Children.First(x => x.Name == "content").Get<string>());
            Assert.Equal(typeof(DateTime), data.Children.Skip(1).First().Children.First(x => x.Name == "created").Value.GetType());
            Assert.True(data.Children.Skip(1).First().Children.First(x => x.Name == "valid-seconds").Get<long>() > 1000);
            Assert.Equal("RsaGeneral", data.Children.Skip(1).First().Children.First(x => x.Name == "algorithm").Value);
            Assert.Equal(2048, data.Children.Skip(1).First().Children.First(x => x.Name == "bit-strength").Value);
            Assert.True(data.Children.Skip(1).First().Children.First(x => x.Name == "is-encryption").Get<bool>());
            Assert.False(data.Children.Skip(1).First().Children.First(x => x.Name == "is-master").Get<bool>());
            Assert.False(data.Children.Skip(1).First().Children.First(x => x.Name == "is-revoked").Get<bool>());
            Assert.Null(data.Children.Skip(1).First().Children.FirstOrDefault(x => x.Name == "ids"));
        }
    }
}