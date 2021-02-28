using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Poly.Tests
{
    public sealed class StringShould
    {
        public static IEnumerable<TestCaseData> ExposeCases => Cases.StringCodePointsPairs.Select(
            @case => new TestCaseData(@case.Value.String, @case.Value.CodePoints).SetName(@case.Name)
        );

        [Test]
        [TestCaseSource(nameof(ExposeCases))]
        public void ExposeCodePoints(string source, CodePoint[] expectation)
        {
            var @string = String.From(source);
            Assert(@string, expectation);
        }

        private static void Assert(String subject, params CodePoint[] codepoint)
        {
            AssertList(subject);
            AssertList(subject.ToList());

            void AssertList(IReadOnlyCollection<CodePoint> list)
            {
                list.Count.Should().Be(codepoint.Length);
                for (var i = 0; i < list.Count; i++)
                {
                    subject[i].Should().Be(codepoint[i]);
                }
            }
        }
    }
}