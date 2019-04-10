namespace CashPack
{
    /// <summary>
    /// A class representing the initial values required to start solving a CashPacked payload.
    /// </summary>
    public class CashPack
    {
        /// <summary>
        /// The encrypted payload.
        /// </summary>
        public byte[] Payload { get; internal set; }
        /// <summary>
        /// The IV used to encrypt the payload.
        /// </summary>
        public byte[] IV { get; internal set; }
        /// <summary>
        /// The known parts of the key. Should end with (BitDifficulty) amount of zeros.
        /// </summary>
        public byte[] Key { get; internal set; }
        /// <summary>
        /// The SHA512 hash of the decrypted output.
        /// </summary>
        public byte[] Hash { get; internal set; }
        /// <summary>
        /// The amount of bits that are missing from the key.
        /// </summary>
        public ushort BitDifficulty { get; internal set; }

        /// <summary>
        /// Creates a new CashPack. Only use this to reconstruct properly created CashPacks.
        /// </summary>
        /// <param name="encryptedPayload">The encrypted payload.</param>
        /// <param name="encryptionIv">The encryption IV.</param>
        /// <param name="reducedEncryptionKey">The reduced encryption key.</param>
        /// <param name="sha512Hash">The hash of the decrypted payload.</param>
        /// <param name="bitDifficulty">The amount of stripped bits in the key.</param>
        public CashPack(byte[] encryptedPayload, byte[] encryptionIv, byte[] reducedEncryptionKey, byte[] sha512Hash, ushort bitDifficulty)
        {
            this.Payload = encryptedPayload;
            this.IV = encryptionIv;
            this.Key = reducedEncryptionKey;
            this.Hash = sha512Hash;
            this.BitDifficulty = bitDifficulty;
        }

        internal CashPack()
        {
            
        }
    }
}