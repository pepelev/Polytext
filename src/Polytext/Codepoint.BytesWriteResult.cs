namespace Poly
{
    public readonly partial struct CodePoint
    {
        public readonly struct BytesWriteResult
        {
            public BytesWriteResult(byte bytesRequired, byte bytesWritten)
            {
                BytesRequired = bytesRequired;
                BytesWritten = bytesWritten;
            }

            public byte BytesRequired { get; }
            public byte BytesWritten { get; }
            public bool Completed => BytesRequired == BytesWritten;
        }
    }
}