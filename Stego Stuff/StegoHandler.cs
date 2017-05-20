using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;
using encryption;
using System.IO;


namespace Stego_Stuff
{
    class StegoHandler
    {
        //this can fit a byte of message into 2048 bytes or 512 px, because a bit takes 256 bytes of data. At 600x800, that gives 937 bytes of info
        //however, some space is taken up by the headers, although they at least are in sequential bytes--basically nothing.
        //when I actually encrypt the underlying data, I will need a protocol to deal with those headers.
        //on 600x800 holds exactly 931 bytes of data w/ 1024-64-16 headers although theres an exception on read that happens
        //will need to add 3 different modes--sequential encrypted, dispersed encrypted with all sequential headers, and dispersed encrypted with non-sequential headers.
        //for the non-sequential headers, I will prolly use 64-64-16. 
        //could add ways to shrink the non-sequentialism by factors of 2, such as 0-128 or 0-64, but that is later
        //also need to build frontside UI


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
        public static readonly int SALT_LENGTH = 1024; //bytes not bits

        /// <summary>
        /// This saves so much time and space in the code.
        /// </summary>
        public static readonly int START_LENGTH = BLOCK_LENGTH + HASH_LENGTH + SALT_LENGTH;

        public static readonly String Filename = "C:\\Users\\Jack Koefoed\\Pictures\\test1.png";
        public static readonly String Filename2 = "C:\\Users\\Jack Koefoed\\Pictures\\test2.png";
        public static readonly String MESSAGEFILE = "C:\\Users\\Jack Koefoed\\Pictures\\message.txt";

        /// <summary>
        /// The Number of iterations used for the PBKDF2. This slows the program down a lot
        /// but it is good that it does, because it makes the hash, iv cryptographically secure.
        /// </summary>
        public static readonly int NUM_ITERATIONS = 4096; //slows the algorithm down by about a second...for security though

        //IF STUFF AINT WORKING--IT IS PROLLY BECAUSE OF A JPEG
        public static void Main(String[] args)
        {
            /*Image b1 = new Bitmap(1920, 1080);
            Image b2 = new Bitmap(1920, 1080);
            b1.Save(Filename);
            b2.Save(Filename2);
            b1.Dispose();
            b2.Dispose();*/
            implantMain("a");
            Console.ReadKey();
            extractMain("a");
            Console.ReadKey();
        }

        public static void implantMain(String password)
        {
            if(Filename2.Contains(".jpg")||Filename2.Contains(".jpeg"))
            {
                throw new ArgumentException("NO JPEGS PLEASE DEAR GOD");
            }

            Bitmap b = new Bitmap(Filename); //throws FileNotFoundException
            byte[] readBytes = File.ReadAllBytes(MESSAGEFILE); //this throws IO if larger than 2GB--should really make a stream
            byte[] messBytes = addEOF(readBytes);

            if (messBytes.Length>(b.Height*b.Width-2 * START_LENGTH) / 512)
            {
               //throw new ArgumentException("Message is too long");
            }

            Rfc2898DeriveBytes keyDeriver = new Rfc2898DeriveBytes(password, SALT_LENGTH, NUM_ITERATIONS); //creates random salt for a key
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider(); //this is cryptographically secure IV
            byte[] initVect = new byte[BLOCK_LENGTH];
            rng.GetBytes(initVect);
            byte[] salt = keyDeriver.Salt;
            byte[] key = keyDeriver.GetBytes(BLOCK_LENGTH); //gets a key from the password
            byte[] keyHash = getHash(key, salt);//64 bytes--uses same salt as key deriver...this shouldn't be an issue.
            BitMatrix[] keySched = AES.getKeySchedule(key);

            //printByteArray(keyHash);
            //printByteArray(initVect);
            //printByteArray(salt);

            implantBlock(b, 0, keyHash);
            implantBlock(b, keyHash.Length, initVect);
            implantBlock(b, keyHash.Length+initVect.Length, salt);
            implantMessage(b, keySched, messBytes, initVect);
            b.Save(Filename2, ImageFormat.Png);
        }

        public static void extractMain(String password)//int[] image)
        {

            Bitmap b =  new Bitmap(Filename2); //throws FileNotFoundException
            if(b.Height*b.Width<START_LENGTH*2)
            {
                //throw new ArgumentException("File is too small to read");
            }
            byte[] initVect = new byte[BLOCK_LENGTH];
            byte[] salt = new byte[SALT_LENGTH];
            byte[] readHash = new byte[HASH_LENGTH];
            Queue<byte> messQueue = new Queue<byte>();
            
            extractBlock(b, 0, readHash);
            extractBlock(b, readHash.Length, initVect);
            extractBlock(b, readHash.Length + initVect.Length, salt);

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

            if(!readHash.SequenceEqual(compHash))
            {
                throw new ArgumentException("Wrong Password or not a Stego File");
            }

            extractMessage(b, keySched, messQueue, initVect);
            Console.WriteLine(messQueue.Count);
            byte[] messBytes = messQueue.ToArray();
            byte[] finalMessBytes = new byte[messBytes.Length - 7];
            Array.Copy(messBytes, finalMessBytes, finalMessBytes.Length);

            //extractBlock(b, keyHash.Length + initVect.Length + salt.Length, messBytes);
            //printByteArray(readHash);
            //printByteArray(initVect); 
            //printByteArray(salt);
            //printByteArray(finalMessBytes);

            String message = Encoding.UTF8.GetString(finalMessBytes, 0, finalMessBytes.Length);
            Console.WriteLine(message);
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
            for (int ii = 0; ii < Math.Floor((double) message.Length / 2.0); ii++)
            {
                AES.encryptSingle(keySched, iv); //operates as a stream cipher--XTS mode I think? Who knows.
                for (int jj = 0; jj < BLOCK_LENGTH; jj++)
                {
                    modifyPixel(START_LENGTH*8 + /*256 **/  (16 * ii + jj) /*+ initVect[jj]*/, b, getBitFromByte(message[ii*2+jj/8], jj%8));
                }
            }
            if (message.Length%2==1)
            {
                AES.encryptSingle(keySched, iv); //operates as a stream cipher--XTS mode I think? Who knows.
                int ii=(int) Math.Floor((double)message.Length / 2.0);
                for (int jj = 0; jj < BLOCK_LENGTH/2; jj++)
                {
                    modifyPixel(START_LENGTH * 8 + /*256 **/ (16 * ii + jj) /*+ initVect[jj]*/, b, getBitFromByte(message[ii * 2 + jj / 8], jj % 8));
                }
            }
        }

