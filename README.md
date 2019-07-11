![Alt text](perf.png?raw=true "Perf")

``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18362
Intel Core i5-9600K CPU 3.70GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET Core SDK=2.2.300
  [Host]    : .NET Core 2.2.5 (CoreCLR 4.6.27617.05, CoreFX 4.6.27618.01), 64bit RyuJIT
  MediumRun : .NET Core 2.2.5 (CoreCLR 4.6.27617.05, CoreFX 4.6.27618.01), 64bit RyuJIT

Job=MediumRun  Server=True  IterationCount=15  
LaunchCount=1  WarmupCount=10  

```
|                      Method | Length |          Mean |         Error |        StdDev |  Ratio | RatioSD |   Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|---------------------------- |------- |--------------:|--------------:|--------------:|-------:|--------:|--------:|--------:|--------:|----------:|
|                        Fill |     20 |      4.137 ns |     0.2342 ns |     0.2191 ns |   0.85 |    0.05 |       - |       - |       - |         - |
|                        Copy |     20 |      4.849 ns |     0.0894 ns |     0.0836 ns |   1.00 |    0.00 |       - |       - |       - |         - |
|                    NewArray |     20 |      8.037 ns |     0.1553 ns |     0.1452 ns |   1.66 |    0.04 |  0.0007 |       - |       - |      48 B |
|                  StackAlloc |     20 |     11.358 ns |     0.0731 ns |     0.0683 ns |   2.34 |    0.04 |       - |       - |       - |         - |
| StackAllocWithoutLocalsInit |     20 |     11.490 ns |     0.1257 ns |     0.1175 ns |   2.37 |    0.04 |       - |       - |       - |         - |
|                CopyByteLoop |     20 |     12.256 ns |     0.3545 ns |     0.3316 ns |   2.53 |    0.09 |       - |       - |       - |         - |
|                   ArrayPool |     20 |     38.115 ns |     0.3081 ns |     0.2731 ns |   7.87 |    0.12 |       - |       - |       - |         - |
|                MemoryStream |     20 |     48.095 ns |     0.5204 ns |     0.4613 ns |   9.93 |    0.17 |  0.0055 |       - |       - |     352 B |
|          PooledMemoryStream |     20 |     64.095 ns |     0.3816 ns |     0.3187 ns |  13.23 |    0.26 |  0.0008 |       - |       - |      64 B |
|                      Native |     20 |     70.426 ns |     1.1829 ns |     1.1064 ns |  14.53 |    0.36 |       - |       - |       - |         - |
|      RecyclableMemoryStream |     20 |  1,696.492 ns |    26.1682 ns |    24.4778 ns | 350.02 |    9.17 |  0.0057 |       - |       - |     448 B |
|                             |        |               |               |               |        |         |         |         |         |           |
|                        Fill |  10000 |     81.735 ns |     0.4842 ns |     0.4293 ns |   0.62 |    0.03 |       - |       - |       - |         - |
| StackAllocWithoutLocalsInit |  10000 |    102.891 ns |     4.4224 ns |     4.1367 ns |   0.79 |    0.05 |       - |       - |       - |         - |
|                        Copy |  10000 |    131.125 ns |     6.0070 ns |     5.6190 ns |   1.00 |    0.00 |       - |       - |       - |         - |
|                   ArrayPool |  10000 |    174.800 ns |     6.0385 ns |     5.6484 ns |   1.34 |    0.08 |       - |       - |       - |         - |
|                      Native |  10000 |    227.224 ns |    17.7400 ns |    16.5940 ns |   1.74 |    0.17 |       - |       - |       - |         - |
|                  StackAlloc |  10000 |    398.897 ns |    15.4594 ns |    14.4607 ns |   3.05 |    0.20 |       - |       - |       - |         - |
|          PooledMemoryStream |  10000 |    404.287 ns |     8.2584 ns |     6.8961 ns |   3.07 |    0.12 |  0.0010 |       - |       - |      64 B |
|                    NewArray |  10000 |    556.801 ns |    11.0404 ns |     9.7871 ns |   4.24 |    0.19 |  0.1554 |       - |       - |   10024 B |
|      RecyclableMemoryStream |  10000 |  1,925.756 ns |    20.5536 ns |    17.1632 ns |  14.62 |    0.62 |  0.0038 |       - |       - |     448 B |
|                MemoryStream |  10000 |  2,077.724 ns |    32.5469 ns |    28.8520 ns |  15.83 |    0.72 |  0.3891 |  0.0076 |       - |   28816 B |
|                CopyByteLoop |  10000 |  5,399.961 ns |   202.0025 ns |   188.9533 ns |  41.28 |    2.70 |       - |       - |       - |         - |
|                             |        |               |               |               |        |         |         |         |         |           |
| StackAllocWithoutLocalsInit | 100000 |  1,731.405 ns |    12.6357 ns |     9.8651 ns |   0.98 |    0.02 |       - |       - |       - |         - |
|                        Copy | 100000 |  1,763.524 ns |    43.8676 ns |    41.0338 ns |   1.00 |    0.00 |       - |       - |       - |         - |
|                        Fill | 100000 |  1,775.233 ns |    30.7721 ns |    25.6961 ns |   1.01 |    0.03 |       - |       - |       - |         - |
|                      Native | 100000 |  1,914.330 ns |    20.4151 ns |    17.0476 ns |   1.09 |    0.03 |       - |       - |       - |         - |
|                   ArrayPool | 100000 |  2,100.360 ns |    72.2981 ns |    67.6277 ns |   1.19 |    0.05 |       - |       - |       - |         - |
|          PooledMemoryStream | 100000 |  3,446.430 ns |   118.2598 ns |   110.6203 ns |   1.96 |    0.10 |       - |       - |       - |      64 B |
|      RecyclableMemoryStream | 100000 |  5,144.926 ns |   142.9896 ns |   126.7566 ns |   2.92 |    0.06 |       - |       - |       - |     448 B |
|                  StackAlloc | 100000 |  5,546.492 ns |   193.9061 ns |   181.3799 ns |   3.15 |    0.13 |       - |       - |       - |         - |
|                    NewArray | 100000 | 21,597.481 ns | 6,550.4162 ns | 6,127.2634 ns |  12.25 |    3.46 | 27.6489 | 27.6489 | 27.6489 |  100024 B |
|                CopyByteLoop | 100000 | 54,028.950 ns | 2,515.4464 ns | 2,352.9501 ns |  30.65 |    1.43 |       - |       - |       - |         - |
|                MemoryStream | 100000 | 71,012.148 ns | 1,606.0806 ns | 1,502.3288 ns |  40.29 |    1.27 | 36.7432 | 36.7432 | 36.7432 |  258281 B |
