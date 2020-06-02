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
            string mimeMessage = @"-----BEGIN PGP PRIVATE KEY BLOCK-----

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
=eoY4
-----END PGP PRIVATE KEY BLOCK-----";
            var lambda = Common.Evaluate(@"
.data
pgp.import:@""" + mimeMessage + @"""
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