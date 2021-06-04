using System;

namespace Poly
{
    public readonly partial struct CodePoint
    {
        public readonly struct Utf8
        {
            private readonly CodePoint codepoint;

            public Utf8(CodePoint codepoint)
            {
                this.codepoint = codepoint;
            }

            public byte BytesRequired => Write(Span<byte>.Empty).BytesRequired;

            public BytesWriteResult Write(Span<byte> destination)
            {
                if (codepoint.Value <= 0x7F)
                {
                    if (destination.Length < 1)
                    {
                        return new BytesWriteResult(1, 0);
                    }

                    destination[0] = unchecked((byte) codepoint.Value);
                    return new BytesWriteResult(1, 1);
                }

                const int followingHeader = 0b1000_0000;
                const int followingMask = 0b0011_1111;
                if (codepoint.Value <= 0x7FF)
                {
                    if (destination.Length < 2)
                    {
                        return new BytesWriteResult(2, 0);
                    }

                    var low = 0b1100_0000 | ((codepoint.Value >> 6) & 0b0001_1111);
                    var high = followingHeader | (codepoint.Value & followingMask);
                    destination[0] = unchecked((byte) low);
                    destination[1] = unchecked((byte) high);
                    return new BytesWriteResult(2, 2);
                }

                if (codepoint.Value <= 0xFFFF)
                {
                    if (destination.Length < 3)
                    {
                        return new BytesWriteResult(3, 0);
                    }

                    var low = 0b1110_0000 | ((codepoint.Value >> 12) & 0b0000_1111);
                    var mid = followingHeader | ((codepoint.Value >> 6) & followingMask);
                    var high = followingHeader | (codepoint.Value & followingMask);
                    destination[0] = unchecked((byte) low);
                    destination[1] = unchecked((byte) mid);
                    destination[2] = unchecked((byte) high);
                    return new BytesWriteResult(3, 3);
                }
                else
                {
                    if (destination.Length < 4)
                    {
                        return new BytesWriteResult(4, 0);
                    }

                    var low = 0b1111_0000 | ((codepoint.Value >> 18) & 0b0000_0111);
                    var mid = followingHeader | ((codepoint.Value >> 12) & followingMask);
                    var mid2 = followingHeader | ((codepoint.Value >> 6) & followingMask);
                    var high = followingHeader | (codepoint.Value & followingMask);
                    destination[0] = unchecked((byte) low);
                    destination[1] = unchecked((byte) mid);
                    destination[2] = unchecked((byte) mid2);
                    destination[3] = unchecked((byte) high);
                    return new BytesWriteResult(4, 4);
                }
            }

            public byte[] Bytes()
            {
                var result = new byte[BytesRequired];
                Write(result);
                return result;
            }
        }
    }
}