using System.IO;
using System.Security.Cryptography;

namespace CashPack
{
    /// <summary>
    /// Class to pack payloads into CashPacks.
    /// </summary>
    public static class Packer
    {
        /// <summary>
        /// Packs a payload into a CashPack.
        /// </summary>
        /// <param name="payload">The plaintext payload to CashPack.</param>
        /// <param name="bitDifficulty">The amount of bits to omit from the packed key. Calculator class can calculate this.</param>
        /// <returns>The packed CashPack ready to distribute. It only contains the encrypted payload and reduced key.</returns>
        public static CashPack Pack(byte[] payload, ushort bitDifficulty)
        {
            CashPack pack = new CashPack()
            {
                BitDifficulty = bitDifficulty
            };

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                aes.GenerateKey();
                aes.GenerateIV();

                pack.IV = aes.IV;

                // IMPORTANT: Use HMAC instead of Checksum
                // This prevents you to do attacks where you can correlate known payloads
                // And a CashPack without ever solving it.
                using (HMACSHA512 sha = new HMACSHA512(aes.Key))
                {
                    pack.Hash = sha.ComputeHash(payload);
                }

                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        using (CryptoStream stream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
                        {
                            stream.Write(payload, 0, payload.Length);

                            stream.FlushFinalBlock();

                            pack.Payload = outputStream.ToArray();

                            // Reduce key
                            byte[] reducedKey = aes.Key;
                            BitHelper.ReduceKey(reducedKey, bitDifficulty);
                            
                            
                            pack.Key = reducedKey;
                        }
                    }
                }

                return pack;
            }
        }
    }
}