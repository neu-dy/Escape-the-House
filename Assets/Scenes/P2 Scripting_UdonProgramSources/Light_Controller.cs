
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Light_Controller : UdonSharpBehaviour
{
    [Header("Stage Lights (3 per stage)")]
    public Light[] stage1Lights;
    public Light[] stage2Lights;
    public Light[] stage3Lights;

    private bool stage1On = false;
    private bool stage2On = false;
    private bool stage3On = false;

    void Start()
    {
        SetLights(stage1Lights, false);
        SetLights(stage2Lights, false);
        SetLights(stage3Lights, false);
    }

    // 🎯 Friend calls this
    public void TurnOnStage(int stage)
    {
        if (stage == 1 && !stage1On)
        {
            stage1On = true;
            SetLights(stage1Lights, true);
        }
        else if (stage == 2 && !stage2On)
        {
            stage2On = true;
            SetLights(stage2Lights, true);
        }
        else if (stage == 3 && !stage3On)
        {
            stage3On = true;
            SetLights(stage3Lights, true);
        }
    }

    public void TurnOffAll()
    {
        stage1On = stage2On = stage3On = false;
        SetLights(stage1Lights, false);
        SetLights(stage2Lights, false);
        SetLights(stage3Lights, false);
    }

    private void SetLights(Light[] lights, bool on)
    {
        if (lights == null) return;

        for (int i = 0; i < lights.Length; i++)
        {
            if (lights[i] != null)
                lights[i].enabled = on;
        }
    }
}
