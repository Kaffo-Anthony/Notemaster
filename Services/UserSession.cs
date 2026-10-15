namespace Notemaster.Services;

public static class UserSession
{
    public static string Name
    {
        get => Preferences.Get("user_name", "Étudiant");
        set => Preferences.Set("user_name", value);
    }

    public static string Email
    {
        get => Preferences.Get("user_email", "");
        set => Preferences.Set("user_email", value);
    }

    public static string School
    {
        get => Preferences.Get("user_school", "");
        set => Preferences.Set("user_school", value);
    }

    public static int QuizzesDone
    {
        get => Preferences.Get("quizzes_done", 0);
        set => Preferences.Set("quizzes_done", value);
    }
}