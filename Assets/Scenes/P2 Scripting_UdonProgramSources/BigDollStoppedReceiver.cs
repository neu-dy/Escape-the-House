using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// MOCK for testing. Receives stop signal from SmallDollAI and shows visual feedback.
/// Replace with the real Big Doll script; it must implement ReceiveStopSignal().
/// Stop lasts stopDuration seconds, then resumes (visuals revert, placeholder re-enabled).
/// </summary>
public class BigDollStoppedReceiver : UdonSharpBehaviour
{
    [Header("Stop")]
    public float stopDuration = 5f; // Seconds until big doll resumes

    [Header("Feedback")]
    public Renderer indicatorRenderer;  // Turns stoppedColor when stopped, defaultColor when resumed
    public Color stoppedColor = Color.green;
    public Color defaultColor = Color.gray;
    public bool changeColorOnStop = true;
    public GameObject bigDollPlaceholder; // Disabled when stopped, enabled when resumed

    private Material _indicatorMat;
    private float _stopTimer; // Countdown; when 0, big doll resumes

    void Start()
    {
        if (indicatorRenderer != null && indicatorRenderer.material != null)
        {
            _indicatorMat = indicatorRenderer.material;
            _indicatorMat.color = defaultColor;
        }
    }

    void Update()
    {
        if (_stopTimer <= 0f) return;
        _stopTimer -= Time.deltaTime;
        if (_stopTimer <= 0f) OnResume(); // Timer expired, big doll resumes
    }

    // Called by SmallDollAI via SendCustomEvent. Resets stop timer to stopDuration.
    public void ReceiveStopSignal()
    {
        _stopTimer = stopDuration;
        if (changeColorOnStop && _indicatorMat != null) _indicatorMat.color = stoppedColor;
        if (bigDollPlaceholder != null) bigDollPlaceholder.SetActive(false);
    }

    // Reverts visuals when stop timer expires. Real Big Doll would resume movement here.
    private void OnResume()
    {
        if (_indicatorMat != null) _indicatorMat.color = defaultColor;
        if (bigDollPlaceholder != null) bigDollPlaceholder.SetActive(true);
    }

    // Resets stopped state. Call when round restarts or puzzle resets.
    public void Reset()
    {
        _stopTimer = 0f;
        OnResume();
    }
}
