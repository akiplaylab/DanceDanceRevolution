using System;
using System.Collections.Generic;

public sealed class JudgementCounter
{
    readonly Dictionary<Judgement, int> counts = new();
    int missCount;
    int currentCombo;
    int maxCombo;

    public void Reset()
    {
        counts.Clear();
        missCount = 0;
        currentCombo = 0;
        maxCombo = 0;
    }

    public void Record(Judgement judgement)
    {
        if (judgement == Judgement.None) return;

        if (!counts.TryGetValue(judgement, out var current))
            current = 0;

        counts[judgement] = current + 1;

        if (IsComboJudgement(judgement))
        {
            currentCombo++;
            maxCombo = Math.Max(maxCombo, currentCombo);
        }
        else
        {
            currentCombo = 0;
        }
    }

    public void RecordMiss()
    {
        missCount++;
        currentCombo = 0;
    }

    public JudgementSummary CreateSummary(int totalNotes)
    {
        return new JudgementSummary(counts, missCount, maxCombo, totalNotes);
    }

    public int CurrentCombo => currentCombo;
    public int MaxCombo => maxCombo;

    static bool IsComboJudgement(Judgement judgement)
    {
        return judgement != Judgement.None && judgement <= Judgement.Good;
    }
}

public readonly struct JudgementSummary
{
    readonly IReadOnlyDictionary<Judgement, int> counts;
    readonly int maxCombo;

    public JudgementSummary(IReadOnlyDictionary<Judgement, int> counts, int missCount, int maxCombo, int totalNotes)
    {
        this.counts = new Dictionary<Judgement, int>(counts);
        MissCount = missCount;
        this.maxCombo = maxCombo;
        TotalNotes = totalNotes;

        Score = ScoreCalculator.Calculate(
            totalNotes,
            GetCount(Judgement.Marvelous),
            GetCount(Judgement.Perfect),
            GetCount(Judgement.Great),
            GetCount(Judgement.Good),
            GetCount(Judgement.Bad),
            MissCount);
    }

    public int MissCount { get; }
    public int MaxCombo => maxCombo;
    public int TotalNotes { get; }
    public int Score { get; }

    public int GetCount(Judgement judgement)
    {
        return counts.TryGetValue(judgement, out var count) ? count : 0;
    }
}
