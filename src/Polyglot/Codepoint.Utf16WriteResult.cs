namespace Poly
{
    public readonly partial struct CodePoint
    {
        public readonly struct Utf16WriteResult
        {
            public Utf16WriteResult(byte codeUnitsRequired, byte codeUnitsWritten)
            {
                CodeUnitsRequired = codeUnitsRequired;
                CodeUnitsWritten = codeUnitsWritten;
            }

            public byte CodeUnitsRequired { get; }
            public byte CodeUnitsWritten { get; }
            public bool Completed => CodeUnitsRequired == CodeUnitsWritten;
        }
    }
}