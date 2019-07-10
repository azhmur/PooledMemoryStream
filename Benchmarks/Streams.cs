using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BetterStreams;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class Streams
    {
        private const int blockSize = 4096;

        private byte[] data;

        private byte[] data2;

        [Params(10_000, 100_000, 1_000_000)]
        public int Length;

        [GlobalSetup]
        public void GlobalSetup()
        {
            this.data = new byte[this.Length];
            this.data2 = new byte[this.Length];
        }

        [Benchmark]
        public void Array()
        {
            var array = new byte[this.Length];
            this.data.CopyTo(array, 0);
        }

        [Benchmark]
        public void ArrayPool()
        {
            var array = ArrayPool<byte>.Shared.Rent(this.Length);

            for (int position = 0; position < this.Length; position += blockSize)
            {
                Buffer.BlockCopy(this.data, position, array, position, Math.Min(blockSize, this.Length - position));
            }

            ArrayPool<byte>.Shared.Return(array);
        }

        [Benchmark]
        public void MemoryStream()
        {
            using (var stream = new MemoryStream())
            {
                for (int position = 0; position < this.Length; position += blockSize)
                {
                    stream.Write(this.data, position, Math.Min(blockSize, this.Length - position));
                }
            }
        }

        [Benchmark]
        public void PooledMemoryStream()
        {
            using (var stream = new PooledMemoryStream())
            {
                for (int position = 0; position < this.Length; position += blockSize)
                {
                    stream.Write(this.data, position, Math.Min(blockSize, this.Length - position));
                }
            }
        }

        [Benchmark(Baseline = true)]
        public void Copy()
        {
            Buffer.BlockCopy(this.data, 0, this.data2, 0, this.Length);
        }

        [Benchmark()]
        public void Fill()
        {
            this.data.AsSpan().Fill(1);
        }
    }
}
