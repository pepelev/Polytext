using System;

namespace Poly
{
    public readonly partial struct CodePoint : IEquatable<CodePoint>, IComparable<CodePoint>
    {
        private const ushort TenBitsMask = 0b0011_1111_1111;
        private const int MaximalCode = 0x10FFFF;
        private const int SurrogatesRangeStart = 0xD800;
        private const int SurrogatesRangeEnd = 0xDFFF;

        public static CodePoint Minimum => new(0);
        public static CodePoint Maximum => new(MaximalCode);

        public CodePoint(uint value)
        {
            if (MaximalCode < value || SurrogatesRangeStart <= value && value <= SurrogatesRangeEnd)
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

        #region utf16

        public Utf16WriteResult WriteUtf16(Span<char> destination)
        {
            if (Value <= 0xFFFF)
            {
                if (destination.Length < 1)
                {
                    return new Utf16WriteResult(1, 0);
                }

                destination[0] = unchecked((char)Value);
                return new Utf16WriteResult(1, 1);
            }

            if (destination.Length < 2)
            {
                return new Utf16WriteResult(2, 0);
            }

            var content = Value - 0x10000;
            var high = (content >> 10) & TenBitsMask;
            var low = content & TenBitsMask;
            destination[0] = unchecked((char)(SurrogatesRangeStart + high));
            destination[1] = unchecked((char)(0xDC00 + low));
            return new Utf16WriteResult(2, 2);
        }

        public char[] AsChars()
        {
            var writeResult = WriteUtf16(Span<char>.Empty);
            var result = new char[writeResult.CodeUnitsRequired];
            WriteUtf16(result);
            return result;
        }

        public BytesWriteResult WriteUtf16LittleEndian(Span<byte> destination)
        {
            if (Value <= 0xFFFF)
            {
                if (destination.Length < 2)
                {
                    return new BytesWriteResult(2, 0);
                }

                destination[0] = unchecked((byte)(Value & 0xFF));
                destination[1] = unchecked((byte)(Value >> 8));
                return new BytesWriteResult(2, 2);
            }

            if (destination.Length < 4)
            {
                return new BytesWriteResult(4, 0);
            }

            var content = Value - 0x10000;
            var high = SurrogatesRangeStart + ((content >> 10) & TenBitsMask);
            var low = 0xDC00 + (content & TenBitsMask);
            destination[0] = unchecked((byte)(high & 0xFF));
            destination[1] = unchecked((byte)(high >> 8));
            destination[2] = unchecked((byte)(low & 0xFF));
            destination[3] = unchecked((byte)(low >> 8));
            return new BytesWriteResult(4, 4);
        }

        public byte[] AsUtf16LittleEndianBytes()
        {
            var writeResult = WriteUtf16LittleEndian(Span<byte>.Empty);
            var result = new byte[writeResult.BytesRequired];
            WriteUtf16LittleEndian(result);
            return result;
        }

        public BytesWriteResult WriteUtf16BigEndian(Span<byte> destination)
        {
            if (Value <= 0xFFFF)
            {
                if (destination.Length < 2)
                {
                    return new BytesWriteResult(2, 0);
                }

                destination[0] = unchecked((byte)(Value >> 8));
                destination[1] = unchecked((byte)(Value & 0xFF));
                return new BytesWriteResult(2, 2);
            }

            if (destination.Length < 4)
            {
                return new BytesWriteResult(4, 0);
            }

            var content = Value - 0x10000;
            var high = SurrogatesRangeStart + ((content >> 10) & TenBitsMask);
            var low = 0xDC00 + (content & TenBitsMask);
            destination[0] = unchecked((byte)(high >> 8));
            destination[1] = unchecked((byte)(high & 0xFF));
            destination[2] = unchecked((byte)(low >> 8));
            destination[3] = unchecked((byte)(low & 0xFF));
            return new BytesWriteResult(4, 4);
        }

        public byte[] AsUtf16BigEndianBytes()
        {
            var writeResult = WriteUtf16BigEndian(Span<byte>.Empty);
            var result = new byte[writeResult.BytesRequired];
            WriteUtf16BigEndian(result);
            return result;
        }

        #endregion

        #region utf32

        public BytesWriteResult WriteUtf32LittleEndian(Span<byte> destination)
        {
            if (destination.Length < 4)
            {
                return new BytesWriteResult(4, 0);
            }

            unchecked
            {
                destination[0] = (byte)(Value & 0xFF);
                destination[1] = (byte)((Value >> 8) & 0xFF);
                destination[2] = (byte)((Value >> 16) & 0xFF);
                destination[3] = (byte)((Value >> 24) & 0xFF);
            }

            return new BytesWriteResult(4, 4);
        }

        public byte[] AsUtf32LittleEndianBytes()
        {
            var writeResult = WriteUtf32LittleEndian(Span<byte>.Empty);
            var result = new byte[writeResult.BytesRequired];
            WriteUtf32LittleEndian(result);
            return result;
        }

        public BytesWriteResult WriteUtf32BigEndian(Span<byte> destination)
        {
            if (destination.Length < 4)
            {
                return new BytesWriteResult(4, 0);
            }

            unchecked
            {
                destination[0] = (byte)((Value >> 24) & 0xFF);
                destination[1] = (byte)((Value >> 16) & 0xFF);
                destination[2] = (byte)((Value >> 8) & 0xFF);
                destination[3] = (byte)(Value & 0xFF);
            }

            return new BytesWriteResult(4, 4);
        }

        public byte[] AsUtf32BigEndianBytes()
        {
            var writeResult = WriteUtf32BigEndian(Span<byte>.Empty);
            var result = new byte[writeResult.BytesRequired];
            WriteUtf32BigEndian(result);
            return result;
        }

        #endregion
    }
}