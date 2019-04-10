using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace CashPack
{
    /// <summary>
    /// Helper class to help calculate difficulty values.
    /// </summary>
    public static class Calculator
    {
        /// <summary>
        /// Gets the number of bits difficulty required to get an average solve time for a specific payload on the current hardware.
        /// </summary>
        /// <param name="payload">The payload to attempt on.</param>
        /// <param name="span">The average solve time.</param>
        /// <param name="accuracy">The accuracy to use to estimate the processor speed.</param>
        /// <param name="solveThreads">The amount of simultaneous solvers to account for.</param>
        /// <returns>The amount of bits required to be omitted.</returns>
        public static ushort GetBitsForPayloadDecodeTime(byte[] payload, TimeSpan span, EstimationAccuracy accuracy = EstimationAccuracy.Normal, uint solveThreads = 1)
        {
            double decryptsPerSecond = GetSolveAttemptsPerSecond(payload, accuracy) * solveThreads;
            
            double iterationsRequired = (decryptsPerSecond * span.TotalSeconds);

            ulong currentClosestDifference = ulong.MaxValue;
            
            for (ushort i = 0; i < ushort.MaxValue; i++)
            {
                ulong howManyIterationsWeAreOff = ((ulong)Math.Round(iterationsRequired) - (ulong)Math.Pow(2, i));

                if (howManyIterationsWeAreOff < currentClosestDifference)
                    currentClosestDifference = howManyIterationsWeAreOff;
                else
                {
                    // DONT use (i - 1) here, because we want to actually have one binary bit more, which doubles the complexity. Since the average solve time is in the middle.
                    return i;
                }
            }

            return ushort.MaxValue;
        }

        private static double GetSolveAttemptsPerSecond(byte[] payload, EstimationAccuracy accuracy)
        {
            CashPack pack = Packer.Pack(payload, 128);
            
            Stopwatch watch  = new Stopwatch();
            
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                // Random key and IV, we are only testing a fail
                aes.GenerateIV();
                aes.GenerateKey();
                
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                int decrypts = 0;
                
                watch.Start();
                
                while (watch.ElapsedMilliseconds < ((uint)accuracy * 1000))
                {
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                        {
                            try
                            {
                                CryptoStream stream = new CryptoStream(outputStream, decryptor, CryptoStreamMode.Write);

                                stream.Write(pack.Payload, 0, pack.Payload.Length);

                                stream.FlushFinalBlock();
                                
                                using (HMACSHA512 sha = new HMACSHA512(aes.Key))
                                {
                                    outputStream.Position = 0;

                                    sha.ComputeHash(outputStream);

                                    outputStream.ToArray();
                                }
                            }
                            catch (CryptographicException)
                            {
                                // Thrown when padding gets fucked up by invalid key (Can cause Stackoverflow in Mono)
                            }
                        }
                    }

                    decrypts++;
                }
                
                watch.Stop();

                return decrypts / (watch.ElapsedMilliseconds / 1000d);
            }
        }
    }
}