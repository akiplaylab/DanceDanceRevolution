using UnityEngine;

public sealed class NoteView : MonoBehaviour
{
    public Lane lane;
    public double timeSec;

    public void Init(Lane lane, double timeSec)
    {
        this.lane = lane;
        this.timeSec = timeSec;
        ApplyRotation();
    }

    private void OnValidate() => ApplyRotation();

    private void ApplyRotation()
    {
        float z = lane switch
        {
            Lane.Left => 0f,
            Lane.Down => 90f,
            Lane.Up => -90f,
            Lane.Right => 180f,
            _ => 0f
        };
        transform.localRotation = Quaternion.Euler(0, 0, z);
    }
}
