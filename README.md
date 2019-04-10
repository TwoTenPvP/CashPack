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

Here is an example of the average solve times I got on a two different CPUs.

| Difficulty | i7-4710HQ @ 3.5GHz | i7-7700K @ 4.5GHz |
|---------|------------------|-------------------|
| 0 bits  | 0.000038 seconds | 0.000034 seconds  |
| 1 bits  | 0.000075 seconds | 0.000068 seconds  |
| 2 bits  | 0.000151 seconds | 0.000136 seconds  |
| 3 bits  | 0.000301 seconds | 0.000271 seconds  |
| 4 bits  | 0.000602 seconds | 0.000543 seconds  |
| 5 bits  | 0.001204 seconds | 0.001086 seconds  |
| 6 bits  | 0.002408 seconds | 0.002172 seconds  |
| 7 bits  | 0.004817 seconds | 0.004344 seconds  |
| 8 bits  | 0.009633 seconds | 0.008688 seconds  |
| 9 bits  | 0.019267 seconds | 0.017375 seconds  |
| 10 bits | 0.038533 seconds | 0.034751 seconds  |
| 11 bits | 0.077067 seconds | 0.069502 seconds  |
| 12 bits | 0.154133 seconds | 0.139004 seconds  |
| 13 bits | 0.308267 seconds | 0.278008 seconds  |
| 14 bits | 0.616533 seconds | 0.556016 seconds  |
| 15 bits | 1 seconds        | 1 seconds         |
| 16 bits | 2 seconds        | 2 seconds         |
| 17 bits | 5 seconds        | 4 seconds         |
| 18 bits | 10 seconds       | 9 seconds         |
| 19 bits | 20 seconds       | 18 seconds        |
| 20 bits | 39 seconds       | 36 seconds        |
| 21 bits | 79 seconds       | 71 seconds        |
| 22 bits | 158 seconds      | 142 seconds       |
| 23 bits | 316 seconds      | 285 seconds       |
| 24 bits | 631 seconds      | 569 seconds       |
| 25 bits | 1263 seconds     | 1139 seconds      |
| 26 bits | 2525 seconds     | 2277 seconds      |
| 27 bits | 5051 seconds     | 4555 seconds      |
| 28 bits | 3 hours          | 3 hours           |
| 29 bits | 6 hours          | 5 hours           |
| 30 bits | 11 hours         | 10 hours          |
| 31 bits | 22 hours         | 20 hours          |
| 32 bits | 45 hours         | 40 hours          |
| 33 bits | 4 days           | 3 days            |
| 34 bits | 7 days           | 7 days            |
| 35 bits | 15 days          | 13 days           |
| 36 bits | 30 days          | 27 days           |
| 37 bits | 60 days          | 54 days           |
| 38 bits | 120 days         | 108 days          |
| 39 bits | 239 days         | 216 days          |
| 40 bits | 479 days         | 432 days          |
| 41 bits | 3 years          | 2 years           |
| 42 bits | 5 years          | 5 years           |
| 43 bits | 10 years         | 9 years           |
| 44 bits | 21 years         | 19 years          |
| 45 bits | 42 years         | 38 years          |
| 46 bits | 84 years         | 76 years          |
| 47 bits | 168 years        | 151 years         |
| 48 bits | 336 years        | 303 years         |
| 49 bits | 671 years        | 605 years         |
| 50 bits | 1343 years       | 1211 years        |
| 51 bits | 2685 years       | 2422 years        |
| 52 bits | 5370 years       | 4843 years        |
| 53 bits | 10740 years      | 9686 years        |
| 54 bits | 21481 years      | 19372 years       |
| 55 bits | 42962 years      | 38745 years       |
| 56 bits | 85924 years      | 77489 years       |
| 57 bits | 171847 years     | 154979 years      |
| 58 bits | 343694 years     | 309958 years      |
| 59 bits | 687389 years     | 619916 years      |
| 60 bits | 1374777 years    | 1239832 years     |
| 61 bits | 2749554 years    | 2479664 years     |
| 62 bits | 5499109 years    | 4959327 years     |
| 63 bits | 10998217 years   | 9918655 years     |
| 64 bits | 21996434 years   | 19837310 years    |


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