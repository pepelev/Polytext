namespace Poly.Tests
{
    public static class Extensions
    {
        public static Named<T> Named<T>(this T value, string name) => new Named<T>(name, value);
    }
}