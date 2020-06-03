/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using Xunit;
using magic.node.extensions;

namespace magic.lambda.mime.tests
{
    public class PgpTests
    {
        const string PUBLIC_KEYPAIR = @"-----BEGIN PGP PUBLIC KEY BLOCK-----
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

        const string PRIVATE_KEYPAIR = @"-----BEGIN PGP PRIVATE KEY BLOCK-----

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
=Rqgu
-----END PGP PRIVATE KEY BLOCK-----
";

        const string SECRET_KEY = @"-----BEGIN PGP PRIVATE KEY BLOCK-----
Version: BCPG C# v1.8.5.0

lQPGBF7V98IBCADE5IzvcgltfoDIU60ahHpfdjiht1ja+vIyCsq/N2BAa0JAItOM
a/1tUux+2bSuaHc8e4c8VKUJvS5KJC1ElmoISnFnvpNSb/xTPsga2auP8IsptWRU
BWXIGRoztFUoK22le4KaKQDJL+Icrqi31DKo0TGgg628EOa0SlR8YPGctpqQ54Bi
FCg123bD1X8UL6QG1xS8KuXbTbzWHNOGisa8d+7mSxcnLlwliJBNCCGBxD+zwuus
5+hXIVI1OllApmUggoxDnqukpA6agOneISnEzX1teoBwBnUDUZLt4lcsTRr0OzrQ
QDIxZPCcBT0FUkFXBTCRxzyHi+WF9btpfYpTABEBAAH+BwMCD134G9vjOSHOhk2f
t3TeXjNvMtRdJuagqvSBubYdbC1zzb/O6fitxXbouNTJaoJnNa4y+IC2PUeJPHF/
x2PqFZRBts2KU1oSGyWy+nM5EsVa7FN/5SDhIgshw1eHlc+dLxeyXIFDxJ8ZUKhu
LAvjOyLN4dx0+VLj9wGQfXtjCdK2O6n3Sxvnwr31YI7XKj6HeEFMzA/b6LF8ZetM
D+h3V6B+fDIrtKN5QTVkZ4hGBbwjAZVpqxviC24cmZl4EVtarJQcQwVxF4CSbpHN
3Wmzer9+zuPvOa8KBJUsM2+ttXsFamUkp7Tgi7mpfYB7BJ1WYTYJn5X91pCvmhms
15spfiuy59o/UR7kw+P4h0Ssc9cN1rYahh3RSOet8+nsDy+Z3gWxQJHvyM0i8t3H
VDPQMLtrvs/Jn9VRE+rrc9hIJRSG06MVoEa/mS1wK8X3r3nm4qAEeK8E11dI05SM
U7lgXQwK2PHg5D5XiVTCNUYxeeUipXbfkIXcY+a1QJA0nyiVWGTlbtHk0MCGO2a9
g99MuLTPxA1BUaN4lAx/5yd/jNnSMEwHMANdjFMGjoEJ9rPRPvPv4x8TiEZ7vXEJ
wddgg+GUlmpLk564rrDqdEE6+QJI1Tg7sysv8qgrTR18WFDw6Nt4ms+dubCpDTNc
0fhTjCGkH9uPuErCDVMSSTcd5154GVOCOKn4QiEz0sR3Hr3AIS22kb1s+3GrGDrN
O5uKnRRSIMB6WqguFtSHNy+RhMy9/9/lsvnTk8ME/fgEL1YN1JRF5L2nN2rdDCTT
igknWz33kWRAStnxcbyoQmb9fmg+XlT116NWu4fakqGiDsV9g6tyDG9sDP5r9NV0
RaOH5FTZ8fabq+aWyg9ACsl1v8WWkqIHmyQTm+rNDKtElsdqx/2iIarjzAENL2ob
m9nLMScfBjDXtCNUaG9tYXMgSGFuc2VuIDx0aG9tYXNAZ2FpYXNvdWwuY29tPokB
VAQTAQgAPhYhBEY+CBgYbdtr+EbSJJKsJQxJy+r5BQJe1ffCAhsDBQkDwoROBQsJ
CAcCBhUKCQgLAgQWAgMBAh4BAheAAAoJEJKsJQxJy+r5XzcH/1MFUKV6GPfLQHhx
hbZTvUhxxVAnc27H57d8twqfiZlKeOTvdKfUKVTb1qOCvWirOQxznj3RHXF7gAt7
WmcURYRZkHvUXUa45dH3BKGnrl9wE37AO2uysADojJe0HrAD2OEpZ+KQgEX4ezcB
QPeKxsvBw5N42t4/JBUUfuHJPKUWIs8Dq6DchXDvGBoL7PBAQehQh7FgK/M+Whgd
o5fQBh+uLyBRIntW766tb+F0QUv7ksyC9GOEXBd1aIR8iNCKlwGkqOKDSj7K+D1Z
k6gGo7VqAZDEkYY4IptZ3qGDUXV3pkTbpc2NTx1B8/Fih8ck6huCSSTBTUE4ygfi
Nx92FnQ=
=fqsf
-----END PGP PRIVATE KEY BLOCK-----
";

        const string PUBLIC_KEY = @"-----BEGIN PGP PUBLIC KEY BLOCK-----
Version: BCPG C# v1.8.5.0

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
zY1PHUHz8WKHxyTqG4JJJMFNQTjKB+I3H3YWdA==
=6XAy
-----END PGP PUBLIC KEY BLOCK-----
";

