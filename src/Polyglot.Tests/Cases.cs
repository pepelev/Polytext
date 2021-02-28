using System;

namespace Poly.Tests
{
    public static class Cases
    {
        public static Named<(string String, CodePoint[] CodePoints)>[] StringCodePointsPairs => new[]
        {
            ("", Array.Empty<CodePoint>()).Named("Empty string"),
            (
                "hello",
                new CodePoint[]
                {
                    new('h'),
                    new('e'),
                    new('l'),
                    new('l'),
                    new('o')
                }
            ).Named("Simple ascii string"),
            (
                "Съешь ещё этих мягких французских булок!",
                new CodePoint[]
                {
                    new('С'),
                    new('ъ'),
                    new('е'),
                    new('ш'),
                    new('ь'),
                    new(' '),
                    new('е'),
                    new('щ'),
                    new('ё'),
                    new(' '),
                    new('э'),
                    new('т'),
                    new('и'),
                    new('х'),
                    new(' '),
                    new('м'),
                    new('я'),
                    new('г'),
                    new('к'),
                    new('и'),
                    new('х'),
                    new(' '),
                    new('ф'),
                    new('р'),
                    new('а'),
                    new('н'),
                    new('ц'),
                    new('у'),
                    new('з'),
                    new('с'),
                    new('к'),
                    new('и'),
                    new('х'),
                    new(' '),
                    new('б'),
                    new('у'),
                    new('л'),
                    new('о'),
                    new('к'),
                    new('!')
                }
            ).Named("Russian letters (2 byte per letter in utf-8)"),
            (
                "ﭐﭑﭒﭓﭔ",
                new CodePoint[]
                {
                    new("ﭐ", 0),
                    new("ﭑ", 0),
                    new("ﭒ", 0),
                    new("ﭓ", 0),
                    new("ﭔ", 0)
                }
            ).Named("Arabic letters (3 byte per letter in utf-8)"),
            ("🌟", new[] {new CodePoint("🌟", 0)}).Named("Single surrogate"),
            (
                "🌟🌟",
                new CodePoint[]
                {
                    new("🌟", 0),
                    new("🌟", 0)
                }
            ).Named("Two surrogates"),
            (
                " 🌟🌟s",
                new CodePoint[]
                {
                    new(' '),
                    new("🌟", 0),
                    new("🌟", 0),
                    new('s')
                }
            ).Named("Surrogate and regular chars"),
            (
                "🌟 cat 🌟",
                new CodePoint[]
                {
                    new("🌟", 0),
                    new(' '),
                    new('c'),
                    new('a'),
                    new('t'),
                    new(' '),
                    new("🌟", 0)
                }
            ).Named("String ends with surrogate")
        };
    }
}