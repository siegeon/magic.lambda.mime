/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Linq;
using Xunit;
using MimeKit;
using magic.node;
using magic.node.extensions;

namespace magic.lambda.mime.tests
{
    public class MimeTests
    {
        [Fact]
        public void ParseMultipartMessage()
        {
            string mimeMessage = @"MIME-Version: 1.0
Content-Type: multipart/mixed;
        boundary=""XXXXboundary text""

This is a multipart message in MIME format.

--XXXXboundary text
Content-Type: text/plain

this is the body text
--XXXXboundary text
Content-Type: text/plain;

this is another body text
--XXXXboundary text--";
            var lambda = Common.Evaluate($"mime.parse:@\"{mimeMessage.Replace(@"""", @"""""")}\"");
            Assert.Single(lambda.Children.First().Children);
            Assert.Equal("entity",
                lambda.Children.First().Children.First().Name);
            Assert.Equal("multipart/mixed",
                lambda.Children.First().Children.First().GetEx<string>());
            Assert.Equal(2,
                lambda.Children.First().Children.First().Children.Count());
            Assert.Equal("entity",
                lambda.Children.First().Children.First().Children.First().Name);
            Assert.Equal("text/plain",
                lambda.Children.First().Children.First().Children.First().GetEx<string>());
            Assert.Equal("entity",
                lambda.Children.First().Children.First().Children.Skip(1).First().Name);
            Assert.Equal("text/plain",
                lambda.Children.First().Children.First().Children.Skip(1).First().GetEx<string>());
            Assert.Equal("content",
                lambda.Children.First().Children.First().Children.First().Children.First().Name);
            Assert.Equal("this is the body text",
                lambda.Children.First().Children.First().Children.First().Children.First().GetEx<string>());
            Assert.Equal("content",
                lambda.Children.First().Children.First().Children.Skip(1).First().Children.First().Name);
            Assert.Equal("this is another body text",
                lambda.Children.First().Children.First().Children.Skip(1).First().Children.First().GetEx<string>());
        }

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
.data
pgp.public.import:@""" + mimeMessage + @"""
   .lambda
      set-value:x:@.data
         get-value:x:@.key");
        }

        [Fact]
        public void ParseMessageWithHeaders()
        {
            string mimeMessage = @"MIME-Version: 1.0
Content-Type: text/plain
Content-Disposition: inline

Hello World!";
            var lambda = Common.Evaluate($"mime.parse:@\"{mimeMessage.Replace(@"""", @"""""")}\"");
            Assert.Single(lambda.Children.First().Children);
            Assert.Equal("entity",
                lambda.Children.First().Children.First().Name);
            Assert.Equal("text/plain",
                lambda.Children.First().Children.First().GetEx<string>());
            Assert.Equal("headers",
                lambda.Children.First().Children.First().Children.First().Name);
            Assert.Equal("Content-Disposition",
                lambda.Children.First().Children.First().Children.First().Children.First().Name);
            Assert.Equal("inline",
                lambda.Children.First().Children.First().Children.First().Children.First().GetEx<string>());
            Assert.Equal("content",
                lambda.Children.First().Children.First().Children.Skip(1).First().Name);
            Assert.Equal("Hello World!",
                lambda.Children.First().Children.First().Children.Skip(1).First().GetEx<string>());
        }

        [Fact]
        public void ParseRawMessage()
        {
            var entity = new TextPart("plain")
            {
                Text = "Hello World!"
            };
            var lambda = new Node("", entity);
            var signaler = Common.GetSignaler();
            signaler.Signal(".mime.parse", lambda);
            Assert.Single(lambda.Children.First().Children);
            Assert.Equal("entity",
                lambda.Children.First().Name);
            Assert.Equal("text/plain",
                lambda.Children.First().GetEx<string>());
            Assert.Equal("content",
                lambda.Children.First().Children.First().Name);
            Assert.Equal("Hello World!",
                lambda.Children.First().Children.First().GetEx<string>());
        }

        [Fact]
        public void CreateSimpleMessage()
        {
            var signaler = Common.GetSignaler();
            var node = new Node("");
            var message = new Node("entity", "text/plain");
            var content = new Node("content", "foo bar");
            message.Add(content);
            node.Add(message);
            signaler.Signal(".mime.create", node);
            var entity = node.Value as MimeEntity;
            try
            {
                Assert.Equal(@"Content-Type: text/plain

foo bar", entity.ToString());
            }
            finally
            {
                Common.Dispose(entity);
            }
        }

        [Fact]
        public void CreateMessageWithHeaders()
        {
            var signaler = Common.GetSignaler();
            var node = new Node("");
            var message = new Node("entity", "text/plain");
            var content = new Node("content", "foo bar");
            message.Add(content);
            var headers = new Node("headers");
            message.Add(headers);
            var header = new Node("Foo-Bar", "howdy");
            headers.Add(header);
            node.Add(message);
            signaler.Signal(".mime.create", node);
            var entity = node.Value as MimeEntity;
            try
            {
                Assert.Equal(@"Content-Type: text/plain
Foo-Bar: howdy

foo bar", entity.ToString());
            }
            finally
            {
                Common.Dispose(entity);
            }
        }

        [Fact]
        public void CreateMultipartMessage()
        {
            var signaler = Common.GetSignaler();

            // Creating a Multipart
            var node = new Node("");
            var message = new Node("entity", "multipart/mixed");
            var content = new Node("content");
            var message2 = new Node("entity", "text/plain");
            var content2 = new Node("content", "some text");
            content.Add(message2);
            message2.Add(content2);
            var message3 = new Node("entity", "text/plain");
            var content3 = new Node("content", "some other text");
            content.Add(message3);
            message3.Add(content3);
            message.Add(content);
            node.Add(message);
            signaler.Signal(".mime.create", node);
            var entity = node.Value as MimeEntity;
            try
            {

                // Running through a couple of simple asserts.
                Assert.Equal(typeof(Multipart), entity.GetType());
                var multipart = entity as Multipart;
                Assert.Equal(2, multipart.Count);
                Assert.Equal(typeof(MimePart), multipart.First().GetType());
                Assert.Equal(typeof(MimePart), multipart.Skip(1).First().GetType());
                var text1 = multipart.First() as MimePart;
                Assert.Equal(@"Content-Type: text/plain

some text", text1.ToString());
                var text2 = multipart.Skip(1).First() as MimePart;
                Assert.Equal(@"Content-Type: text/plain

some other text", text2.ToString());
            }
            finally
            {
                Common.Dispose(entity);
            }
        }
    }
}