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