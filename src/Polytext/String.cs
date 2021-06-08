using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Poly
{
    public abstract partial class String : IReadOnlyList<CodePoint>
    {
        public abstract IEnumerator<CodePoint> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public abstract int Count { get; }
        public abstract CodePoint this[int index] { get; }

        public static unsafe String From(string value)
        {
            List<int> surrogates = null;
            var i = 0;
            var codepointIndex = 0;

            if (Avx2.IsSupported)
            {
                var xor = (ushort) 0xD800;
                var mask = (ushort) 0xF800;
                var xorVector = Avx2.BroadcastScalarToVector256(&xor);
                var maskVector = Avx2.BroadcastScalarToVector256(&mask);
                fixed (char* str = value)
                {
                    var step = Vector256<ushort>.Count;
                    while (i + step <= value.Length)
                    {
                        var chars = Avx.LoadVector256((ushort*)(str + i));
                        var masked = Avx2.And(chars, maskVector);
                        var equality = Avx2.CompareEqual(masked, xorVector);
                        var msk = Avx2.MoveMask(equality.As<ushort, byte>());
                        var surrogate = msk != 0;
                        if (!surrogate)
                        {
                            i += step;
                            codepointIndex += step;
                        }
                        else
                        {
                            var border = i + step;
                            while (i < border)
                            {
                                Parse();
                            }
                        }
                    }
                }
            }

            while (i < value.Length)
            {
                Parse();
            }

            return surrogates is null
                ? new SurrogateFreeRegular(value)
                : new Regular(value, surrogates);

            void Parse()
            {
                if (char.IsHighSurrogate(value[i]))
                {
                    if (i + 1 >= value.Length)
                    {
                        throw new ArgumentException(
                            "Value is malformed - it ends with a high surrogate.",
                            nameof(value)
                        );
                    }

                    if (!char.IsLowSurrogate(value[i + 1]))
                    {
                        throw new ArgumentException(
                            $"Value is malformed - a high surrogate at [{i}] not followed by a low surrogate.",
                            nameof(value)
                        );
                    }

                    surrogates ??= new List<int>();
                    surrogates.Add(codepointIndex);
                    i += 2;
                }
                else if (char.IsLowSurrogate(value, i))
                {
                    throw new ArgumentException(
                        $"Value is malformed - a low surrogate at [{i}] not following a high surrogate.",
                        nameof(value)
                    );
                }
                else
                {
                    i++;
                }

                codepointIndex++;
            }
        }
    }
}