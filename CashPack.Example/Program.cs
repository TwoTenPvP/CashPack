using System;
using System.Diagnostics;
using System.Text;

namespace CashPack.Example
{
    internal class Program
    {
        private const string Lipsum = "Magni et sit rerum odio. Optio quo in atque modi molestias architecto blanditiis veritatis. Vel ut perspiciatis praesentium qui et veniam. Quia ut officiis aut non. Accusantium repudiandae deleniti ducimus non aliquid quidem enim. Minima ipsa voluptas saepe dignissimos exercitationem provident dolore. In excepturi officia molestiae at unde aut nihil. Veniam maiores ducimus aut minus unde officia. Voluptas id sit nemo sit porro delectus esse recusandae. Consequatur ipsam labore autem harum. Nihil ullam eos et suscipit. Vel sed sequi quam earum molestias eos dolores consequatur. Voluptatem nam omnis ut numquam ex. Facere ipsum qui delectus ut. Enim dolorem rem alias cupiditate eum. Eaque iusto hic et ipsa non delectus ex laborum. Sit et autem similique nisi maxime adipisci. Commodi non sint doloribus. Quia qui dolore commodi sunt quia ab. Dolores sit dolorem sed est praesentium omnis. Incidunt veritatis dolor quia. Sed velit hic quia. Quos eveniet placeat consectetur ipsam.";
        
        public static void Main(string[] args)
        {
            // Encode the payload as bytes.
            byte[] decryptedPayload = Encoding.UTF8.GetBytes(Lipsum);

            // Calculate the difficulty for the current machine. Takes ~10 seconds.
            Console.WriteLine("Calculating difficulty for an average solve time of 1 minute...");
            ushort difficulty = Calculator.GetBitsForPayloadDecodeTime(decryptedPayload, TimeSpan.FromMinutes(1));
            Console.WriteLine("Difficulty required is " + difficulty);
            
            
            // Packs the payload. Very fast operation.
            Console.WriteLine("Packing payload with " + difficulty + " difficulty...");
            CashPack pack = Packer.Pack(decryptedPayload, difficulty);
            Console.WriteLine("Packing completed!");
            
            
            // Create measurement watch.
            Stopwatch oneGoSolveTime = new Stopwatch();
            oneGoSolveTime.Start();

            {
                // Solves the CashPack in one go.
                Console.WriteLine("Solving HashPack in one go...");
                byte[] result = Solver.Solve(pack);
                Console.WriteLine("Solved CashPack successfully in " + oneGoSolveTime.ElapsedMilliseconds + " ms!");   
            }

            // Stop watch.
            oneGoSolveTime.Stop();
            
            // Creates a CashPack container with progress.
            ProgressPack progressPack = ProgressPack.Create(pack);

            // Create measurements for the incremental solve.
            int increments = 0;
            Stopwatch incrementalWatch = new Stopwatch();
            incrementalWatch.Start();

            {
                // Solves the CashPack in an incremental manner. It will attempt to solve it 10_000 times, then return.
                Console.WriteLine("Solving CashPack in iterations of 10.000...");
                while (!Solver.SolveIncremental(10_000, progressPack, out byte[] result))
                {
                    // Here you can save the progressPack to disk.
                    // You can always resume the incremental solving later by reconstructing the progressPack and calling SolveIncremental on it.
                    increments++;
                }
                Console.WriteLine("Solved CashPack incrementally successfully after " + increments + " increments in " + incrementalWatch.ElapsedMilliseconds + " ms!");
            }
            
            // Stop watch.
            incrementalWatch.Stop();            
        }
    }
}