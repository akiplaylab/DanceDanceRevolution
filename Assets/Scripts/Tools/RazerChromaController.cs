using System;
using UnityEngine;


#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using ChromaSDK;
#endif

public sealed class RazerChromaController : MonoBehaviour
{
    [SerializeField] string appTitle = "DanceDanceRevolution";
    [SerializeField] string appDescription = "Light up Razer Chroma devices on judgements.";
    [SerializeField] string authorName = "DanceDanceRevolution";
    [SerializeField] string authorContact = "https://developer.razer.com/chroma";

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    bool initialized;
    APPINFOTYPE appInfo;
#endif

    void OnEnable()
    {
        InitializeChroma();
    }

    void OnDisable()
    {
        ShutdownChroma();
    }

    public void TriggerJudgement(Judgement judgement, Color color)
    {
        if (judgement != Judgement.Perfect && judgement != Judgement.Great)
            return;

        if (!InitializeChroma())
            return;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        int chromaColor = ChromaAnimationAPI.GetRGB((byte)Mathf.RoundToInt(color.r * 255f), (byte)Mathf.RoundToInt(color.g * 255f), (byte)Mathf.RoundToInt(color.b * 255f));

        ApplyStaticColor(chromaColor);
#endif
    }

    bool InitializeChroma()
    {
#if !UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN
        return false;
#else
        if (initialized)
            return true;

        try
        {
            appInfo = new APPINFOTYPE
            {
                Title = appTitle,
                Description = appDescription,
                Author_Name = authorName,
                Author_Contact = authorContact,
                SupportedDevice = (0x01 | 0x02 | 0x04 | 0x08 | 0x10 | 0x20),
                Category = 1,
            };

            ChromaAnimationAPI.UseIdleAnimations(false);
            int result = ChromaAnimationAPI.InitSDK(ref appInfo);

            if (result != 0)
            {
                Debug.LogWarning($"Razer Chroma SDK initialization failed with code {result}.");
                return false;
            }

            initialized = true;
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to initialize Razer Chroma SDK: {ex.Message}");
            initialized = false;
            return false;
        }
#endif
    }

    void ShutdownChroma()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (!initialized)
            return;

        try
        {
            ChromaAnimationAPI.UseIdleAnimations(true);
            ChromaAnimationAPI.Uninit();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to shut down Razer Chroma SDK cleanly: {ex.Message}");
        }

        initialized = false;
#endif
    }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    static void ApplyStaticColor(int color)
    {
        ChromaAnimationAPI.SetStaticColor((int)ChromaAnimationAPI.DeviceType.DE_1D, (int)ChromaAnimationAPI.Device1D.ChromaLink, color);
        ChromaAnimationAPI.SetStaticColor((int)ChromaAnimationAPI.DeviceType.DE_1D, (int)ChromaAnimationAPI.Device1D.Headset, color);
        ChromaAnimationAPI.SetStaticColor((int)ChromaAnimationAPI.DeviceType.DE_1D, (int)ChromaAnimationAPI.Device1D.Mousepad, color);

        ChromaAnimationAPI.SetStaticColor((int)ChromaAnimationAPI.DeviceType.DE_2D, (int)ChromaAnimationAPI.Device2D.Keyboard, color);
        ChromaAnimationAPI.SetStaticColor((int)ChromaAnimationAPI.DeviceType.DE_2D, (int)ChromaAnimationAPI.Device2D.Keypad, color);
        ChromaAnimationAPI.SetStaticColor((int)ChromaAnimationAPI.DeviceType.DE_2D, (int)ChromaAnimationAPI.Device2D.Mouse, color);
    }
#endif
}
