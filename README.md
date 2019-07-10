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
|                        Fill |     20 |      3.758 ns |     0.0521 ns |     0.0487 ns |   0.80 |    0.02 |       - |       - |       - |         - |
|                        Copy |     20 |      4.673 ns |     0.0634 ns |     0.0593 ns |   1.00 |    0.00 |       - |       - |       - |         - |
| StackAllocWithoutLocalsInit |     20 |     11.070 ns |     0.0508 ns |     0.0475 ns |   2.37 |    0.03 |       - |       - |       - |         - |
|                  StackAlloc |     20 |     11.142 ns |     0.1076 ns |     0.0898 ns |   2.38 |    0.04 |       - |       - |       - |         - |
|                       Array |     20 |     19.553 ns |     0.3221 ns |     0.3013 ns |   4.18 |    0.08 |  0.0007 |       - |       - |      48 B |
|                   ArrayPool |     20 |     37.095 ns |     0.2159 ns |     0.2019 ns |   7.94 |    0.11 |       - |       - |       - |         - |
|                MemoryStream |     20 |     58.037 ns |     0.8776 ns |     0.7779 ns |  12.41 |    0.27 |  0.0056 |       - |       - |     352 B |
|          PooledMemoryStream |     20 |     66.881 ns |     0.1896 ns |     0.1773 ns |  14.31 |    0.18 |  0.0010 |       - |       - |      64 B |
|                      Native |     20 |     68.231 ns |     1.1526 ns |     1.0781 ns |  14.60 |    0.37 |       - |       - |       - |         - |
|      RecyclableMemoryStream |     20 |  1,639.042 ns |    10.6559 ns |     9.9676 ns | 350.80 |    4.60 |  0.0057 |       - |       - |     448 B |
|                             |        |               |               |               |        |         |         |         |         |           |
|                        Fill |  10000 |     78.834 ns |     0.7876 ns |     0.7367 ns |   0.63 |    0.01 |       - |       - |       - |         - |
| StackAllocWithoutLocalsInit |  10000 |    100.217 ns |     1.6301 ns |     1.5248 ns |   0.79 |    0.01 |       - |       - |       - |         - |
|                        Copy |  10000 |    126.100 ns |     0.6411 ns |     0.5353 ns |   1.00 |    0.00 |       - |       - |       - |         - |
|                   ArrayPool |  10000 |    167.842 ns |     3.5533 ns |     3.3237 ns |   1.34 |    0.03 |       - |       - |       - |         - |
|                      Native |  10000 |    216.898 ns |     5.6388 ns |     4.9986 ns |   1.72 |    0.04 |       - |       - |       - |         - |
|                  StackAlloc |  10000 |    425.450 ns |     8.3980 ns |     7.8555 ns |   3.36 |    0.05 |       - |       - |       - |         - |
|          PooledMemoryStream |  10000 |    523.484 ns |     7.0227 ns |     6.5690 ns |   4.14 |    0.05 |       - |       - |       - |      64 B |
|                       Array |  10000 |    797.254 ns |    13.5948 ns |    12.7166 ns |   6.33 |    0.10 |  0.1593 |       - |       - |   10024 B |
|      RecyclableMemoryStream |  10000 |  1,909.457 ns |    12.8024 ns |    11.9754 ns |  15.14 |    0.12 |  0.0038 |       - |       - |     448 B |
|                MemoryStream |  10000 |  2,036.022 ns |    29.8443 ns |    26.4562 ns |  16.16 |    0.22 |  0.3929 |  0.0076 |       - |   28816 B |
|                             |        |               |               |               |        |         |         |         |         |           |
|                        Fill | 100000 |  1,655.496 ns |    50.7124 ns |    47.4365 ns |   0.95 |    0.03 |       - |       - |       - |         - |
|                        Copy | 100000 |  1,737.465 ns |    29.9842 ns |    26.5802 ns |   1.00 |    0.00 |       - |       - |       - |         - |
|                      Native | 100000 |  1,831.317 ns |    20.4847 ns |    18.1592 ns |   1.05 |    0.02 |       - |       - |       - |         - |
| StackAllocWithoutLocalsInit | 100000 |  2,006.037 ns |    25.0503 ns |    23.4320 ns |   1.15 |    0.02 |       - |       - |       - |         - |
|                   ArrayPool | 100000 |  2,009.389 ns |    16.0198 ns |    14.9849 ns |   1.16 |    0.02 |       - |       - |       - |         - |
|      RecyclableMemoryStream | 100000 |  4,557.684 ns |   136.9321 ns |   121.3867 ns |   2.62 |    0.06 |       - |       - |       - |     448 B |
|                  StackAlloc | 100000 |  5,175.712 ns |   115.4865 ns |   108.0262 ns |   2.98 |    0.09 |       - |       - |       - |         - |
|          PooledMemoryStream | 100000 |  5,276.844 ns |    31.3270 ns |    27.7706 ns |   3.04 |    0.05 |       - |       - |       - |      64 B |
|                       Array | 100000 | 33,924.043 ns | 2,701.6352 ns | 2,255.9878 ns |  19.56 |    1.26 | 31.1890 | 31.1890 | 31.1890 |  100024 B |
|                MemoryStream | 100000 | 65,104.760 ns |   851.1694 ns |   796.1844 ns |  37.45 |    0.83 | 36.8652 | 36.8652 | 36.8652 |  258273 B |
