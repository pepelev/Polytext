using System.Collections.Generic;

namespace Poly
{
    public abstract partial class String
    {
        private sealed class Regular : String
        {
            private readonly string content;
            private readonly List<int> surrogates;

            public Regular(string content, List<int> surrogates)
            {
                this.content = content;
                this.surrogates = surrogates;
            }

            public override int Count => content.Length - surrogates.Count;

            public override CodePoint this[int index] => new(
                content,
                index + SurrogatesBefore(index)
            );

            public override IEnumerator<CodePoint> GetEnumerator()
            {
                for (var i = 0; i < content.Length;)
                {
                    var point = new CodePoint(content, i);
                    yield return point;
                    i += point.AsUtf16.CodeUnitsRequired;
                }
            }

            private int SurrogatesBefore(int index) => surrogates.BinarySearch(index) switch
            {
                { } found and >= 0 => found,
                { } notFound => ~notFound
            };

            public override string ToString() => content;
        }
    }
}