        public static void extractMessage(Bitmap b, BitMatrix[] keySched, Queue<byte> message, byte[] initVect)
        {
            int endCount = 0;
            BitMatrix iv = new BitMatrix(AES.GF_TABLE, AES.SUB_TABLE, initVect, 0);
            //Console.WriteLine(keySched[6].ToString());
            Console.WriteLine(b.Height * b.Width / 1024);
            for (int ii = 0; ii < b.Height*b.Width/1024; ii++)//MAGIC NUMBER--because 2 bytes of message takes up 1024 px
            {
                //printByteArray(message);
                AES.encryptSingle(keySched, iv);
                byte newbyte1 = 0;
                for (int jj = 0; jj < BLOCK_LENGTH/2; jj++)
                {
                    if(readPixel(START_LENGTH*8 + /*256 **/ (16 * ii + jj) /*+ initVect[jj]*/, b)==1)//256 is there to provide room for stream cipher
                    {
                        newbyte1 =stickBitInByte(newbyte1, jj);
                    }
                    //Console.WriteLine(message[2 * ii]);
                }
                byte newbyte2 = 0;
                for (int jj = BLOCK_LENGTH/2; jj < BLOCK_LENGTH; jj++)
                {
                    if (readPixel(START_LENGTH*8 + /*256 **/ (16 * ii + jj) /*+ initVect[jj]*/, b) == 1)
                    {
                        newbyte2=stickBitInByte(newbyte2, jj-8);
                    }
                }
                //this is all EOM stuff in here--its bloody magic
                if (endCount>2)
                {
                    if (newbyte1==4)
                    {
                        return;
                    }
                    else if (newbyte1==0)
                    {
                        if (newbyte2==4)
                        {
                            message.Enqueue(newbyte1);
                            return;
                        }
                        else if (newbyte2!=0)
                        {
                            endCount = 0;
                        }
                    }
                    else
                    {
                        endCount = 0;
                    }
                }

                message.Enqueue(newbyte1);
                message.Enqueue(newbyte2);
                if ((newbyte1==0&&newbyte2==0))
                {
                    endCount++;
                }
                else
                {
                    endCount = 0;
                }
            }
        }

        public static void modifyPixel(int valueNum, Bitmap b, int toEncode) //toEncode must be either 0 or 1--could be bool but still type conversion
        {
            int pixelNum = valueNum / 4;
            int pixVal = b.GetPixel(pixelNum % b.Width, pixelNum / b.Width).ToArgb();
            Console.Write("{0:X}", pixVal);
            Console.Write("|");
            toEncode = toEncode << (8 * (3 - (valueNum % 4)));
            int cleaning = 1 << 8 * ((3 - (valueNum % 4)));
            pixVal = (pixVal & (Int32.MaxValue - cleaning)) | toEncode;
            Console.WriteLine("{0:X}", pixVal);
            Console.ReadKey();
            b.SetPixel(pixelNum % b.Width, pixelNum / b.Width, Color.FromArgb(pixVal)); //fix this
        }

        //check the types thru here
        public static byte readPixel(int valueNum, Bitmap b) //toEncode must be either 0 or 1--could be bool but still type conversion
        {
            int pixelNum = valueNum / 4;
            int pixVal = b.GetPixel(pixelNum % b.Width, pixelNum / b.Width).ToArgb();
            //Console.WriteLine("{0:X}", pixVal);
            //printIntAsBits((ulong) pixVal);
            int returnValue = ((pixVal >> (8 * (3 - valueNum % 4))));
            uint UretVal = intToUInt(returnValue);
            UretVal = UretVal % 2;
            //Console.WriteLine("readBit=" + returnValue);
            return (byte) UretVal;
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
            byte[] messBytes = new byte[messChars.Length+8];
            for (int ii=0; ii<messChars.Length; ii++)
            {
                messBytes[ii] = (byte)messChars[ii];
            }
            //lay down 4s, then an EOF
            for (int ii=0; ii<7; ii++)//MAGIC
            {
                messBytes[messChars.Length + ii] = 0x00;
            }
            messBytes[messChars.Length + 7] = 0x04;
            return messBytes;
        }

        public static byte[] addEOF(byte[] message)
        {
            byte[] bytesWEOF = new byte[message.Length + 8];
            Array.Copy(message, bytesWEOF, message.Length);
            for (int ii = 0; ii < 7; ii++)//MAGIC
            {
                bytesWEOF[message.Length + ii] = 0x00;
            }
            bytesWEOF[message.Length + 7] = 0x04;
            return bytesWEOF;
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
