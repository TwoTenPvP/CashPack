# CashPack
Bruteforce based packing library inspired by HashCash. Allows for fast packing and slow unpacking.

## Calculate Difficulty
Calculates the difficulty required for a specific solve time on the current hardware.
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

Here is an example of the estimated solve times I got on a Intel i7-4710HQ (8) @ 3.500GHz.

| Difficulty | Estimated Solve Time |
|------------|----------------------|
| 0 | 0.000038 seconds |
| 1 | 0.000075 seconds |
| 2 | 0.000151 seconds |
| 3 | 0.000301 seconds |
| 4 | 0.000602 seconds |
| 5 | 0.001204 seconds |
| 6 | 0.002408 seconds |
| 7 | 0.004817 seconds |
| 8 | 0.009633 seconds |
| 9 | 0.019267 seconds |
| 10 | 0.038533 seconds |
| 11 | 0.077067 seconds |
| 12 | 0.154133 seconds |
| 13 | 0.308267 seconds |
| 14 | 0.616533 seconds |
| 15 | 1 seconds |
| 16 | 2 seconds |
| 17 | 5 seconds |
| 18 | 10 seconds |
| 19 | 20 seconds |
| 20 | 39 seconds |
| 21 | 79 seconds |
| 22 | 158 seconds |
| 23 | 316 seconds |
| 24 | 631 seconds |
| 25 | 1263 seconds |
| 26 | 2525 seconds |
| 27 | 5051 seconds |
| 28 | 3 hours |
| 29 | 6 hours |
| 30 | 11 hours |
| 31 | 22 hours |
| 32 | 45 hours |
| 33 | 4 days |
| 34 | 7 days |
| 35 | 15 days |
| 36 | 30 days |
| 37 | 60 days |
| 38 | 120 days |
| 39 | 239 days |
| 40 | 479 days |
| 41 | 3 years |
| 42 | 5 years |
| 43 | 10 years |
| 44 | 21 years |
| 45 | 42 years |
| 46 | 84 years |
| 47 | 168 years |
| 48 | 336 years |
| 49 | 671 years |
| 50 | 1343 years |
| 51 | 2685 years |
| 52 | 5370 years |
| 53 | 10740 years |
| 54 | 21481 years |
| 55 | 42962 years |
| 56 | 85924 years |
| 57 | 171847 years |
| 58 | 343694 years |
| 59 | 687389 years |
| 60 | 1374777 years |
| 61 | 2749554 years |
| 62 | 5499109 years |
| 63 | 10998217 years |
| 64 | 21996434 years |


For all these reasons, solve times are not exact, they are simply estimated based on current hardware and average luck.

# Future Work
Here is what can be done to evolve the project.

## Algorithm
### Scale
If you want to evolve this further, using a different encryption algorithm would be the main thing to do. One that relies on simple operations and doesn't scale too differently on different hardware. AES is not optimal as it can be accelerated and be very fast.
### Speed
Another big thing is to pick an encryption algorithm that is faster, this will give much more control of the solve times.

## Parallelization
Add parallelized solving. Currently you can specify the level of parallelization when estimating the difficulty, but the library doesn't offer parallelized solving. This would be as simple as chunking the key prediction into even blocks and solving one block per thread.