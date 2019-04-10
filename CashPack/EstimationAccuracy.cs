namespace CashPack
{
    /// <summary>
    /// Represents the accuracy of a solve time estimation.
    /// </summary>
    public enum EstimationAccuracy : uint
    {
        /// <summary>
        /// Benchmarks for 60 seconds to determine the processor speed.
        /// </summary>
        VeryHigh = 60,
        /// <summary>
        /// Benchmarks for 30 seconds to determine the processor speed.
        /// </summary>
        High = 30,
        /// <summary>
        /// Benchmarks for 10 seconds to determine the processor speed.
        /// </summary>
        Normal = 10,
        /// <summary>
        /// Benchmarks for 5 seconds to determine the processor speed.
        /// </summary>
        Low = 5,
        /// <summary>
        /// Benchmarks for 1 seconds to determine the processor speed.
        /// </summary>
        VeryLow = 1
    }
}