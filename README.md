
# Magic Lambda MIME

[![Build status](https://travis-ci.org/polterguy/magic.lambda.mime.svg?master)](https://travis-ci.org/polterguy/magic.lambda.mime)

Magic Lambda MIME give you the ability to parse and create MIME messages from Hyperlambda. It contains 6 basic slots.

1. **[mime.parse]** - Parses a MIME message, and returns as lambda.
1. **[.mime.parse]** - Parses a native MimeEntity for you, and returns as lambda.
2. **[mime.create]** - Creates a MIME message for you, and returns the entire message as text.
3. **[.mime.create]** - Creates a MIME message for you, and returns it as a native MimeEntity object (not for usage directly from Hyperlambda code).
4. **[pgp.keys.private.import]** - Imports an ASCII armored private PGP key bundle, in addition to any public keys found in the bundle.
4. **[pgp.keys.public.import]** - Imports an ASCII armored public PGP key bundle.

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

## PGP Cryptography

This project also supports encrypting, and cryptographically signing MIME messages, in addition to decrypting and verifying signed
messages. To cryptographically sign a MIME message with your private PGP key, you can use something such as follows.

```
mime.create
   entity:text/plain
      sign:@"-----BEGIN PGP PRIVATE KEY BLOCK----- ...... etc"
         password:your-pgp-key-password-here
      content:Foo bar
```

To encrypt a message you could do something such as follows.

```
mime.create
   entity:text/plain
      encrypt:@"-----BEGIN PGP PUBLIC KEY BLOCK----- ..... etc"
      content:Foo bar
```

You can encrypt and sign the message in one go, by adding both a private **[sign]** key and its password, in addition to a public
encryption key, using **[encrypt]**. If you wish to encrypt the same message for multiple recipients, you can add a collection
of public PGP keys that will be used to encrypt the message, such as follows.

```
mime.create
   entity:text/plain
      encrypt
         .:@"-----BEGIN PGP PUBLIC KEY BLOCK----- ..... etc, key 1"
         .:@"-----BEGIN PGP PUBLIC KEY BLOCK----- ..... etc, key 2"
      content:Foo bar
```

## Importing PGP keys

Notice, these slots expects a PGP key bundle, either private or public, and will unwrap each public and private key found in the bundle,
and invoke your **[.lambda]** callback once for each key found in the bundle. This callback will be given the fingerprint, ID, ids, etc
for each key in your bundle. Usage is something as follows.

```
pgp.keys.public.import:@"-----BEGIN PGP PUBLIC KEY BLOCK----- ..... etc"
   .lambda
      lambda2hyper:x:.
      log.info:x:-
```

## License

Although most of Magic's source code is publicly available, Magic is _not_ Open Source or Free Software.
You have to obtain a valid license key to install it in production, and I normally charge a fee for such a
key. You can [obtain a license key here](https://servergardens.com/buy/).
Notice, 7 days after you put Magic into production, it will stop working, unless you have a valid
license for it.

* [Get licensed](https://servergardens.com/buy/)

Copyright(c) Thomas Hansen 2019 - 2020, Thomas Hansen - thomas@servergardens.com
