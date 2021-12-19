using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RSA
{
    class StaticMethods
    {
        public static List<BigInteger> GenerateMessageNumber(string message, int blockLenghtInput)
        {
            List<BigInteger> returnMessage = new List<BigInteger>();
            int tmpMessageLenght = message.Length;
            int blockCount = 0;
            int blockLenght = blockLenghtInput / 8;

            if(message.Length < blockLenght)
            {
                blockCount = 1;
            }
            else
            {
                blockCount = message.Length / blockLenght;
                if (message.Length % blockLenght != 0)
                {
                    blockCount += 1;
                }
            }

            int startIndex = 0;

            for (int i = 0; i < blockCount; i++)
            {
                if (tmpMessageLenght > blockLenght)
                {
                    byte[] partOfMessage = Encoding.ASCII.GetBytes(message.Substring(startIndex, blockLenght));
                    startIndex += blockLenght;
                    returnMessage.Add(new BigInteger(partOfMessage));
                    tmpMessageLenght -= partOfMessage.Length;
                }
                else
                {
                    byte[] partOfMessage = Encoding.ASCII.GetBytes(message.Substring(startIndex, tmpMessageLenght));
                    returnMessage.Add(new BigInteger(partOfMessage));
                }
                
            }

            return returnMessage;
        }

        /*
        public static BigInteger GCD(BigInteger p, BigInteger q)
        {
            BigInteger r = 1;

            if (p > q)
            {
                BigInteger tmp = p;
                p = q;
                q = tmp;
            }

            while (r != 0)
            {
                r = q % p;

                if (r == 0)
                {
                    break;
                }

                q = p;
                p = r;
            }

            return p;
        }
        */

        public static string ReturnMessageFromNumber(BigInteger byteArrayNumber)
        {
            byte[] decryptedByteArray = byteArrayNumber.ToByteArray();
            string decryptedText = "";
            for (int i = 0; i < decryptedByteArray.Length; i++)
            {
                decryptedText += Convert.ToString(Convert.ToChar(decryptedByteArray[i]));
            }

            return decryptedText;
        }

        public static BigInteger extendedGCD(BigInteger a, BigInteger n)
        {
            BigInteger i = n;
            BigInteger v = 0;
            BigInteger d = 1;

            while (a > 0)
            {
                BigInteger t = i / a;
                BigInteger x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }

            v %= n;

            if (v < 0)
            {
                v = (v + n) % n;
            }

            return v;
        }
    }
}
