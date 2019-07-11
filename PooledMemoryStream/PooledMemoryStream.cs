using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;

namespace BetterStreams
{
    public sealed class PooledMemoryStream : Stream
    {
        private const float overexpansionFactor = 2;

        private byte[] data;
        private int length;
        private ArrayPool<byte> pool;
        private bool isDisposed;

        public PooledMemoryStream()
            : this(ArrayPool<byte>.Shared)
        {
        }

        public PooledMemoryStream(ArrayPool<byte> arrayPool, int capacity = 0)
        {
            this.pool = arrayPool ?? throw new ArgumentNullException(nameof(arrayPool));
            if (capacity > 0)
            {
                this.data = this.pool.Rent(capacity);
            }  
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length { get => this.length; }

        public override long Position { get; set; }

        public long Capacity => this.data?.Length ?? 0;
#if NETCOREAPP || NET452
        public Span<byte> GetSpan()
        {
            return this.data.AsSpan(0, this.length);
        }

        public Memory<byte> GetMemory()
        {
            return this.data.AsMemory(0, this.length);
        }
#endif
        public override void Flush()
        {
            this.AssertNotDisposed();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.AssertNotDisposed();

            if (count == 0)
            {
                return 0;
            }

            var available = Math.Min(count, this.Length - this.Position);
            Array.Copy(this.data, this.Position, buffer, offset, available);
            this.Position += available;
            return (int)available;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override long Seek(long offset, SeekOrigin origin)
        {
            this.AssertNotDisposed();

            switch (origin)
            {
                case SeekOrigin.Current: 
                    if (this.Position + offset < 0 || this.Position + offset > this.Capacity)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset));
                    }
                    this.Position += offset;
                    this.length = (int)Math.Max(this.Position, this.length);
                    return this.Position;

                case SeekOrigin.Begin:
                    if (offset < 0 || offset > this.Capacity)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset));
                    }

                    this.Position = offset;
                    this.length = (int)Math.Max(this.Position, this.length);
                    return this.Position;

                case SeekOrigin.End:
                    if (this.Length + offset < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset));
                    }

                    if (this.Length + offset > this.Capacity)
                    {
                        this.SetCapacity((int)(this.Length + offset));
                    }

                    this.Position = this.Length + offset;
                    this.length = (int)Math.Max(this.Position, this.length);
                    return this.Position;

                default:
                    throw new ArgumentOutOfRangeException(nameof(origin));
            }
        }

        public override void SetLength(long value)
        {
            this.AssertNotDisposed();

            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (value > this.Capacity)
            {
                this.SetCapacity((int)value);
            }

            this.length = (int)value;
            if (this.Position > this.Length)
            {
                this.Position = this.Length;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.AssertNotDisposed();

            if (count == 0)
            {
                return;
            }

            if (this.Capacity - this.Position < count)
            {
                this.SetCapacity((int)(overexpansionFactor * (this.Position + count)));
            }

            Buffer.BlockCopy(buffer, offset, this.data, (int)this.Position, count);

            this.Position += count;
            this.length = (int)Math.Max(this.Position, this.length);
        }

        public void WriteTo(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            AssertNotDisposed();

            stream.Write(this.data, 0, (int)this.Length);
        }

#if NETCOREAPP
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.isDisposed = true;
                this.Position = 0;
                this.length = 0;

                if (this.data != null)
                {
                    this.pool.Return(this.data);
                    this.data = null;
                }
            }

            base.Dispose(disposing);
        }

        private void SetCapacity(int newCapacity)
        {
            var newData = this.pool.Rent(newCapacity);

            if (this.data != null)
            {
                Buffer.BlockCopy(this.data, 0, newData, 0, (int)this.Position);
                this.pool.Return(data);
            }

            this.data = newData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssertNotDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(nameof(PooledMemoryStream));
            }
        }
    }
}
