namespace Poly.Tests
{
    public readonly struct Named<T>
    {
        public string Name { get; }
        public T Value { get; }

        public Named(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}