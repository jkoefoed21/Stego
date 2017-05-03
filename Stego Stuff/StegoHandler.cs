using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Drawing;


namespace Stego_Stuff
{
    class StegoHandler
    {
        /// <summary>
        /// The length of 128 bits in bytes
        /// </summary>
        public static readonly int BLOCK_LENGTH = 16; //bytes not bits

        /// <summary>
        /// The length of the password hash.
        /// </summary>
        public static readonly int HASH_LENGTH = 64; //bytes

        /// <summary>
        /// The Length of the salt.
        /// </summary>
        public static readonly int SALT_LENGTH = 128; //bytes not bits

        /// <summary>
        /// The Number of iterations used for the PBKDF2. This slows the program down a lot
        /// but it is good that it does, because it makes the hash, iv cryptographically secure.
        /// </summary>
        public static readonly int NUM_ITERATIONS = 4096; //slows the algorithm down by about a second...for security though

        public static void Main(String[] args)
        {

        }

        public static void implant()
        {
            Bitmap b = new Bitmap("FILENAME"); //throws FileNotFoundException
            Rfc2898DeriveBytes keyDeriver = new Rfc2898DeriveBytes("THIS IS A PASSWORD", SALT_LENGTH, NUM_ITERATIONS); //creates random salt for a key
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider(); //this is cryptographically secure IV
            byte[] initVect = new byte[BLOCK_LENGTH];
            rng.GetBytes(initVect);
            byte[] salt = keyDeriver.Salt;
            byte[] key = keyDeriver.GetBytes(BLOCK_LENGTH); //gets a key from the password
            byte[] keyHash = getHash(key, salt);//64 bytes--uses same salt as key deriver...this shouldn't be an issue.
            for (int ii=0; ii<keyHash.Length*8; ii++)
            {
                modifyPixel(ii, b, (byte)(keyHash[ii / 8] >> 7-(ii % 8)));
            }
            for (int ii=0; ii<initVect.Length*8; ii++) //this is all fucked up
            {
                modifyPixel(ii+keyHash.Length*8, b, (byte)(initVect[ii / 8] >> 7 - (ii % 8)));
            }
            for (int ii=0;ii<salt.Length; ii++) 
            {
                modifyPixel(ii+ (keyHash.Length + initVect.Length) * 8, b, (byte)(salt[ii / 8] >> 7 - (ii % 8)));
            }
        }

        public static void modifyPixel(int valueNum, Bitmap b, int toEncode) //toEncode must be either 0 or 1 
        {
            int pixelNum = valueNum / 4;
            int pixVal = b.GetPixel(pixelNum % b.Height, pixelNum / b.Height).ToArgb();
            toEncode = toEncode << 3-(valueNum % 4);
            pixVal = (pixVal & (Int32.MaxValue - toEncode)) | toEncode;
            b.SetPixel(pixelNum % b.Height, pixelNum / b.Height, Color.FromArgb(pixVal));
        }

        /// <summary>
        /// Creates the Hash of the key.
        /// </summary>
        /// <param name="key"> The key to be hashed. </param>
        /// <param name="salt"> The salt hashed with the key </param>
        /// <returns> The hash of the key. </returns>
        public static byte[] getHash(byte[] key, byte[] salt)
        {
            Rfc2898DeriveBytes rdb = new Rfc2898DeriveBytes(key, salt, NUM_ITERATIONS); //The hash is PBKDF2 on the key
            return rdb.GetBytes(HASH_LENGTH);                                           //with the same salt as before.
        }

        public static String getPassword()
        {
            return null;
        }

        public static byte[] getKeyFromPassword(string password)
        {
            return null;
        }

    }
}
