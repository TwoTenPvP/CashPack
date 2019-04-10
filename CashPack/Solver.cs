using System;
using System.IO;
using System.Security.Cryptography;

namespace CashPack
{
    /// <summary>
    /// Class to solve CashPacks to plaintext payloads.
    /// </summary>
    public static class Solver
    {
        /// <summary>
        /// Solves a CashPack in a single go. Only recommended to be used for very easy CashPacks that takes no more than a few minutes.
        /// </summary>
        /// <param name="pack">The CashPack to solve.</param>
        /// <returns>The solved plaintext payload.</returns>
        public static byte[] Solve(CashPack pack)
        {
            byte[] currentKey = new byte[pack.Key.Length];
            Array.Copy(pack.Key, 0, currentKey, 0, pack.Key.Length);
            
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.IV = pack.IV;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                
                while (true)
                {
                    // Set key
                    aes.Key = currentKey;     
                    
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        using (ICryptoTransform decryptor = aes.CreateDecryptor(currentKey, pack.IV))
                        {
                            try
                            {
                                CryptoStream stream = new CryptoStream(outputStream, decryptor, CryptoStreamMode.Write);

                                stream.Write(pack.Payload, 0, pack.Payload.Length);

                                stream.FlushFinalBlock();

                                // IMPORTANT: Use HMAC instead of Checksum
                                // This prevents you to do attacks where you can correlate known payloads
                                // And a CashPack without ever solving it.
                                using (HMACSHA512 sha = new HMACSHA512(currentKey))
                                {
                                    outputStream.Position = 0;

                                    byte[] hash = sha.ComputeHash(outputStream);

                                    bool failed = false;

                                    for (int i = 0; i < pack.Hash.Length; i++)
                                    {
                                        if (pack.Hash[i] != hash[i])
                                        {
                                            failed = true;
                                            break;
                                        }
                                    }

                                    if (!failed)
                                    {
                                        return outputStream.ToArray();
                                    }
                                }
                            }
                            catch (CryptographicException)
                            {
                                // Thrown when padding gets fucked up by invalid key (Can cause Stackoverflow in Mono)
                            }
                        }
                    }

                    // Increment key
                    ulong keyAsNumber = BitHelper.KeyToNumber(currentKey, pack.BitDifficulty);
                    BitHelper.SetLastBitNumberBytes(currentKey, keyAsNumber + 1, pack.BitDifficulty);
                }
            }
        }

        /// <summary>
        /// Attempts to solve the provided ProgressPack in a specified amount of iterations.
        /// </summary>
        /// <param name="iterations">The max amount of iterations to try for.</param>
        /// <param name="pack">The progress pack to continue solving.</param>
        /// <param name="result">The solved payload.</param>
        /// <returns>Whether or not solving was successful.</returns>
        public static bool SolveIncremental(uint iterations, ProgressPack pack, out byte[] result)
        {
            byte[] currentKey = new byte[pack.Key.Length];
            Array.Copy(pack.Key, 0, currentKey, 0, pack.Key.Length);
            
            BitHelper.SetLastBitNumberBytes(currentKey, pack.WorkingIteration, pack.BitDifficulty);
            
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.IV = pack.IV;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                for (int i = 0; i < iterations; i++)
                {
                    // Set key
                    aes.Key = currentKey;

                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        using (ICryptoTransform decryptor = aes.CreateDecryptor(currentKey, pack.IV))
                        {
                            try
                            {
                                CryptoStream stream = new CryptoStream(outputStream, decryptor, CryptoStreamMode.Write);

                                stream.Write(pack.Payload, 0, pack.Payload.Length);

                                stream.FlushFinalBlock();

                                // IMPORTANT: Use HMAC instead of Checksum
                                // This prevents you to do attacks where you can correlate known payloads
                                // And a CashPack without ever solving it.
                                using (HMACSHA512 sha = new HMACSHA512(currentKey))
                                {
                                    outputStream.Position = 0;

                                    byte[] hash = sha.ComputeHash(outputStream);

                                    bool failed = false;

                                    for (int x = 0; x < pack.Hash.Length; x++)
                                    {
                                        if (pack.Hash[x] != hash[x])
                                        {
                                            failed = true;
                                            break;
                                        }
                                    }

                                    if (!failed)
                                    {
                                        result = outputStream.ToArray();
                                        pack.WorkingIteration = BitHelper.KeyToNumber(currentKey, pack.BitDifficulty);
                                        return true;
                                    }
                                }
                            }
                            catch (CryptographicException)
                            {
                                // Thrown when padding gets fucked up by invalid key (Can cause Stackoverflow in Mono)
                            }
                        }
                    }

                    // Increment key
                    ulong keyAsNumber = BitHelper.KeyToNumber(currentKey, pack.BitDifficulty);
                    BitHelper.SetLastBitNumberBytes(currentKey, keyAsNumber + 1, pack.BitDifficulty);
                }

                result = null;
                pack.WorkingIteration = BitHelper.KeyToNumber(currentKey, pack.BitDifficulty);
                return false;
            }
        }
    }
}