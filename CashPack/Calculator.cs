using System;
using System.Diagnostics;
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

        /// <summary>
        /// Calculates the time in seconds for each bit difficulty.
        /// </summary>
        /// <returns>The time in seconds it takes to unpack the payload on average for the bit values up to the bits parameter.</returns>
        /// <param name="payload">The payload to calculate for.</param>
        /// <param name="bits">The amount of bits to calculate up to.</param>
        public static ulong[] CalculateTimeForDifficultyLookup(byte[] payload, byte bits = 64)
        {
            ulong decryptsPerSecond = (ulong)GetSolveAttemptsPerSecond(payload, EstimationAccuracy.Normal);

            ulong[] results = new ulong[64];

            for (ushort i = 0; i < results.Length; i++)
            {
                results[i] = ((ulong)Math.Pow(2, i) / decryptsPerSecond);
            }

            return results;
        }

        private static double GetSolveAttemptsPerSecond(byte[] payload, EstimationAccuracy accuracy)
        {
            CashPack pack = Packer.Pack(payload, (ushort)(payload.Length * 8));

            Stopwatch watch = new Stopwatch();

            int decrypts = 0;

            watch.Start();

            byte[] currentKey = new byte[pack.Key.Length];
            Array.Copy(pack.Key, 0, currentKey, 0, pack.Key.Length);

            byte[] solveWorkspace = new byte[pack.Payload.Length];

            while (watch.ElapsedMilliseconds < ((uint)accuracy * 1000))
            {

                for (int i = 0; i < pack.Payload.Length; i++)
                {
                    solveWorkspace[i] = (byte)(pack.Payload[i] ^ currentKey[i]);
                }

                // IMPORTANT: Use HMAC instead of Checksum
                // This prevents you to do attacks where you can correlate known payloads
                // And a CashPack without ever solving it.
                using (HMACSHA512 sha = new HMACSHA512(currentKey))
                {
                    byte[] hash = sha.ComputeHash(solveWorkspace);

                    for (int i = 0; i < pack.Hash.Length; i++)
                    {
                        if (pack.Hash[i] != hash[i])
                        {
                            break;
                        }
                    }
                }

                // Increment key
                ulong keyAsNumber = BitHelper.KeyToNumber(currentKey, pack.BitDifficulty);
                BitHelper.SetLastBitNumberBytes(currentKey, keyAsNumber + 1, pack.BitDifficulty);

                decrypts++;
            }

            watch.Stop();

            return decrypts / (watch.ElapsedMilliseconds / 1000d);
        }
    }
}