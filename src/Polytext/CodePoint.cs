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
    }
}