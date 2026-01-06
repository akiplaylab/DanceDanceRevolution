public static class ScoreCalculator
{
    const int MaxScore = 1_000_000;

    public static int Calculate(int totalNotes, int marvelous, int perfect, int great, int good, int bad, int miss)
    {
        if (totalNotes <= 0)
            return 0;

        double basePoint = (double)MaxScore / totalNotes;

        double weighted =
            marvelous * 1.00 +
            perfect * 0.98 +
            great * 0.60 +
            good * 0.20 +
            bad * 0.00 +
            miss * 0.00;

        return (int)(basePoint * weighted);
    }
}
