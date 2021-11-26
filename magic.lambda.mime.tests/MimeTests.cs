/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
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
        public void ParseRawMessage()
        {
            var entity = new TextPart("plain")
            {
                Text = "Hello World!"
            };
            var lambda = new Node("", entity);
            var signaler = Common.GetSignaler();
            signaler.Signal(".mime.parse", lambda);
            Assert.Equal("text/plain", lambda.GetEx<string>());
            Assert.Equal("content", lambda.Children.First().Name);
            Assert.Equal("Hello World!", lambda.Children.First().GetEx<string>());
        }

        [Fact]
        public void CreateSimpleMessage()
        {
            var signaler = Common.GetSignaler();
            var node = new Node("", "text/plain");
            var content = new Node("content", "foo bar");
            node.Add(content);
            signaler.Signal(".mime.create", node);
            var entity = node.Value as MimeEntity;
            try
            {
                Assert.Equal(@"Content-Type: text/plain

foo bar", entity.ToString());
            }
            finally
            {
                Common.DisposeEntity(entity);
            }
        }

        [Fact]
        public void CreateMessageWithHeaders()
        {
            var signaler = Common.GetSignaler();
            var node = new Node("", "text/plain");
            var content = new Node("content", "foo bar");
            node.Add(content);
            var headers = new Node("headers");
            node.Add(headers);
            var header = new Node("Foo-Bar", "howdy");
            headers.Add(header);
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
                Common.DisposeEntity(entity);
            }
        }

        [Fact]
        public void CreateMultipartMessage()
        {
            var signaler = Common.GetSignaler();

            // Creating a Multipart
            var node = new Node("", "multipart/mixed");
            var message1 = new Node("entity", "text/plain");
            node.Add(message1);
            var content1 = new Node("content", "some text");
            message1.Add(content1);
            var message2 = new Node("entity", "text/plain");
            node.Add(message2);
            var content2 = new Node("content", "some other text");
            message2.Add(content2);
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
                Common.DisposeEntity(entity);
            }
        }
    }
}