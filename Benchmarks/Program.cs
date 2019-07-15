using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BetterStreams;
using ObjectLayoutInspector;
using System;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var benchmarks = new BenchmarkSwitcher(new[]
            {
                typeof(Streams)
            });

            benchmarks.Run(
                args,
                ManualConfig.Create(DefaultConfig.Instance)
                    .With(Job.MediumRun.WithLaunchCount(1).WithGcServer(true))
                    .With(MemoryDiagnoser.Default));
        }
    }
}
