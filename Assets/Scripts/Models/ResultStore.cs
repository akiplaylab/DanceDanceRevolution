public static class ResultStore
{
    public static JudgementSummary Summary { get; set; }
    public static bool HasSummary { get; set; }

    public static void Clear()
    {
        HasSummary = false;
    }
}
