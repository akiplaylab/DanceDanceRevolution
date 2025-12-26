public readonly struct NoteEvent
{
    public double TimeSec { get; }
    public Lane Lane { get; }
    public NoteEvent(double timeSec, Lane lane)
    {
        TimeSec = timeSec;
        Lane = lane;
    }
}
