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
        public void ParseSignedMessage()
        {
            string mimeMessage = @"Return-Path: <thomas@gaiasoul.com>
Received: from [192.168.0.28] ([213.140.201.154])
        by smtp.gmail.com with ESMTPSA id l131sm6488464wmb.5.2017.08.24.08.54.51
        for <asgeir@dagligdata.no>
        (version=TLS1 cipher=AES128-SHA bits=128/128);
        Thu, 24 Aug 2017 08:54:52 -0700 (PDT)
From: Thomas Hansen <thomas@gaiasoul.com>
Date: Thu, 24 Aug 2017 18:54:51 +0300
Subject: Pressemelding - Velkommen til mitt hjem
Message-Id: <22M5Z6M462U4.DD26BQA3Q92I1@ubuntu>
To: ""Asgeir Høgli"" <asgeir@dagligdata.no>
MIME-Version: 1.0
Content-Type: multipart/signed; boundary=""=-70CJRx+kzBN9hWTAOosSdQ==""; protocol=""application/pgp-signature""; micalg=pgp-sha256

--=-70CJRx+kzBN9hWTAOosSdQ==
Content-Type: multipart/alternative; boundary=""=-ZbhtG5e/mW81PPRARfJYbw==""

--=-ZbhtG5e/mW81PPRARfJYbw==
Content-Type: text/plain
X-Type: markdown
Content-Transfer-Encoding: quoted-printable

Heisann Asgeir, jeg la deg opp p=C3=A5 denne ogs=C3=A5, slik at du kan se e=
tt eksempel p=C3=A5 hva jeg kan gj=C3=B8re ... :)

Lenken under her, leder faktisk inn i mitt hjem. Der har jeg satt opp en Li=
nux server, som fungerer som min personlige hjemme cloud. Derifra kan jeg l=
ese eposten min, gjennom webmail interface, likevel ved bruk av ende til en=
de PGP kryptering. P=C3=A5 toppen er det installert ett SSL certifikat, som=
 gj=C3=B8r all aksess til serveren kryptert og sikker. Hvilke vil si at jeg=
 kan lese eposten min, kryptert, over for eksempel en iPhone eller iPad, fr=
a uansett hvor jeg m=C3=A5tte befinne meg i verden. Med mindre det kommer e=
tt str=C3=B8mbrudd, ol, i huset der jeg bor.

https://home.gaiasoul.com/welcome

Det tar meg ca. 30 minutter =C3=A5 sette opp en lignende konfigurasjon, for=
 bedriftskunder med flere ansatte litt lengre tid (selvf=C3=B8lgelig). I ti=
llegg har jeg laget ett script, som setter opp hele systemet, nesten 100% a=
utomatisert. En IT-kyndig person kan sikkert installere sin egen server sli=
k, i l=C3=B8pet av 30 minutter.

Serveren dere bes=C3=B8ker, er en gammel utrangert laptop, som er konverter=
t til en Ubuntu Server, som kj=C3=B8rer Apache, Linux, MySQL og Mono. =C3=
=98verst har jeg installert Phosphorus Five, som er mitt eget-utviklete web=
 operativsystem, som er ekstremt modulbasert. Phosphorus Five er utviklet i=
 C# og ASP.NET.

Jeg har en helt vanlig standard internett tilkobling, fra en av de mer popu=
l=C3=A6re internett leverand=C3=B8rene som finnes p=C3=A5 Kypros.

Det er (naturligvis) litt ""lag"" i nettverket, men jeg har testet det ekster=
nt, og selv i fra land som S=C3=B8r Afrika ol, s=C3=A5 loader hjemme web se=
rveren min sidene p=C3=A5 1-3 sekunder.

Hele sulamitten er Fri Programvare og =C3=85pen Kildekode. Jeg er selvf=C3=
=B8lgelig disponibel til intervju, ol.


Ha en fin dag :)

Thomas Hansen

--=20
Got Privacy ...?

https://gaiasoul.com/got-privacy
--=-ZbhtG5e/mW81PPRARfJYbw==
Content-Type: text/html
Content-Transfer-Encoding: quoted-printable

<html><head><title>Email</title></head><body><div><p>Heisann Asgeir, jeg la=
 deg opp p=C3=A5 denne ogs=C3=A5, slik at du kan se ett eksempel p=C3=A5 hv=
a jeg kan gj=C3=B8re ... :)</p><p>Lenken under her, leder faktisk inn i mit=
t hjem. Der har jeg satt opp en Linux server, som fungerer som min personli=
ge hjemme cloud. Derifra kan jeg lese eposten min, gjennom webmail interfac=
e, likevel ved bruk av ende til ende PGP kryptering. P=C3=A5 toppen er det =
installert ett SSL certifikat, som gj=C3=B8r all aksess til serveren krypte=
rt og sikker. Hvilke vil si at jeg kan lese eposten min, kryptert, over for=
 eksempel en iPhone eller iPad, fra uansett hvor jeg m=C3=A5tte befinne meg=
 i verden. Med mindre det kommer ett str=C3=B8mbrudd, ol, i huset der jeg b=
