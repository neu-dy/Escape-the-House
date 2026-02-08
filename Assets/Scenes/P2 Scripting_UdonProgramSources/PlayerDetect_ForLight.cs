
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerDetect_ForLight : UdonSharpBehaviour
{
    [Header("Player Detection")]
    private int playerCount = 0;
    private bool setOn = false;

    [Header("Light Settings")]
    public Light targetLight;          
    public bool startOff = true;

    void Start()
    {
        if (targetLight != null && startOff)
        {
            targetLight.enabled = false; // set the light to off at the beginning
        }
    }

    private void Update()
    {
        // Same idea as the door zone: 1 player in zone = ON, otherwise OFF
        setOn = (playerCount == 1);

        ApplyLightState(setOn);
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        playerCount++;
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        playerCount--;

        // Safety clamp so it never goes negative (idk but I think this can happen with odd collider/respawn cases)
        if (playerCount < 0) playerCount = 0;
    }

    private void ApplyLightState(bool turnOn)
    {
        if (targetLight == null) return;

        // Only set when needed (avoids spamming the same assignment every frame)
        if (targetLight.enabled != turnOn)
        {
            targetLight.enabled = turnOn;
        }
    }
}