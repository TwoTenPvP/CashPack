# CashPack
Bruteforce based packing library inspired by HashCash. Allows for fast packing and slow unpacking. CashPack is not intended to be used on large data or non random data as it's using XOR encryption. Instead it's intended to be used on a key that is protecting a larger payload. A common scenario is where you AES encrypt a payload, then CashPack the key and submit it along side the payload, the payload key then has to be solved in order to decrypt the payload.

## Calculate Difficulty
Calculates the difficulty required for a specific average solve time on the current hardware with a specific payload.
```csharp
// Gets the difficulty required for an average solve time of 1 minute on the current hardware with the specified payload.
ushort difficulty = Calculator.GetBitsForPayloadDecodeTime(Encoding.UTF8.GetBytes("HELLO WORLD"), TimeSpan.FromMinutes(1));
```

## Pack
Packs the payload into a CashPack which has to be solved to get the original payload back.
```csharp
// Pack the payload into a CashPack
CashPack pack = Packer.Pack(Encoding.UTF8.GetBytes("HELLO WORLD"), difficulty);
```

## Solve
Solves the CashPack in one go. Depending on the difficulty and hardware, this can take a long time. For long running solve operations, check out the incremental solver.
```csharp
// Solve CashPack in one go
byte[] result = Solver.Solve(pack);
```

## Incremental Solve
Solves a CashPack incrementally, allowing you to save the progress and resume later.
```csharp
// Creates a progressPack object to store the current progress.
ProgressPack progressPack = ProgressPack.Create(pack);

// Try to solve the pack. Will pause every 10_000 iterations to allow you to save the progress.
while (!Solver.SolveIncremental(10_000, progressPack, out byte[] result))
{
    // Here you can save the progressPack to disk.
    // You can always resume the incremental solving later by reconstructing the progressPack and calling SolveIncremental on it.
}
```

# Limitations
Here are some possible weaknesses and limitations.

## Parallelization
Because solving can be parallelized, solve times can vary greatly. This also applies to HashCash.
## Estimated Time
Because of the factors of the solve time being so many and complex, it's very difficult to get an accurate unpack date. There reasons are:
1. Solve can be parallelized which can make the solve time many **times** less, this has to be accounted for.
2. Bruteforce is dependent on a random key. That means that you only know the average solve time. In theory it could be solved first try.
3. Because the time complexity of a bruteforce is exponential, this gives little control of the exact solve time.

For all these reasons, solve times are not exact, they are simply estimated based on current hardware and average luck.

# Future Work
Here is what can be done to evolve the project.

## Parallelization
Add parallelized solving. Currently you can specify the level of parallelization when estimating the difficulty, but the library doesn't offer parallelized solving. This would be as simple as chunking the key prediction into even blocks and solving one block per thread.