namespace ToDoListWebApplication.Models;

internal static class Defaults
{
    public static T[] Array<T>()
    {
        return [];
    }

    public static string String()
    {
        return string.Empty;
    }

    public static T Complex<T>()
        where T : IDefaultConstructible<T>
    {
        return T.Default;
    }
}