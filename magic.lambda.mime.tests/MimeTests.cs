/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using Xunit;

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
Content - Type: text / plain

this is the body text

--XXXXboundary text
Content - Type: text / plain;
            Content - Disposition: attachment;
            filename = ""test.txt""

this is the attachment text

--XXXXboundary text--";
            var lambda = Common.Evaluate($"mime.parse:@\"{mimeMessage.Replace(@"""", @"""""")}\"");
        }
    }
}
