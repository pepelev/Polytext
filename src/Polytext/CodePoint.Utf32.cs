using System;

namespace Poly
{
    public readonly partial struct CodePoint
    {
        public readonly struct Utf32
        {
            private readonly CodePoint codepoint;

            public Utf32(CodePoint codepoint)
            {
                this.codepoint = codepoint;
            }

            public BytesWriteResult WriteLittleEndian(Span<byte> destination)
            {
                if (destination.Length < 4)
                {
                    return new BytesWriteResult(4, 0);
                }

                unchecked
                {
                    destination[0] = (byte) (codepoint.Value & 0xFF);
                    destination[1] = (byte) ((codepoint.Value >> 8) & 0xFF);
                    destination[2] = (byte) ((codepoint.Value >> 16) & 0xFF);
                    destination[3] = (byte) ((codepoint.Value >> 24) & 0xFF);
                }

                return new BytesWriteResult(4, 4);
            }

            public byte[] LittleEndianBytes()
            {
                var writeResult = WriteLittleEndian(Span<byte>.Empty);
                var result = new byte[writeResult.BytesRequired];
                WriteLittleEndian(result);
                return result;
            }

            public BytesWriteResult WriteBigEndian(Span<byte> destination)
            {
                if (destination.Length < 4)
                {
                    return new BytesWriteResult(4, 0);
                }

                unchecked
                {
                    destination[0] = (byte) ((codepoint.Value >> 24) & 0xFF);
                    destination[1] = (byte) ((codepoint.Value >> 16) & 0xFF);
                    destination[2] = (byte) ((codepoint.Value >> 8) & 0xFF);
                    destination[3] = (byte) (codepoint.Value & 0xFF);
                }

                return new BytesWriteResult(4, 4);
            }

            public byte[] BigEndianBytes()
            {
                var writeResult = WriteBigEndian(Span<byte>.Empty);
                var result = new byte[writeResult.BytesRequired];
                WriteBigEndian(result);
                return result;
            }
        }
    }
}