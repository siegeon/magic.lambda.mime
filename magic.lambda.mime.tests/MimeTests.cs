/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Linq;
using Xunit;
using magic.node.extensions;

namespace magic.lambda.mime.tests
{
    public class MimeTests
    {
        [Fact]
        public void ParseSimpleMessage()
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
            Assert.Equal("message",
                lambda.Children.First().Children.First().Name);
            Assert.Equal("multipart/mixed",
                lambda.Children.First().Children.First().GetEx<string>());
            Assert.Equal(2, 
                lambda.Children.First().Children.First().Children.Count());
            Assert.Equal("message", 
                lambda.Children.First().Children.First().Children.First().Name);
            Assert.Equal("text/plain",
                lambda.Children.First().Children.First().Children.First().GetEx<string>());
            Assert.Equal("message",
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
            string mimeMessage = @"MIME-Version: 1.0
Content-Type: text/plain
Content-Disposition: inline

Hello World!";
            var lambda = Common.Evaluate($"mime.parse:@\"{mimeMessage.Replace(@"""", @"""""")}\"");
            Assert.Single(lambda.Children.First().Children);
            Assert.Equal("message",
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
    }
}
