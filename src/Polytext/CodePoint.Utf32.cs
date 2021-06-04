using System;

namespace Poly
{
    public readonly partial struct CodePoint
    {
        public readonly struct Utf32
        {
            public const int BytesRequired = 4;

            private readonly CodePoint codepoint;

            public Utf32(CodePoint codepoint)
            {
                this.codepoint = codepoint;
            }

            public BytesWriteResult WriteLittleEndian(Span<byte> destination)
            {
                if (destination.Length < BytesRequired)
                {
                    return new BytesWriteResult(BytesRequired, 0);
                }

                unchecked
                {
                    destination[0] = (byte) (codepoint.Value & 0xFF);
                    destination[1] = (byte) ((codepoint.Value >> 8) & 0xFF);
                    destination[2] = (byte) ((codepoint.Value >> 16) & 0xFF);
                    destination[3] = (byte) ((codepoint.Value >> 24) & 0xFF);
                }

                return new BytesWriteResult(BytesRequired, BytesRequired);
            }

            public byte[] LittleEndianBytes()
            {
                var result = new byte[BytesRequired];
                WriteLittleEndian(result);
                return result;
            }

            public BytesWriteResult WriteBigEndian(Span<byte> destination)
            {
                if (destination.Length < BytesRequired)
                {
                    return new BytesWriteResult(BytesRequired, 0);
                }

                unchecked
                {
                    destination[0] = (byte) ((codepoint.Value >> 24) & 0xFF);
                    destination[1] = (byte) ((codepoint.Value >> 16) & 0xFF);
                    destination[2] = (byte) ((codepoint.Value >> 8) & 0xFF);
                    destination[3] = (byte) (codepoint.Value & 0xFF);
                }

                return new BytesWriteResult(BytesRequired, BytesRequired);
            }

            public byte[] BigEndianBytes()
            {
                var result = new byte[BytesRequired];
                WriteBigEndian(result);
                return result;
            }
        }
    }
}