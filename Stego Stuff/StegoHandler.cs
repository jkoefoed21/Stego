using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;
using encryption;


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

        public static byte[] storeStuffhere = null;

        /// <summary>
        /// The Length of the salt.
        /// </summary>
        public static readonly int SALT_LENGTH = 32; //bytes not bits

        public static readonly int START_LENGTH = BLOCK_LENGTH + HASH_LENGTH + SALT_LENGTH;

        public static String Filename = "C:\\Users\\Jack Koefoed\\Pictures\\PSAT2.png";
        public static String Filename2 = "C:\\Users\\Jack Koefoed\\Pictures\\PSAT3.png";

        //public static int[] img = { 0xF0FFF0 };

        /// <summary>
        /// The Number of iterations used for the PBKDF2. This slows the program down a lot
        /// but it is good that it does, because it makes the hash, iv cryptographically secure.
        /// </summary>
        public static readonly int NUM_ITERATIONS = 4096; //slows the algorithm down by about a second...for security though

        //IF STUFF AINT WORKING--IT IS PROLLY BECAUSE OF A JPEG
        public static void Main(String[] args)
        {
            Image b1 = new Bitmap(800, 600);
            Image b2 = new Bitmap(800, 600);
            b1.Save(Filename);
            b2.Save(Filename2);
            b1.Dispose();
            b2.Dispose();
            //modifyPixel(2, img, 0);
            //printIntAsBits(513);
            implantMain("a", "I like Ike");
            Console.ReadKey();
            extractMain("a");
            Console.ReadKey();
        }

        public static void implantMain(String password, String message)
        {
            if(Filename.Contains(".jpg")||Filename.Contains(".jpeg"))
            {
                throw new ArgumentException("NO JPEGS PLEASE DEAR GOD");
            }
            Bitmap b = new Bitmap(Filename); //throws FileNotFoundException
            int[] image = imageToIntArray(b);
            byte[] messBytes = stringToByteArrayWithEOF(message);
            printByteArray(messBytes);
            Rfc2898DeriveBytes keyDeriver = new Rfc2898DeriveBytes(password, SALT_LENGTH, NUM_ITERATIONS); //creates random salt for a key
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider(); //this is cryptographically secure IV
            byte[] initVect = new byte[BLOCK_LENGTH];
            rng.GetBytes(initVect);
            byte[] salt = keyDeriver.Salt;
            byte[] key = keyDeriver.GetBytes(BLOCK_LENGTH); //gets a key from the password
            byte[] keyHash = getHash(key, salt);//64 bytes--uses same salt as key deriver...this shouldn't be an issue.
            BitMatrix[] keySched = AES.getKeySchedule(key);
            storeStuffhere = salt;
            printByteArray(keyHash);
            printByteArray(initVect);
            printByteArray(salt);
            Console.WriteLine();

            implantBlock(b, 0, keyHash);
            implantBlock(b, keyHash.Length, initVect);
            //Console.WriteLine("WRITE SALT START");
            implantBlock(b, keyHash.Length+initVect.Length, salt);
            //Console.WriteLine("WRITE SALT END");
            implantMessage(b, keySched, messBytes, initVect);
            //implantBlock(b, keyHash.Length + initVect.Length + salt.Length, messBytes);

            //setImageFromIntArray(image, b); //SEE THIS
            b.Save(Filename2, ImageFormat.Png);
        }

        public static void extractMain(String password)//int[] image)
        {
            Bitmap b =  new Bitmap(Filename2); //throws FileNotFoundException
            int[] image = imageToIntArray(b);
            byte[] initVect = new byte[BLOCK_LENGTH];
            byte[] salt = new byte[SALT_LENGTH];
            byte[] readHash = new byte[HASH_LENGTH];
            byte[] messBytes = new byte[12]; //hard coded
            
            extractBlock(b, 0, readHash);
            extractBlock(b, readHash.Length, initVect);
            //Console.WriteLine("READ SALT");
            extractBlock(b, readHash.Length + initVect.Length, salt);
            //Console.WriteLine("END READ SALT");
           /* for (int ii=0; ii<salt.Length; ii++)
            {
                Console.Write(ii + " ");
                Console.Write(salt[ii]+" ");
                Console.Write(storeStuffhere[ii]);
                if(salt[ii]!=storeStuffhere[ii])
                {
                    Console.Write("FAIL");
                    Console.ReadKey();
                }
                Console.WriteLine();
                //throw new Exception("Whack");
            }*/
            Rfc2898DeriveBytes keyDeriver = new Rfc2898DeriveBytes(password, salt, NUM_ITERATIONS);
            byte[] key = keyDeriver.GetBytes(BLOCK_LENGTH);
            BitMatrix[] keySched = AES.getKeySchedule(key);
            byte[] compHash = getHash(key, salt);
            extractMessage(b, keySched, messBytes, initVect);
            //extractBlock(b, keyHash.Length + initVect.Length + salt.Length, messBytes);
            printByteArray(readHash);
            printByteArray(initVect);
            printByteArray(salt);
            printByteArray(messBytes);
        }

        public static void implantBlock(Bitmap b, int start, byte[] array)
        {
            for (int ii = 0; ii < array.Length * 8; ii++)
            {
                modifyPixel(start*8+ii, b, (byte)(array[ii / 8] >> 7 - (ii % 8)) % 2);
            }
        }

        public static void extractBlock(Bitmap b, int start, byte[] array)
        {
            for (int ii = start; ii < start+array.Length; ii++)
            {
                byte nextNum = 0;
                for (int jj = 0; jj < 8; jj++)
                {
                    nextNum = (byte)(nextNum << 1);
                    nextNum += readPixel(ii * 8 + jj, b);
                }
                array[ii-start] = nextNum;
            }
        }

        public static void implantMessage(Bitmap b, BitMatrix[] keySched, byte[] message, byte[] initVect) //add start place param
        {
            BitMatrix iv = new BitMatrix(AES.GF_TABLE, AES.SUB_TABLE, initVect, 0);
            //Console.WriteLine(keySched[6].ToString());
            for (int ii = 0; ii < Math.Ceiling(message.Length / 2.0); ii++)
            {
                AES.encryptSingle(keySched, iv); //operates as a stream cipher--XTS mode I think? Who knows.
                for (int jj = 0; jj < BLOCK_LENGTH; jj++)
                {
                    modifyPixel(START_LENGTH*8 + 256 * (16 * ii + jj) + initVect[jj], b, getBitFromByte(message[ii*2+jj/8], jj));
                }
            }
        }

        public static void extractMessage(Bitmap b, BitMatrix[] keySched, byte[] message, byte[] initVect)
        {
            BitMatrix iv = new BitMatrix(AES.GF_TABLE, AES.SUB_TABLE, initVect, 0);
            //Console.WriteLine(keySched[6].ToString());
            for (int ii = 0; ii < Math.Ceiling(message.Length / 2.0); ii++)
            {
                //printByteArray(message);
                AES.encryptSingle(keySched, iv);
                for (int jj = 0; jj < BLOCK_LENGTH/2; jj++)
                {
                    if(readPixel(START_LENGTH*8 + 256 * (16 * ii + jj)+ initVect[jj], b)==1)
                    {
                        message[2 * ii]=stickBitInByte(message[2*ii], jj);

                    }
                    //Console.WriteLine(message[2 * ii]);
                }
                for (int jj = BLOCK_LENGTH/2; jj < BLOCK_LENGTH; jj++)
                {
                    if (readPixel(START_LENGTH*8 + 256 * (16 * ii + jj)+ initVect[jj], b) == 1)
                    {
                        message[2 * ii+1]=stickBitInByte(message[2 * ii+1], jj-8);
                    }
                }
            }
        }

        public static void modifyPixel(int valueNum, Bitmap b, int toEncode) //toEncode must be either 0 or 1--could be bool but still type conversion
        {
            int pixelNum = valueNum / 4;
            int pixVal = b.GetPixel(pixelNum % b.Height, pixelNum / b.Height).ToArgb();
            toEncode = toEncode << (8 * (3 - (valueNum % 4)));
            int cleaning = 1 << 8 * ((3 - (valueNum % 4)));
            pixVal = (pixVal & (Int32.MaxValue - cleaning)) | toEncode;
            //Console.WriteLine("{0:X}", pixVal);
            b.SetPixel(pixelNum % b.Height, pixelNum / b.Height, Color.FromArgb(pixVal)); //fix this
        }
        /*
        public static void modifyPixel(int valueNum, int[] img, int toEncode) //toEncode must be either 0 or 1--could be bool but still type conversion
        {
            if (toEncode!=0&&toEncode!=1)
            {
                throw new ArgumentException();
            }
            int pixelNum = valueNum / 4;
            int pixVal = img[pixelNum];
            //Console.Write("pixVal=");
            //Console.Write("{0:X}", pixVal);
            toEncode = toEncode << (8*(3 - (valueNum % 4)));
            int cleaning = 1 << 8*((3 - (valueNum % 4)));
            //Console.Write("|");
           // Console.Write("{0:X}", cleaning);
            //Console.Write("|");
           // Console.Write("{0:X}", toEncode);
            pixVal = (pixVal & (Int32.MaxValue - cleaning)) | toEncode;
            Console.Write("|");
            Console.WriteLine("{0:X}", pixVal);
            img[pixelNum] = pixVal;
            //Console.ReadKey();
        }*/

        //check the types thru here
        public static byte readPixel(int valueNum, Bitmap b) //toEncode must be either 0 or 1--could be bool but still type conversion
        {
            int pixelNum = valueNum / 4;
            int pixVal = b.GetPixel(pixelNum % b.Height, pixelNum / b.Height).ToArgb();
            //Console.WriteLine("{0:X}", pixVal);
            //printIntAsBits((ulong) pixVal);
            int returnValue = ((pixVal >> (8 * (3 - valueNum % 4))));
            uint UretVal = intToUInt(returnValue);
            UretVal = UretVal % 2;
            //Console.WriteLine("readBit=" + returnValue);
            return (byte) UretVal;
        }

       /* public static byte readPixel(int valueNum, int[] img) //toEncode must be either 0 or 1--could be bool but still type conversion
        {
            int pixelNum = valueNum / 4;
            int pixVal = img[pixelNum];
            //Console.Write("pixVal=");
            //Console.WriteLine("{0:X}", pixVal);
            //printIntAsBits(pixVal);
            int returnValue = ((pixVal >> (8 * (3 - valueNum % 4))));
            returnValue = Math.Abs(returnValue % 2);
            //Console.WriteLine("readBit=" + returnValue);
            return (byte)returnValue;
        }*/

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

        public static void printByteArray(byte[] byteArray)
        {
            for (int ii=0; ii<byteArray.Length; ii++)
            {
                Console.Write("{0:X}", byteArray[ii]);
                Console.Write(" ");
            }
            Console.WriteLine();
        }

        public static void printIntAsBits(int toPrint)
        {
            for (int ii=0; ii<32; ii++)
            {
                Console.Write((toPrint >> 31 - ii) % 2);
            }
        }
        public static void printIntAsBits(long toPrint)
        {
            for (int ii = 0; ii < 32; ii++)
            {
                Console.Write((toPrint >> 31 - ii) % 2);
                Console.Write(" | ");
            }
        }
        public static void printIntAsBits(ulong toPrint)
        {
            for (int ii = 0; ii < 32; ii++)
            {
                Console.Write((toPrint >> 31 - ii) % 2);
                Console.Write(" | ");
            }
        }

        public static int getBitFromByte(byte b, int index) //this is indexed where 0 is MSB, 7 is LSB
        {
            return (b >> (7 - index)) % 2;
        }

        public static byte stickBitInByte(byte b, int index) //can't add a 0 to a byte--would just be do nothing
        {
            byte add = 1;
            b += (byte) (add << (7 - (byte) index));
            return b;
        }

        public static int[] imageToIntArray(Bitmap b)
        {
            int[] output = new int[b.Height*b.Width];
            for (int ii=0; ii<b.Height*b.Width; ii++)
            {
                output[ii] = b.GetPixel(ii % b.Width, ii / b.Width).ToArgb();
            }
            return output;
        }
        public static void setImageFromIntArray(int[] intArr, Bitmap b)
        {
            for (int ii = 0; ii < b.Height * b.Width; ii++)
            {
                b.SetPixel(ii % b.Width, ii / b.Width, Color.FromArgb(intArr[ii]));
            }
        }
        public static byte[] stringToByteArrayWithEOF(string message)
        {
            char[] messChars=message.ToCharArray(); //base on mod 2 
            byte[] messBytes = new byte[messChars.Length+2];
            for (int ii=0; ii<messChars.Length; ii++)
            {
                messBytes[ii] = (byte)messChars[ii];
            }
            messBytes[messChars.Length] = 0x04;
            //messBytes[messChars.Length + 1] = 0x00;
            messBytes[messChars.Length + 1] = 0x04;
            return messBytes;
        }

        public static uint intToUInt(int toConvert)
        {
            unchecked
            {
                return (uint)toConvert;
            }
        }
    }
}
