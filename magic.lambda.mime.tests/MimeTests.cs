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
            string mimeMessage = @"Content-Type: multipart/mixed;
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
        public void ParseMessageWithHeaders()
        {
            string mimeMessage = @"Content-Type: text/plain
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