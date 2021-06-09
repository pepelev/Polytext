using System;
using System.Collections.Generic;

namespace Poly
{
    public sealed class PlainString : String
    {
        private readonly IReadOnlyList<CodePoint> points;

        public PlainString(IReadOnlyList<CodePoint> points)
        {
            this.points = points ?? throw new ArgumentNullException(nameof(points));
        }

        public override IEnumerator<CodePoint> GetEnumerator() => points.GetEnumerator();
        public override int Count => points.Count;
        public override CodePoint this[int index] => points[index];
    }
}