using System;

namespace Poly
{
    public readonly partial struct CodePoint
    {
        public readonly partial struct Utf16
        {
            private readonly CodePoint codepoint;

            public Utf16(CodePoint codepoint)
            {
                this.codepoint = codepoint;
            }

            public byte CodeUnitsRequired => Write(Span<char>.Empty).CodeUnitsRequired;
            public byte BytesRequired => WriteLittleEndian(Span<byte>.Empty).BytesRequired;

            public WriteResult Write(Span<char> destination)
            {
                if (codepoint.Value <= 0xFFFF)
                {
                    if (destination.Length < 1)
                    {
                        return new WriteResult(1, 0);
                    }

                    destination[0] = unchecked((char) codepoint.Value);
                    return new WriteResult(1, 1);
                }

                if (destination.Length < 2)
                {
                    return new WriteResult(2, 0);
                }

                var content = codepoint.Value - 0x10000;
                var high = (content >> 10) & TenBitsMask;
                var low = content & TenBitsMask;
                destination[0] = unchecked((char) (SurrogatesRangeStart + high));
                destination[1] = unchecked((char) (0xDC00 + low));
                return new WriteResult(2, 2);
            }

            public char[] Chars()
            {
                var result = new char[CodeUnitsRequired];
                Write(result);
                return result;
            }

            public BytesWriteResult WriteLittleEndian(Span<byte> destination)
            {
                if (codepoint.Value <= 0xFFFF)
                {
                    if (destination.Length < 2)
                    {
                        return new BytesWriteResult(2, 0);
                    }

                    destination[0] = unchecked((byte) (codepoint.Value & 0xFF));
                    destination[1] = unchecked((byte) (codepoint.Value >> 8));
                    return new BytesWriteResult(2, 2);
                }

                if (destination.Length < 4)
                {
                    return new BytesWriteResult(4, 0);
                }

                var content = codepoint.Value - 0x10000;
                var high = SurrogatesRangeStart + ((content >> 10) & TenBitsMask);
                var low = 0xDC00 + (content & TenBitsMask);
                destination[0] = unchecked((byte) (high & 0xFF));
                destination[1] = unchecked((byte) (high >> 8));
                destination[2] = unchecked((byte) (low & 0xFF));
                destination[3] = unchecked((byte) (low >> 8));
                return new BytesWriteResult(4, 4);
            }

            public byte[] LittleEndianBytes()
            {
                var result = new byte[BytesRequired];
                WriteLittleEndian(result);
                return result;
            }

            public BytesWriteResult WriteBigEndian(Span<byte> destination)
            {
                if (codepoint.Value <= 0xFFFF)
                {
                    if (destination.Length < 2)
                    {
                        return new BytesWriteResult(2, 0);
                    }

                    destination[0] = unchecked((byte) (codepoint.Value >> 8));
                    destination[1] = unchecked((byte) (codepoint.Value & 0xFF));
                    return new BytesWriteResult(2, 2);
                }

                if (destination.Length < 4)
                {
                    return new BytesWriteResult(4, 0);
                }

                var content = codepoint.Value - 0x10000;
                var high = SurrogatesRangeStart + ((content >> 10) & TenBitsMask);
                var low = 0xDC00 + (content & TenBitsMask);
                destination[0] = unchecked((byte) (high >> 8));
                destination[1] = unchecked((byte) (high & 0xFF));
                destination[2] = unchecked((byte) (low >> 8));
                destination[3] = unchecked((byte) (low & 0xFF));
                return new BytesWriteResult(4, 4);
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