
# Magic Lambda MIME

[![Build status](https://travis-ci.org/polterguy/magic.lambda.mime.svg?master)](https://travis-ci.org/polterguy/magic.lambda.mime)

Magic Lambda MIME give you the ability to parse and create MIME messages from Hyperlambda. It has 4 basic slots.

1. **[mime.parse]** - Parses a MIME message, and returns as lambda.
1. **[.mime.parse]** - Parses a native MimeEntity for you, and returns as lambda.
2. **[mime.create]** - Creates a MIME message for you, and returns the entire message as text.
3. **[.mime.create]** - Creates a MIME message for you, and returns it as a native MimeEntity object (not for usage directly from Hyperlambda code).

## Parsing MIME messages

Below is an example of how parsing a MIME message might look like.

```
// Actual message
.msg:@"MIME-Version: 1.0
Content-Type: multipart/mixed;
        boundary=""XXXXboundary text""

This is a multipart message in MIME format.

--XXXXboundary text
Content-Type: text/plain

this is the body text
--XXXXboundary text
Content-Type: text/plain;

this is another body text
--XXXXboundary text--"

// Parsing the above message
mime.parse:x:@.msg
```

After evaluating the above, you'll end up with something resembling the following.

```
mime.parse
   entity:multipart/mixed
      entity:text/plain
         content:this is the body text
      entity:text/plain
         content:this is another body text
```

Notice how the slot creates a tree structure, perfectly resembling your original MIME message. It will also take care of
MIME headers for you, adding these into a **[headers]** collection, on a per message basis, depending upon whether or not
your message actually contains headers or not.

The **[.mime.parse]** semantically works identically, except it requires as its input a raw `MimeEntity` object from MimeKit.

## Creating a mime message

This slot is logically the exact opposite of the **[mime.parse]** slot, and can take (almost) the exact same input as
its sibling produces as output. Below is an example.

```
mime.create
   entity:multipart/mixed
      entity:text/plain
         content:this is the body text
      entity:text/plain
         content:this is another body text
```

Which of course wil result in something resembling the following after evaluation.

```
mime.create:@"MIME-Version: 1.0
Content-Type: multipart/mixed;
        boundary=""XXXXboundary text""

This is a multipart message in MIME format.

--XXXXboundary text
Content-Type: text/plain

this is the body text
--XXXXboundary text
Content-Type: text/plain;

this is another body text
--XXXXboundary text--"
```

The **[.mime.create]** slot, will semantically do the exact same thing, but instead of returning a piece of text, being the MIME message,
it will produce a raw `MimeEntity` that it returns to caller. This slot is used internally when the _"magic.lambda.mail"_ projects
constructs emails to send over an SMTP connection for instance.
