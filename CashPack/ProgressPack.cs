namespace CashPack
{
    /// <summary>
    /// A class representing a semi solved CashPack. Used for incremental solving.
    /// </summary>
    public class ProgressPack : CashPack
    {
        /// <summary>
        /// The amount of iterations the pack has tried already.
        /// </summary>
        public ulong WorkingIteration { get; internal set; }

        /// <summary>
        /// Creates a brand new Progress pack from a CashPack. Used before starting a incremental solve for the first time.
        /// </summary>
        /// <param name="pack">The original CashPack.</param>
        /// <returns>The progress pack containing all the values from the original CashPack</returns>
        public static ProgressPack Create(CashPack pack)
        {
            return new ProgressPack()
            {
                Key = pack.Key,
                BitDifficulty = pack.BitDifficulty,
                Hash = pack.Hash,
                Payload = pack.Payload,
                WorkingIteration = 0
            };
        }

        /// <summary>
        /// Constructs a ProgressPack. Use this to reconstruct properly constructed ProgressPacks.
        /// </summary>
        /// <param name="iterations">The amount of iterations the pack has tried already.</param>
        /// <param name="pack">The pack to attempt to solve.</param>
        public ProgressPack(ulong iterations, CashPack pack)
        {
            Key = pack.Key;
            BitDifficulty = pack.BitDifficulty;
            Hash = pack.Hash;
            Payload = pack.Payload;
            WorkingIteration = iterations;
        }

        internal ProgressPack()
        {
            
        }
    }
}