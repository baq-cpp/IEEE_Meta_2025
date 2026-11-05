// Holds the signed-in student's name and Bronco ID in memory (RAM) for the
// whole app run. Because it is static, it survives scene loads, so the lab
// scenes and the metrics code can read UserSession.Name / UserSession.StudentId.
// Nothing is saved to disk here; saving to JSON or the database is a later step.
public static class UserSession
{
    public static string Name { get; private set; } = string.Empty;
    public static string StudentId { get; private set; } = string.Empty;

    public static bool HasUser => !string.IsNullOrEmpty(StudentId);

    // Raised after SetUser so other scripts can react (for example metrics).
    public static event System.Action UserChanged;

    public static void SetUser(string name, string studentId)
    {
        Name = name ?? string.Empty;
        StudentId = studentId ?? string.Empty;
        UserChanged?.Invoke();
    }

    public static void Clear()
    {
        Name = string.Empty;
        StudentId = string.Empty;
        UserChanged?.Invoke();
    }
}