        [Fact]
        public void ImportPublicKey()
        {
            var lambda = Common.Evaluate(@"
.keys
pgp.keys.public.import:@""" + PUBLIC_KEYPAIR + @"""
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

        [Fact]
        public void ImportPrivateKeyPair()
        {
            var lambda = Common.Evaluate(@"
.keys
pgp.keys.private.import:@""" + PRIVATE_KEYPAIR + @"""
   .lambda
      add:x:@.keys
         get-nodes:x:@.key");
            var data = lambda.Children.First(x => x.Name == ".keys");

            Assert.Equal(3, data.Children.Count());

            // Checking first key
            Assert.Equal("463E0818186DDB6BF846D22492AC250C49CBEAF9", data.Children.First().Children.First(x => x.Name == "fingerprint").Value);
            Assert.True(data.Children.First().Children.First(x => x.Name == "private").Get<bool>());
            Assert.Contains("-----BEGIN PGP PRIVATE KEY BLOCK-----", data.Children.First().Children.First(x => x.Name == "content").Get<string>());
            Assert.True(data.Children.First().Children.First(x => x.Name == "is-master").Get<bool>());
            Assert.True(data.Children.First().Children.First(x => x.Name == "is-signing-key").Get<bool>());
            Assert.Equal("Aes128", data.Children.First().Children.First(x => x.Name == "encryption-algorithm").Value);
            Assert.Contains("Thomas Hansen <thomas@gaiasoul.com>", data.Children.First().Children.First(x => x.Name == "ids").Children.First().Get<string>());

            // Checking second key
            Assert.Equal("686C520F02716F59C9694AFE5A95B76FEC21441C", data.Children.Skip(1).First().Children.First(x => x.Name == "fingerprint").Value);
            Assert.True(data.Children.Skip(1).First().Children.First(x => x.Name == "private").Get<bool>());
            Assert.Contains("-----BEGIN PGP MESSAGE-----", data.Children.Skip(1).First().Children.First(x => x.Name == "content").Get<string>());
            Assert.False(data.Children.Skip(1).First().Children.First(x => x.Name == "is-master").Get<bool>());
            Assert.True(data.Children.Skip(1).First().Children.First(x => x.Name == "is-signing-key").Get<bool>());
            Assert.Equal("Aes128", data.Children.Skip(1).First().Children.First(x => x.Name == "encryption-algorithm").Value);
            Assert.Null(data.Children.Skip(1).First().Children.FirstOrDefault(x => x.Name == "ids"));

            // Checking public key for master key
            Assert.Equal("463E0818186DDB6BF846D22492AC250C49CBEAF9", data.Children.Skip(2).First().Children.First(x => x.Name == "fingerprint").Value);
            Assert.False(data.Children.Skip(2).First().Children.First(x => x.Name == "private").Get<bool>());
            Assert.Contains("-----BEGIN PGP PUBLIC KEY BLOCK-----", data.Children.Skip(2).First().Children.First(x => x.Name == "content").Get<string>());
            Assert.Equal(typeof(DateTime), data.Children.Skip(2).First().Children.First(x => x.Name == "created").Value.GetType());
            Assert.True(data.Children.Skip(2).First().Children.First(x => x.Name == "valid-seconds").Get<long>() > 1000);
            Assert.Equal("RsaGeneral", data.Children.Skip(2).First().Children.First(x => x.Name == "algorithm").Value);
            Assert.Equal(2048, data.Children.Skip(2).First().Children.First(x => x.Name == "bit-strength").Value);
            Assert.True(data.Children.Skip(2).First().Children.First(x => x.Name == "is-encryption").Get<bool>());
            Assert.True(data.Children.Skip(2).First().Children.First(x => x.Name == "is-master").Get<bool>());
            Assert.False(data.Children.Skip(2).First().Children.First(x => x.Name == "is-revoked").Get<bool>());
            Assert.Contains("Thomas Hansen <thomas@gaiasoul.com>", data.Children.Skip(2).First().Children.First(x => x.Name == "ids").Children.First().Get<string>());
        }

        [Fact]
        public void Sign()
        {
            var lambda = Common.Evaluate(string.Format(@"
.key:@""{0}""
mime.create
   entity:text/plain
      sign:x:@.key
         password:8pr4ms
      content:Foo bar
", SECRET_KEY));
            var entity = lambda.Children.FirstOrDefault(x => x.Name == "mime.create");
            Assert.Empty(entity.Children);
        }

        [Fact]
        public void Encrypt()
        {
            var lambda = Common.Evaluate(string.Format(@"
.key:@""{0}""
mime.create
   entity:text/plain
      encrypt:x:@.key
      content:Foo bar
", PUBLIC_KEY));
            var entity = lambda.Children.FirstOrDefault(x => x.Name == "mime.create");
            Assert.Empty(entity.Children);
            Assert.Contains(@"-----END PGP MESSAGE-----", entity.Get<string>());
            Assert.Contains(@"Content-Type: application/pgp-encrypted", entity.Get<string>());
        }

        [Fact]
        public void SignAndEncrypt()
        {
            var lambda = Common.Evaluate(string.Format(@"
.public:@""{1}""
.key:@""{0}""
mime.create
   entity:text/plain
      sign:x:@.key
         password:8pr4ms
      encrypt:x:@.public
      content:Foo bar
", SECRET_KEY, PUBLIC_KEY));
            var entity = lambda.Children.FirstOrDefault(x => x.Name == "mime.create");
            Assert.Empty(entity.Children);
        }
    }
}
