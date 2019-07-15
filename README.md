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
|                        Fill |     20 |      3.604 ns |     0.0224 ns |     0.0210 ns |   0.79 |    0.00 |       - |       - |       - |         - |
|                        Copy |     20 |      4.545 ns |     0.0212 ns |     0.0198 ns |   1.00 |    0.00 |       - |       - |       - |         - |
|                    NewArray |     20 |      8.063 ns |     0.0259 ns |     0.0243 ns |   1.77 |    0.01 |  0.0008 |       - |       - |      48 B |
|                  StackAlloc |     20 |     10.795 ns |     0.0384 ns |     0.0360 ns |   2.38 |    0.01 |       - |       - |       - |         - |
| StackAllocWithoutLocalsInit |     20 |     10.890 ns |     0.0475 ns |     0.0421 ns |   2.40 |    0.01 |       - |       - |       - |         - |
|                CopyByteLoop |     20 |     11.221 ns |     0.0499 ns |     0.0443 ns |   2.47 |    0.01 |       - |       - |       - |         - |
|                   ArrayPool |     20 |     37.521 ns |     0.0832 ns |     0.0779 ns |   8.26 |    0.04 |       - |       - |       - |         - |
|      SmallBlockMemoryStream |     20 |     56.281 ns |     0.2089 ns |     0.1954 ns |  12.38 |    0.07 |  0.0075 |       - |       - |     472 B |
|                MemoryStream |     20 |     57.892 ns |     0.2554 ns |     0.2389 ns |  12.74 |    0.08 |  0.0056 |       - |       - |     352 B |
|          PooledMemoryStream |     20 |     60.027 ns |     0.1529 ns |     0.1430 ns |  13.21 |    0.07 |  0.0010 |       - |       - |      64 B |
|                      Native |     20 |     67.875 ns |     0.2979 ns |     0.2641 ns |  14.94 |    0.09 |       - |       - |       - |         - |
|      RecyclableMemoryStream |     20 |  1,593.734 ns |     9.5103 ns |     8.4306 ns | 350.77 |    2.20 |  0.0057 |       - |       - |     448 B |
|                             |        |               |               |               |        |         |         |         |         |           |
|                        Fill |  10000 |     82.149 ns |     0.1985 ns |     0.1857 ns |   0.66 |    0.01 |       - |       - |       - |         - |
|                        Copy |  10000 |    124.164 ns |     1.0477 ns |     0.9800 ns |   1.00 |    0.00 |       - |       - |       - |         - |
| StackAllocWithoutLocalsInit |  10000 |    128.688 ns |     0.3868 ns |     0.3618 ns |   1.04 |    0.01 |       - |       - |       - |         - |
|                   ArrayPool |  10000 |    154.335 ns |     2.3499 ns |     2.1981 ns |   1.24 |    0.02 |       - |       - |       - |         - |
|                      Native |  10000 |    207.222 ns |     2.5186 ns |     2.3559 ns |   1.67 |    0.02 |       - |       - |       - |         - |
|                  StackAlloc |  10000 |    410.791 ns |     0.9419 ns |     0.8810 ns |   3.31 |    0.03 |       - |       - |       - |         - |
|          PooledMemoryStream |  10000 |    415.421 ns |     4.5877 ns |     4.2913 ns |   3.35 |    0.05 |  0.0010 |       - |       - |      64 B |
|                    NewArray |  10000 |    618.266 ns |     4.8143 ns |     4.0202 ns |   4.97 |    0.06 |  0.1574 |       - |       - |   10024 B |
|      SmallBlockMemoryStream |  10000 |  1,345.493 ns |     2.7104 ns |     2.1161 ns |  10.82 |    0.08 |  0.2651 |  0.0019 |       - |   16648 B |
|      RecyclableMemoryStream |  10000 |  1,885.239 ns |    12.8539 ns |    11.3947 ns |  15.18 |    0.17 |  0.0057 |       - |       - |     448 B |
|                MemoryStream |  10000 |  2,290.672 ns |     7.1890 ns |     6.3729 ns |  18.45 |    0.17 |  0.3929 |  0.0076 |       - |   28816 B |
|                CopyByteLoop |  10000 |  4,997.393 ns |    21.4632 ns |    20.0767 ns |  40.25 |    0.39 |       - |       - |       - |         - |
|                             |        |               |               |               |        |         |         |         |         |           |
|                        Fill | 100000 |  1,651.599 ns |    54.3235 ns |    50.8143 ns |   0.97 |    0.03 |       - |       - |       - |         - |
|                        Copy | 100000 |  1,709.284 ns |     8.6351 ns |     8.0773 ns |   1.00 |    0.00 |       - |       - |       - |         - |
|                   ArrayPool | 100000 |  1,995.511 ns |    34.3353 ns |    32.1172 ns |   1.17 |    0.02 |       - |       - |       - |         - |
| StackAllocWithoutLocalsInit | 100000 |  2,015.614 ns |    38.3572 ns |    35.8793 ns |   1.18 |    0.02 |       - |       - |       - |         - |
|                      Native | 100000 |  2,069.543 ns |    19.3944 ns |    16.1952 ns |   1.21 |    0.01 |       - |       - |       - |         - |
|          PooledMemoryStream | 100000 |  3,291.219 ns |    13.1741 ns |    12.3230 ns |   1.93 |    0.01 |       - |       - |       - |      64 B |
|      RecyclableMemoryStream | 100000 |  4,555.815 ns |    68.5092 ns |    64.0836 ns |   2.67 |    0.04 |       - |       - |       - |     448 B |
|                  StackAlloc | 100000 |  5,001.828 ns |    23.4092 ns |    20.7516 ns |   2.93 |    0.02 |       - |       - |       - |         - |
|      SmallBlockMemoryStream | 100000 | 10,365.935 ns |   400.5115 ns |   374.6387 ns |   6.06 |    0.22 |  2.0447 |  0.2289 |       - |  131408 B |
|                    NewArray | 100000 | 28,097.070 ns | 6,823.5906 ns | 6,382.7909 ns |  16.44 |    3.74 | 24.7192 | 24.7192 | 24.7192 |  100024 B |
|                CopyByteLoop | 100000 | 50,431.811 ns |   696.0034 ns |   651.0421 ns |  29.51 |    0.44 |       - |       - |       - |         - |
|                MemoryStream | 100000 | 66,288.145 ns | 1,440.0600 ns | 1,202.5153 ns |  38.77 |    0.65 | 38.8184 | 38.8184 | 38.8184 |  258277 B |
