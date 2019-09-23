using Aethon.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BetterStreams;
using LocalsInit;
using Microsoft.IO;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
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

        private byte[] data2;

        private RecyclableMemoryStreamManager memoryManager;

        private object o;

        [Params(24, 10_000, 100_000)]
        public int Length;

        [GlobalSetup]
        public void GlobalSetup()
        {
            this.data = new byte[this.Length];
            this.data2 = new byte[this.Length];
            this.memoryManager = new RecyclableMemoryStreamManager();
        }

        [Benchmark]
        public void NewComplexObject()
        {
            for (int i = 0; i < Length / 24; ++i)
            {
                var complex = new Complex();
                complex.Dispose();
            }
        }

        [Benchmark]
        public void NewObject()
        {
            for (int i = 0; i < Length / 24; ++i)
            {
                this.o = new object();
            }
        }

        [Benchmark]
        [BenchmarkCategory("Basic")]
        public void NewArray()
        {
            var array = new byte[this.Length];
        }

        [Benchmark]
        [BenchmarkCategory("Basic")]
        public unsafe void Native()
        {
            var native = Marshal.AllocHGlobal(this.Length);

            Marshal.FreeHGlobal(native);
        }

        [Benchmark]
        [BenchmarkCategory("Basic")]
        public void StackAlloc()
        {
            Span<byte> data = stackalloc byte[this.Length];
        }

        [Benchmark]
        [LocalsInit(false)]
        [BenchmarkCategory("Basic")]
        public void StackAllocWithoutLocalsInit()
        {
            Span<byte> data = stackalloc byte[this.Length];
        }

        [Benchmark]
        [BenchmarkCategory("Basic")]
        public void ArrayPool()
        {
            var array = ArrayPool<byte>.Shared.Rent(this.Length);

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

        [Benchmark()]
        public void Copy()
        {
            Buffer.BlockCopy(this.data, 0, this.data2, 0, this.Length);
        }

        [Benchmark()]
        public void CopyByteLoop()
        {
            for (int i = 0; i < this.Length; ++i)
            {
                this.data2[i] = this.data[i];
            }
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory("Basic")]
        public void Fill()
        {
            this.data.AsSpan().Fill(1);
        }

        private class Complex
        {
            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            ~Complex()
            {
                var i = 1;
            }
        }
    }
}
