using System;
using System.Text;
using NUnit.Framework;

namespace CashPack.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestOneSecondPackAndSolve()
        {
            string plainTextDecoded = "HELLO WORLD";
            
            byte[] plainTextPayload = Encoding.UTF8.GetBytes(plainTextDecoded);

            ushort difficulty = Calculator.GetBitsForPayloadDecodeTime(plainTextPayload, TimeSpan.FromSeconds(1), EstimationAccuracy.VeryLow);

            CashPack pack = Packer.Pack(plainTextPayload, difficulty);

            byte[] solvedResult = Solver.Solve(pack);

            string solvedDecoded = Encoding.UTF8.GetString(solvedResult);
            
            Assert.AreEqual(plainTextDecoded, solvedDecoded);
        }

        [Test]
        public void TestOneSecondPackAndSolveIncremental()
        {
            string plainTextDecoded = "HELLO WORLD";
            
            byte[] plainTextPayload = Encoding.UTF8.GetBytes(plainTextDecoded);

            ushort difficulty = Calculator.GetBitsForPayloadDecodeTime(plainTextPayload, TimeSpan.FromSeconds(1), EstimationAccuracy.VeryLow);

            CashPack pack = Packer.Pack(plainTextPayload, difficulty);

            ProgressPack progressPack = ProgressPack.Create(pack);

            byte[] result;
            
            while (!Solver.SolveIncremental(1000, progressPack, out result)) ;

            string solvedDecoded = Encoding.UTF8.GetString(result);
            
            Assert.AreEqual(plainTextDecoded, solvedDecoded);
        }
    }
}