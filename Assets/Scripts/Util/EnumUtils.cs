public static class EnumUtils {
    // from https://stackoverflow.com/questions/972307/how-to-loop-through-all-enum-values-in-c
    public static IEnumerable<T> GetValues<T>() {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }
}
