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

| Difficulty | i7-4710HQ @ 3.5GHz | i7-7700K @ 4.5GHz | i5-8400 @ 2.8GHz | Phenom II X6 1090T @ 3.2GHz | ARMv7 (v7l) @ 1.4GHz |
|---------|------------------|-------------------|------------------|------------------|------------------|
| 0 bits  | 0.000038 seconds | 0.000034 seconds  | 0.000031 seconds | 0.000055 seconds | 0.000871 seconds |
| 1 bits  | 0.000075 seconds | 0.000068 seconds  | 0.000063 seconds | 0.000109 seconds | 0.001742 seconds |
| 2 bits  | 0.000151 seconds | 0.000136 seconds  | 0.000126 seconds | 0.000219 seconds | 0.003485 seconds |
| 3 bits  | 0.000301 seconds | 0.000271 seconds  | 0.000252 seconds | 0.000438 seconds | 0.006969 seconds |
| 4 bits  | 0.000602 seconds | 0.000543 seconds  | 0.000503 seconds | 0.000875 seconds | 0.013939 seconds |
| 5 bits  | 0.001204 seconds | 0.001086 seconds  | 0.001007 seconds | 0.001751 seconds | 0.027877 seconds |
| 6 bits  | 0.002408 seconds | 0.002172 seconds  | 0.002013 seconds | 0.003501 seconds | 0.055755 seconds |
| 7 bits  | 0.004817 seconds | 0.004344 seconds  | 0.004026 seconds | 0.007003 seconds | 0.111509 seconds |
| 8 bits  | 0.009633 seconds | 0.008688 seconds  | 0.008053 seconds | 0.014005 seconds | 0.223019 seconds |
| 9 bits  | 0.019267 seconds | 0.017375 seconds  | 0.016105 seconds | 0.028011 seconds | 0.446038 seconds |
| 10 bits | 0.038533 seconds | 0.034751 seconds  | 0.032210 seconds | 0.056021 seconds | 0.892075 seconds |
| 11 bits | 0.077067 seconds | 0.069502 seconds  | 0.064421 seconds | 0.112042 seconds | 2 seconds        |
| 12 bits | 0.154133 seconds | 0.139004 seconds  | 0.128841 seconds | 0.224085 seconds | 4 seconds        |
| 13 bits | 0.308267 seconds | 0.278008 seconds  | 0.257683 seconds | 0.448169 seconds | 7 seconds        |
| 14 bits | 0.616533 seconds | 0.556016 seconds  | 0.515366 seconds | 0.896339 seconds | 14 seconds       |
| 15 bits | 1 seconds        | 1 seconds         | 1 seconds        | 2 seconds        | 29 seconds       |
| 16 bits | 2 seconds        | 2 seconds         | 2 seconds        | 4 seconds        | 57 seconds       |
| 17 bits | 5 seconds        | 4 seconds         | 4 seconds        | 7 seconds        | 114 seconds      |
| 18 bits | 10 seconds       | 9 seconds         | 8 seconds        | 14 seconds       | 228 seconds      |
| 19 bits | 20 seconds       | 18 seconds        | 16 seconds       | 29 seconds       | 457 seconds      |
| 20 bits | 39 seconds       | 36 seconds        | 33 seconds       | 57 seconds       | 913 seconds      |
| 21 bits | 79 seconds       | 71 seconds        | 66 seconds       | 115 seconds      | 1827 seconds     |
| 22 bits | 158 seconds      | 142 seconds       | 132 seconds      | 229 seconds      | 3654 seconds     |
| 23 bits | 316 seconds      | 285 seconds       | 264 seconds      | 459 seconds      | 2 hours          |
| 24 bits | 631 seconds      | 569 seconds       | 528 seconds      | 918 seconds      | 4 hours          |
| 25 bits | 1263 seconds     | 1139 seconds      | 1055 seconds     | 1836 seconds     | 8 hours          |
| 26 bits | 2525 seconds     | 2277 seconds      | 2111 seconds     | 3671 seconds     | 16 hours         |
| 27 bits | 5051 seconds     | 4555 seconds      | 4222 seconds     | 2 hours          | 32 hours         |
| 28 bits | 3 hours          | 3 hours           | 2 hours          | 4 hours          | 3 days           |
| 29 bits | 6 hours          | 5 hours           | 5 hours          | 8 hours          | 5 days           |
| 30 bits | 11 hours         | 10 hours          | 9 hours          | 16 hours         | 11 days          |
| 31 bits | 22 hours         | 20 hours          | 19 hours         | 33 hours         | 22 days          |
| 32 bits | 45 hours         | 40 hours          | 38 hours         | 3 days           | 43 days          |
| 33 bits | 4 days           | 3 days            | 3 days           | 5 days           | 87 days          |
| 34 bits | 7 days           | 7 days            | 6 days           | 11 days          | 173 days         |
| 35 bits | 15 days          | 13 days           | 13 days          | 22 days          | 346 days         |
| 36 bits | 30 days          | 27 days           | 25 days          | 44 days          | 693 days         |
| 37 bits | 60 days          | 54 days           | 50 days          | 87 days          | 4 years          |
| 38 bits | 120 days         | 108 days          | 100 days         | 174 days         | 8 years          |
| 39 bits | 239 days         | 216 days          | 200 days         | 348 days         | 15 years         |
| 40 bits | 479 days         | 432 days          | 400 days         | 696 days         | 30 years         |
| 41 bits | 3 years          | 2 years           | 2 years          | 4 years          | 61 years         |
| 42 bits | 5 years          | 5 years           | 4 years          | 8 years          | 121 years        |
| 43 bits | 10 years         | 9 years           | 9 years          | 15 years         | 243 years        |
| 44 bits | 21 years         | 19 years          | 18 years         | 30 years         | 486 years        |
| 45 bits | 42 years         | 38 years          | 35 years         | 61 years         | 971 years        |
| 46 bits | 84 years         | 76 years          | 70 years         | 122 years        | 1943 years       |
| 47 bits | 168 years        | 151 years         | 140 years        | 244 years        | 3885 years       |
| 48 bits | 336 years        | 303 years         | 281 years        | 488 years        | 7770 years       |
| 49 bits | 671 years        | 605 years         | 561 years        | 976 years        | 15541 years      |
| 50 bits | 1343 years       | 1211 years        | 1122 years       | 1952 years       | 31081 years      |
| 51 bits | 2685 years       | 2422 years        | 2245 years       | 3904 years       | 62162 years      |
| 52 bits | 5370 years       | 4843 years        | 4489 years       | 7807 years       | 124325 years     |
| 53 bits | 10740 years      | 9686 years        | 8978 years       | 15615 years      | 248649 years     |
| 54 bits | 21481 years      | 19372 years       | 17956 years      | 31230 years      | 497299 years     |
| 55 bits | 42962 years      | 38745 years       | 35912 years      | 62459 years      | 994597 years     |
| 56 bits | 85924 years      | 77489 years       | 71824 years      | 124919 years     | 1989195 years    |
| 57 bits | 171847 years     | 154979 years      | 143649 years     | 249838 years     | 3978390 years    |
| 58 bits | 343694 years     | 309958 years      | 287297 years     | 499676 years     | 7956779 years    |
| 59 bits | 687389 years     | 619916 years      | 574595 years     | 999351 years     | 15913559 years   |
| 60 bits | 1374777 years    | 1239832 years     | 1149189 years    | 1998702 years    | 31827118 years   |
| 61 bits | 2749554 years    | 2479664 years     | 2298379 years    | 3997404 years    | 63654236 years   |
| 62 bits | 5499109 years    | 4959327 years     | 4596757 years    | 7994809 years    | 127308471 years  |
| 63 bits | 10998217 years   | 9918655 years     | 9193515 years    | 15989618 years   | 254616943 years  |
| 64 bits | 21996434 years   | 19837310 years    | 18387029 years   | 31979235 years   | 509233885 years  |

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