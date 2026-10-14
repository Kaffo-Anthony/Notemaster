namespace Notemaster.Models;

public record QuizQuestion(string Text, List<string> Options, int CorrectIndex, string Explanation);