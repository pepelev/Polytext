using System;

namespace Poly
{
    public readonly partial struct CodePoint : IEquatable<CodePoint>, IComparable<CodePoint>
    {
        private const ushort TenBitsMask = 0b0011_1111_1111;
        public static CodePoint Minimum => new(0);
        public static CodePoint Maximum => new(0x10FFFF);

        public CodePoint(uint value)
        {
            if (value > 0x10FFFF || 0xD800 <= value && value <= 0xDFFF)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "Supported points from 0 to 0x10FFFF excluding surrogates range from 0xD800 to 0xDFFF"
                );
            }

            Value = value;
        }

        public CodePoint(string @string, int index)
            : this(unchecked((uint) char.ConvertToUtf32(@string, index)))
        {
        }

        public CodePoint(char @char)
            : this((uint) @char)
        {
        }

        public uint Value { get; }
        public override string ToString() => char.ConvertFromUtf32((int) Value);
        public bool Equals(CodePoint other) => Value == other.Value;
        public override bool Equals(object obj) => obj is CodePoint other && Equals(other);
        public override int GetHashCode() => unchecked((int) Value);
        public static bool operator ==(CodePoint left, CodePoint right) => left.Equals(right);
        public static bool operator !=(CodePoint left, CodePoint right) => !left.Equals(right);
        public int CompareTo(CodePoint other) => Value.CompareTo(other.Value);
        public static bool operator <(CodePoint left, CodePoint right) => left.CompareTo(right) < 0;
        public static bool operator >(CodePoint left, CodePoint right) => left.CompareTo(right) > 0;
        public static bool operator <=(CodePoint left, CodePoint right) => left.CompareTo(right) <= 0;
        public static bool operator >=(CodePoint left, CodePoint right) => left.CompareTo(right) >= 0;

        public BytesWriteResult WriteUtf8(Span<byte> destination)
        {
            if (Value <= 0x7F)
            {
                if (destination.Length < 1)
                {
                    return new BytesWriteResult(1, 0);
                }

                destination[0] = unchecked((byte) Value);
                return new BytesWriteResult(1, 1);
            }

            const int followingHeader = 0b1000_0000;
            const int followingMask = 0b0011_1111;
            if (Value <= 0x7FF)
            {
                if (destination.Length < 2)
                {
                    return new BytesWriteResult(2, 0);
                }

                var low = 0b1100_0000 | ((Value >> 6) & 0b0001_1111);
                var high = followingHeader | (Value & followingMask);
                destination[0] = unchecked((byte) low);
                destination[1] = unchecked((byte) high);
                return new BytesWriteResult(2, 2);
            }

            if (Value <= 0xFFFF)
            {
                if (destination.Length < 3)
                {
                    return new BytesWriteResult(3, 0);
                }

                var low = 0b1110_0000 | ((Value >> 12) & 0b0000_1111);
                var mid = followingHeader | ((Value >> 6) & followingMask);
                var high = followingHeader | (Value & followingMask);
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

                var low = 0b1111_0000 | ((Value >> 18) & 0b0000_0111);
                var mid = followingHeader | ((Value >> 12) & followingMask);
                var mid2 = followingHeader | ((Value >> 6) & followingMask);
                var high = followingHeader | (Value & followingMask);
                destination[0] = unchecked((byte) low);
                destination[1] = unchecked((byte) mid);
                destination[2] = unchecked((byte) mid2);
                destination[3] = unchecked((byte) high);
                return new BytesWriteResult(4, 4);
            }
        }

        public Utf16WriteResult WriteUtf16(Span<char> destination)
        {
            if (Value <= 0xFFFF)
            {
                if (destination.Length < 1)
                {
                    return new Utf16WriteResult(1, 0);
                }

                destination[0] = unchecked((char) Value);
                return new Utf16WriteResult(1, 1);
            }

            if (destination.Length < 2)
            {
                return new Utf16WriteResult(2, 0);
            }

            var content = Value - 0x10000;
            var high = (content >> 10) & TenBitsMask;
            var low = content & TenBitsMask;
            destination[0] = unchecked((char) (0xD800 + high));
            destination[1] = unchecked((char) (0xDC00 + low));
            return new Utf16WriteResult(2, 2);
        }

        public BytesWriteResult WriteUtf16LittleEndian(Span<byte> destination)
        {
            if (Value <= 0xFFFF)
            {
                if (destination.Length < 2)
                {
                    return new BytesWriteResult(2, 0);
                }

                destination[0] = unchecked((byte) (Value & 0xFF));
                destination[1] = unchecked((byte) (Value >> 8));
                return new BytesWriteResult(2, 2);
            }

            if (destination.Length < 4)
            {
                return new BytesWriteResult(4, 0);
            }

            var content = Value - 0x10000;
            var high = 0xD800 + ((content >> 10) & TenBitsMask);
            var low = 0xDC00 + (content & TenBitsMask);
            destination[0] = unchecked((byte) (high & 0xFF));
            destination[1] = unchecked((byte) (high >> 8));
            destination[2] = unchecked((byte) (low & 0xFF));
            destination[3] = unchecked((byte) (low >> 8));
            return new BytesWriteResult(4, 4);
        }

        public BytesWriteResult WriteUtf16BigEndian(Span<byte> destination)
        {
            if (Value <= 0xFFFF)
            {
                if (destination.Length < 2)
                {
                    return new BytesWriteResult(2, 0);
                }

                destination[0] = unchecked((byte) (Value >> 8));
                destination[1] = unchecked((byte) (Value & 0xFF));
                return new BytesWriteResult(2, 2);
            }

            if (destination.Length < 4)
            {
                return new BytesWriteResult(4, 0);
            }

            var content = Value - 0x10000;
            var high = 0xD800 + ((content >> 10) & TenBitsMask);
            var low = 0xDC00 + (content & TenBitsMask);
            destination[0] = unchecked((byte) (high >> 8));
            destination[1] = unchecked((byte) (high & 0xFF));
            destination[2] = unchecked((byte) (low >> 8));
            destination[3] = unchecked((byte) (low & 0xFF));
            return new BytesWriteResult(4, 4);
        }

        public BytesWriteResult WriteUtf32LittleEndian(Span<byte> destination)
        {
            if (destination.Length < 4)
            {
                return new BytesWriteResult(4, 0);
            }

            unchecked
            {
                destination[0] = (byte) (Value & 0xFF);
                destination[1] = (byte) ((Value >> 8) & 0xFF);
                destination[2] = (byte) ((Value >> 16) & 0xFF);
                destination[3] = (byte) ((Value >> 24) & 0xFF);
            }

            return new BytesWriteResult(4, 4);
        }

        public BytesWriteResult WriteUtf32BigEndian(Span<byte> destination)
        {
            if (destination.Length < 4)
            {
                return new BytesWriteResult(4, 0);
            }

            unchecked
            {
                destination[0] = (byte) ((Value >> 24) & 0xFF);
                destination[1] = (byte) ((Value >> 16) & 0xFF);
                destination[2] = (byte) ((Value >> 8) & 0xFF);
                destination[3] = (byte) (Value & 0xFF);
            }

            return new BytesWriteResult(4, 4);
        }
    }
}