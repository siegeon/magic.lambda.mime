
# Magic Lambda MIME

[![Build status](https://travis-ci.org/polterguy/magic.lambda.mime.svg?master)](https://travis-ci.org/polterguy/magic.lambda.mime)

Magic Lambda MIME give you the ability to parse and create MIME messages from Hyperlambda. It has 4 basic slots.

1. **[mime.parse]** - Parses a MIME message, and returns as lambda.
1. **[.mime.parse]** - Parses a native MimeEntity for you, and returns as lambda.
2. **[mime.create]** - Creates a MIME message for you, and returns the entire message as text.
3. **[.mime.create]** - Creates a MIME message for you, and returns it as a native MimeEntity object (not for usage directly from Hyperlambda code).
