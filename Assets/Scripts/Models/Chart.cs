using System;
using System.Collections.Generic;

public sealed class Chart
{
    public string MusicFile { get; }
    public byte Bpm { get; }
    public float OffsetSec { get; }
    public IReadOnlyList<Note> Notes { get; }

    public Chart(string musicFile, int bpm, float offsetSec, IReadOnlyList<Note> notes)
    {
        if (bpm <= 0 || bpm > byte.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(bpm), $"bpm must be in 1-{byte.MaxValue}");

        MusicFile = musicFile;
        Bpm = (byte)bpm;
        OffsetSec = offsetSec;
        Notes = notes ?? throw new ArgumentNullException(nameof(notes));
    }
}
