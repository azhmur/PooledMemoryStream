using Aethon.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BetterStreams;
using Microsoft.IO;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public class Streams
    {
        private const int blockSize = 4096;

        private byte[] data;

        private RecyclableMemoryStreamManager memoryManager;

        [Params(24, 10_000, 100_000)]
        public int Length;

        [GlobalSetup]
        public void GlobalSetup()
        {
            this.data = new byte[this.Length];
            this.memoryManager = new RecyclableMemoryStreamManager();
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
        public void SmallBlockMemoryStream()
        {
            using (var stream = new SmallBlockMemoryStream())
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

        [Benchmark]
        public void PoolStream()
        {
            using (var stream = new PooledStream.PooledMemoryStream())
            {
                for (int position = 0; position < this.Length; position += blockSize)
                {
                    stream.Write(this.data, position, Math.Min(blockSize, this.Length - position));
                }
            }
        }

        [Benchmark]
        public void RecyclableMemoryStream()
        {
            using (var stream = new RecyclableMemoryStream(this.memoryManager))
            {
                for (int position = 0; position < this.Length; position += blockSize)
                {
                    stream.Write(this.data, position, Math.Min(blockSize, this.Length - position));
                }
            }
        }
    }
}
