using System.Diagnostics.CodeAnalysis;

namespace Shared.Commons
{
    public static class ObjectExtensions
    {
#pragma warning disable CS8777 // Параметр должен иметь значение, отличное от NULL, при выходе.
        public static bool IsNull<T>([NotNull] this T? o) => o is null;
        public static bool IsNotNull<T>([NotNull] this T? o) => o is not null;
#pragma warning restore CS8777 // Параметр должен иметь значение, отличное от NULL, при выходе.
    }
}
