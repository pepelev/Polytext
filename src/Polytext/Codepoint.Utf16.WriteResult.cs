namespace Poly
{
    public readonly partial struct CodePoint
    {
        public readonly partial struct Utf16
        {
            public readonly struct WriteResult
            {
                public WriteResult(byte codeUnitsRequired, byte codeUnitsWritten)
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
}