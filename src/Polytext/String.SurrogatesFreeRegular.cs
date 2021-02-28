using System.Collections.Generic;
using System.Linq;

namespace Poly
{
    public abstract partial class String
    {
        private sealed class SurrogateFreeRegular : String
        {
            private readonly string content;

            public SurrogateFreeRegular(string content)
            {
                this.content = content;
            }

            public override IEnumerator<CodePoint> GetEnumerator() =>
                content.Select(static @char => new CodePoint(@char)).GetEnumerator();

            public override int Count => content.Length;
            public override CodePoint this[int index] => new(content[index]);
            public override string ToString() => content;
        }
    }
}