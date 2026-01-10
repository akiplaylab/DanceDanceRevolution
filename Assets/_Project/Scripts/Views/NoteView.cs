using System;
using System.Collections;
using UnityEngine;

public sealed class NoteView : MonoBehaviour
{
    public Lane Lane { get; private set; }
    public double TimeSec { get; private set; }
    public NoteDivision Division { get; private set; }

    [Header("Division Colors")]
    [SerializeField] Color quarterColor = Color.red;
    [SerializeField] Color eighthColor = Color.blue;
    [SerializeField] Color sixteenthColor = Color.yellow;

    [Header("Hit Burst")]
    [SerializeField] float burstScale = 1.35f;
    [SerializeField] float burstDuration = 0.12f;

    [SerializeField] SpriteRenderer spriteRenderer;
    Coroutine burstRoutine;
    Vector3 baseScale;
    Color baseColor;

    public void Init(Note note)
    {
        Lane = note.Lane;
        TimeSec = note.TimeSec;
        Division = note.Division;

        ApplyRotation();
        ApplyColor();
    }

    void OnValidate()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        CacheBaseState();
    }

    public void PlayHitBurst(float intensity01, Action onComplete)
    {
        if (burstRoutine != null) StopCoroutine(burstRoutine);
        burstRoutine = StartCoroutine(CoHitBurst(intensity01, onComplete));
    }

    private void ApplyRotation()
    {
        float z = Lane switch
        {
            Lane.Left => 0f,
            Lane.Down => 90f,
            Lane.Up => -90f,
            Lane.Right => 180f,
            _ => 0f
        };
        transform.localRotation = Quaternion.Euler(0, 0, z);
    }

    private void ApplyColor()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.color = Division switch
        {
            NoteDivision.Quarter => quarterColor,
            NoteDivision.Eighth => eighthColor,
            NoteDivision.Sixteenth => sixteenthColor,
            _ => Color.white
        };

        CacheBaseState();
    }

    void CacheBaseState()
    {
        if (spriteRenderer == null) return;
        baseScale = transform.localScale;
        baseColor = spriteRenderer.color;
    }

    IEnumerator CoHitBurst(float intensity01, Action onComplete)
    {
        if (spriteRenderer == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        CacheBaseState();

        float t = 0f;
        float dur = Mathf.Max(0.01f, burstDuration);
        float targetScale = Mathf.Lerp(1.05f, burstScale, Mathf.Clamp01(intensity01));

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / dur);
            transform.localScale = baseScale * Mathf.Lerp(1f, targetScale, a);
            var color = baseColor;
            color.a = Mathf.Lerp(baseColor.a, 0f, a);
            spriteRenderer.color = color;
            yield return null;
        }

        transform.localScale = baseScale;
        spriteRenderer.color = baseColor;
        burstRoutine = null;
        onComplete?.Invoke();
    }
}
