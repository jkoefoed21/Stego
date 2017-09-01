Created by Jack Koefoed

This program uses LSB steganography to encode a file into a PNG image. Can encode within JPEGs, but must output as PNG.

Completely randomizes LSBs in image to ensure that someone with the original image cannot tell which bits have been modified.

Uses 128 bit AES stream cipher to disperse bits within image.

Uses 128 bit AES to encrypt data before inserting into image.
