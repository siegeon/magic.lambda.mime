﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using MimeKit;
using MimeKit.IO;
using magic.node;
using magic.node.extensions;
using magic.signals.contracts;

namespace magic.lambda.mime
{
    /// <summary>
    /// Creates a MIME message and returns it as a MimeKit MimeMessage to caller (hidden)
    /// </summary>
    [Slot(Name = ".mime.create")]
    public class MimeCreatePrivate : ISlot
    {
        /// <summary>
        /// Implementation of your slot.
        /// </summary>
        /// <param name="signaler">Signaler that raised the signal.</param>
        /// <param name="input">Arguments to your slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            input.Value = CreateMimeMessage(input);
        }

        #region [ -- Private helper methods -- ]

        MimeEntity CreateMimeMessage(Node input)
        {
            var messageNodes = input.Children.Where(x => x.Name == "message");
            if (messageNodes.Count() != 1)
                throw new ArgumentException("Too many [message] nodes found for [.mime.create] slot to handle.");
            return CreateEntity(messageNodes.First());
        }

        /*
         * Create MimeEntity, or MIME part to be specific.
         */
        MimeEntity CreateEntity(Node input)
        {
            MimeEntity result = null;

            // Finding Content-Type of entity.
            var type = input.GetEx<string>();
            if (!type.Contains("/"))
                throw new ArgumentException($"'{type}' is an unknown MIME Content-Type. Please provide valid Content-Type as value of node.");
            var tokens = type.Split('/');
            if (tokens.Length != 2)
                throw new ArgumentException($"'{type}' is an unknown MIME Content-Type. Please provide valid Content-Type as value of node.");
            var mainType = tokens[0];
            var subType = tokens[1];
            switch (mainType)
            {
                case "text":
                    result = CreateLeafPart(mainType, subType, input);
                    break;
                case "multipart":
                    result = CreateMultipart(mainType, subType, input);
                    break;
            }
            return result;
        }

        /*
         * Creates a leaf part, implying no MimePart children.
         */
        MimePart CreateLeafPart(string mainType, string subType, Node messageNode)
        {
            // Retrieving [content] node.
            var contentNode = messageNode.Children.FirstOrDefault(x => x.Name == "content") ?? 
                throw new ArgumentNullException("No [content] provided in [message]");

            var result = new MimePart(ContentType.Parse(mainType + "/" + subType));
            CreateContentObjectFromObject(contentNode, result);
            return result;
        }

        Multipart CreateMultipart(string mainType, string subType, Node messageNode)
        {
            // Retrieving [content] node.
            var contentNode = messageNode.Children.FirstOrDefault(x => x.Name == "content") ??
                throw new ArgumentNullException("No [content] provided in [message]");

            var result = new Multipart(subType);
            foreach (var idxPart in contentNode.Children)
            {
                result.Add(CreateEntity(idxPart));
            }
            return result;
        }

        /*
         * Creates ContentObject from value found in node.
         */
        void CreateContentObjectFromObject(Node contentNode, MimePart part)
        {
            var stream = new MemoryBlockStream();
            var content = contentNode.GetEx<string>() ??
                throw new ArgumentNullException("Noe actual [content] supplied to message");
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            ContentEncoding encoding = ContentEncoding.Default;
            var encodingNode = contentNode.Children.FirstOrDefault(x => x.Name == "Content-Encoding");
            if (encodingNode != null)
                encoding = (ContentEncoding)Enum.Parse(typeof(ContentEncoding), encodingNode.GetEx<string>());
            part.Content = new MimeContent(stream, encoding);
        }

        #endregion
    }
}