or.</p><p><a target=3D'_blank' href=3D'https://home.gaiasoul.com/welcome'><=
strong>home.gaiasoul.com</strong>/welcome</a></p><p>Det tar meg ca. 30 minu=
tter =C3=A5 sette opp en lignende konfigurasjon, for bedriftskunder med fle=
re ansatte litt lengre tid (selvf=C3=B8lgelig). I tillegg har jeg laget ett=
 script, som setter opp hele systemet, nesten 100% automatisert. En IT-kynd=
ig person kan sikkert installere sin egen server slik, i l=C3=B8pet av 30 m=
inutter.</p><p>Serveren dere bes=C3=B8ker, er en gammel utrangert laptop, s=
om er konvertert til en Ubuntu Server, som kj=C3=B8rer Apache, Linux, MySQL=
 og Mono. =C3=98verst har jeg installert Phosphorus Five, som er mitt eget-=
utviklete web operativsystem, som er ekstremt modulbasert. Phosphorus Five =
er utviklet i C# og ASP.NET.</p><p>Jeg har en helt vanlig standard internet=
t tilkobling, fra en av de mer popul=C3=A6re internett leverand=C3=B8rene s=
om finnes p=C3=A5 Kypros.</p><p>Det er (naturligvis) litt ""lag"" i nettverke=
t, men jeg har testet det eksternt, og selv i fra land som S=C3=B8r Afrika =
ol, s=C3=A5 loader hjemme web serveren min sidene p=C3=A5 1-3 sekunder.</p>=
<p>Hele sulamitten er Fri Programvare og =C3=85pen Kildekode. Jeg er selvf=
=C3=B8lgelig disponibel til intervju, ol.</p><p>Ha en fin dag :)</p><p>Thom=
as Hansen</p><pre style=3D""opacity:.3;"">--=20
Got Privacy ...?
<a target=3D'_blank' href=3D'https://gaiasoul.com/got-privacy'><strong>gaia=
soul.com</strong>/got-privacy</a></pre></div></body></html>
--=-ZbhtG5e/mW81PPRARfJYbw==--
--=-70CJRx+kzBN9hWTAOosSdQ==
Content-Type: application/pgp-signature; name=signature.pgp
Content-Disposition: attachment; filename=signature.pgp
Content-Transfer-Encoding: 7bit

-----BEGIN PGP MESSAGE-----
Version: BCPG C# v1.8.1.0

owJ4nAEfAuD9iQIcBAEBCAAGBQJZnvbLAAoJEK7Z0E9DviqtVWkP/jtHd6ChIasC
4mp40QawLKw5v9EiWjJHWeK+u3Z4dULUFucJ3t3Ge5WkaNQE112pDH3Fv6LzgivY
FEqo6YVIodmrBNBIN/6rS8wevrHOgz0bOKnmmN9VjOB0pIIZwmvgl5eEs2ruLIK6
F0gt8UEc9ezOn74k2cl2TmRbk3qrmXiCoyIDeYdUPB03l25cXHLmfjKzKxOS5tVB
kGFU2QpiHcR2P7ByPFuf/saIfd15SdfchiXwAKAqVVEm/EcF9EV6H2BthWkwYwm2
BgVW6+gYm6VL1QkX+NtOrW47rhHVTv0GHYagVeVB88nqSY+azNjxonuujwgZLzYk
qmJzFQ04sdeyMYF2w/GwP1OH5naspmx31zTqyF4Wy4CjYO8ZqsO3+5Mr3d9XOKCI
QcxBrESoic6waAVaJvZgDiV7WMe1saiS9IgzZoyV4CIYh0V4iAlbju942t0MuMdj
4Le4AOw7YRH4CyjeA+oYaRVRUrxWIlZvYG6+/IpLS3HEN3AYh0NKXfK1z5Alqoti
MKEdxHWB/K/+BMihaNz7eEpHQ4nO/u4v3kIT/+eW8m7Pb69zWcc3fOkbTbfHwJ/B
gVULq0UEz2OVBeWRtawr7BsWyXPeyfHve419lFICsoawpAKYcCmK5RhwtAlFBUBY
0DXlKRAdpsdU7Y9v5rvt3VBTfaKUAacUSsEK1Q==
=vgcR
-----END PGP MESSAGE-----

--=-70CJRx+kzBN9hWTAOosSdQ==--";
            var lambda = Common.Evaluate($"mime.parse:@\"{mimeMessage.Replace(@"""", @"""""")}\"");
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