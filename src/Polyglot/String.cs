using System.Collections;
using System.Collections.Generic;

namespace Poly
{
    public abstract partial class String : IReadOnlyList<CodePoint>
    {
        public abstract IEnumerator<CodePoint> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public abstract int Count { get; }
        public abstract CodePoint this[int index] { get; }

        public static String From(string value)
        {
            List<int> surrogates = null;
            var i = 0;
            var codepointIndex = 0;

            while (i < value.Length)
            {
                if (char.IsHighSurrogate(value, i))
                {
                    surrogates ??= new List<int>();
                    surrogates.Add(codepointIndex);
                    i += 2;
                }
                else
                {
                    i++;
                }

                codepointIndex++;
            }

            return surrogates is not null
                ? new Regular(value, surrogates)
                : new SurrogateFreeRegular(value);
        }
    }
}