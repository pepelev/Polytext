using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Poly.Tests
{
    public sealed class CodepointShould
    {
        private static readonly UTF32Encoding utf32LittleEndian = new(false, false, true);
        private static readonly UTF32Encoding utf32BigEndian = new(true, false, true);
        private static readonly UnicodeEncoding utf16LittleEndian = new(false, false, true);
        private static readonly UnicodeEncoding utf16BigEndian = new(true, false, true);

        public static IEnumerable<TestCaseData> WriteCases => Cases.StringCodePointsPairs.Select(
            @case => new TestCaseData(@case.Value.String).SetName(@case.Name)
        );

        [Test]
        [TestCaseSource(nameof(WriteCases))]
        public void WriteToCharSpan(string originalString)
        {
            var buffer = new char[originalString.Length * 2];
            var span = buffer.AsSpan();
            var @string = String.From(originalString);

            var length = 0;
            foreach (var codePoint in @string)
            {
                var result = codePoint.AsUtf16.Write(span[length..]);
                result.Completed.Should().BeTrue();

                length += result.CodeUnitsWritten;
            }

            var resultString = new string(span[..length]);
            resultString.Should().Be(originalString);
        }

        [Test]
        [TestCaseSource(nameof(WriteCases))]
        public void WriteUtf8(string originalString)
        {
            var buffer = new byte[originalString.Length * 8];
            var span = buffer.AsSpan();
            var @string = String.From(originalString);

            var length = 0;
            foreach (var codePoint in @string)
            {
                var result = codePoint.AsUtf8.Write(span[length..]);
                result.Completed.Should().BeTrue();

                length += result.BytesWritten;
            }

            var resultString = Encoding.UTF8.GetString(span[..length]);
            resultString.Should().Be(originalString);
        }

        [Test]
        [TestCaseSource(nameof(WriteCases))]
        public void WriteUtf16LittleEndian(string originalString)
        {
            var buffer = new byte[originalString.Length * 8];
            var span = buffer.AsSpan();
            var @string = String.From(originalString);

            var length = 0;
            foreach (var codePoint in @string)
            {
                var result = codePoint.AsUtf16.WriteLittleEndian(span[length..]);
                result.Completed.Should().BeTrue();

                length += result.BytesWritten;
            }

            var resultString = utf16LittleEndian.GetString(span[..length]);
            resultString.Should().Be(originalString);
        }

        [Test]
        [TestCaseSource(nameof(WriteCases))]
        public void WriteUtf16BigEndian(string originalString)
        {
            var buffer = new byte[originalString.Length * 8];
            var span = buffer.AsSpan();
            var @string = String.From(originalString);

            var length = 0;
            foreach (var codePoint in @string)
            {
                var result = codePoint.AsUtf16.WriteBigEndian(span[length..]);
                result.Completed.Should().BeTrue();

                length += result.BytesWritten;
            }

            var resultString = utf16BigEndian.GetString(span[..length]);
            resultString.Should().Be(originalString);
        }

        [Test]
        [TestCaseSource(nameof(WriteCases))]
        public void WriteUtf32LittleEndian(string originalString)
        {
            var buffer = new byte[originalString.Length * 8];
            var span = buffer.AsSpan();
            var @string = String.From(originalString);

            var length = 0;
            foreach (var codePoint in @string)
            {
                var result = codePoint.AsUtf32.WriteLittleEndian(span[length..]);
                result.Completed.Should().BeTrue();

                length += result.BytesWritten;
            }

            var resultString = utf32LittleEndian.GetString(span[..length]);
            resultString.Should().Be(originalString);
        }

        [Test]
        [TestCaseSource(nameof(WriteCases))]
        public void WriteUtf32BigEndian(string originalString)
        {
            var buffer = new byte[originalString.Length * 8];
            var span = buffer.AsSpan();
            var @string = String.From(originalString);

            var length = 0;
            foreach (var codePoint in @string)
            {
                var result = codePoint.AsUtf32.WriteBigEndian(span[length..]);
                result.Completed.Should().BeTrue();

                length += result.BytesWritten;
            }

            var resultString = utf32BigEndian.GetString(span[..length]);
            resultString.Should().Be(originalString);
        }
    }
}