using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;

namespace BetterStreams
{
    public sealed class PooledMemoryStream : Stream
    {
        private byte[] data;
        private ArrayPool<byte> pool;

        public PooledMemoryStream()
            : this(ArrayPool<byte>.Shared)
        {
        }

        public PooledMemoryStream(ArrayPool<byte> arrayPool)
        {
            this.pool = arrayPool ?? throw new ArgumentNullException(nameof(arrayPool));
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length { get => this.data?.Length ?? 0; }

        public override long Position { get; set; }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count) => throw new System.NotImplementedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Current: 
                    if (this.Position + offset < 0 || this.Position + offset > this.Length)
                    {
                        throw new ArgumentException(nameof(offset));
                    }
                    return this.Position += offset;

                case SeekOrigin.Begin:
                    if (offset < 0 || offset > this.Length)
                    {
                        throw new ArgumentException(nameof(offset));
                    }

                    return this.Position = offset;

                case SeekOrigin.End:
                    if (this.Length + offset < 0 || this.Length + offset > this.Length)
                    {
                        throw new ArgumentException(nameof(offset));
                    }
                    return this.Position = this.Length + offset;

                default:
                    throw new ArgumentException(nameof(origin));
            }
        }

        public override void SetLength(long value)
        {
            //if (value != this.Length)
            //{
            //    var newData = this.pool.Rent((int)this.Position + count);

            //}
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.Length - this.Position < count)
            {
                var newData = this.pool.Rent((int)this.Position + count);

                if (this.data != null)
                {
                    Buffer.BlockCopy(this.data, 0, newData, 0, (int)this.Position);
                    this.pool.Return(data);
                }

                this.data = newData;
            }

            Buffer.BlockCopy(buffer, offset, this.data, (int)this.Position, count);

            this.Position += count; 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.data != null)
                {
                    this.pool.Return(this.data);
                    this.data = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
