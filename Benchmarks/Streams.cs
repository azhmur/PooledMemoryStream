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

        [Params(20, 10_000, 100_000)]
        public int Length;

        [GlobalSetup]
        public void GlobalSetup()
        {
            this.data = new byte[this.Length];
            this.data2 = new byte[this.Length];
            this.memoryManager = new RecyclableMemoryStreamManager();
        }

        [Benchmark]
        public void NewArray()
        {
            var array = new byte[this.Length];
            Buffer.BlockCopy(array, 0, array, 0, this.Length);
        }

        [Benchmark]
        public unsafe void Native()
        {
            var native = Marshal.AllocHGlobal(this.Length);

            fixed (void* dataPtr = this.data)
            {
                Buffer.MemoryCopy(dataPtr, (void*)native, this.Length, this.Length);
            }

            Marshal.FreeHGlobal(native);
        }

        [Benchmark]
        public void StackAlloc()
        {
            Span<byte> data = stackalloc byte[this.Length];
            this.data.AsSpan().CopyTo(data);
        }

        [Benchmark]
        [LocalsInit(false)]
        public void StackAllocWithoutLocalsInit()
        {
            Span<byte> data = stackalloc byte[this.Length];
            this.data.AsSpan().CopyTo(data);
        }

        [Benchmark]
        public void ArrayPool()
        {
            var array = ArrayPool<byte>.Shared.Rent(this.Length);

            Buffer.BlockCopy(this.data, 0, array, 0, this.Length);

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

        [Benchmark(Baseline = true)]
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

        [Benchmark()]
        public void Fill()
        {
            this.data.AsSpan().Fill(1);
        }
    }
}
