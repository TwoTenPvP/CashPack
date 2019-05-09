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
                BitDifficulty = bitDifficulty,
            };

            // Alloc key
            byte[] fullKey = new byte[payload.Length];

            // Create key
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(fullKey);
            }

            // HMAC Key
            using (HMACSHA512 sha = new HMACSHA512(fullKey))
            {
                pack.Hash = sha.ComputeHash(payload);
            }

            // Alloc payload
            pack.Payload = new byte[payload.Length];

            // Encrypt payload
            for (int i = 0; i < pack.Payload.Length; i++)
            {
                pack.Payload[i] = (byte)(payload[i] ^ fullKey[i]);
            }

            // Reduce key
            byte[] reducedKey = fullKey;
            BitHelper.ReduceKey(reducedKey, bitDifficulty);

            pack.Key = reducedKey;

            return pack;
        }
    }
}