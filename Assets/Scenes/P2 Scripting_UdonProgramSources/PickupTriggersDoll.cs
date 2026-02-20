using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class PickupTriggersDoll : UdonSharpBehaviour
{
    [Header("References")]
    public DollChaser doll;   // drag the Doll root (with DollChaser + VRC Object Sync) here

    [Header("State (read-only)")]
    [SerializeField] private bool localInsideZone;

    void Start()
    {
        InteractionText = "Catch";
    }

    // Called by NearZoneTracker (on the trigger zone)
    public void SetInsideTrue()
    {
        localInsideZone = true;
        // Debug.Log("[PickupTriggersDoll] SetInsideTrue");
    }

    public void SetInsideFalse()
    {
        localInsideZone = false;
        // Debug.Log("[PickupTriggersDoll] SetInsideFalse");
    }

    // VRChat Interact: fires when player clicks (shows "Catch" instead of "Hold to Grab")
    public override void Interact()
    {
        if (!localInsideZone) return;
        if (doll == null) return;

        VRCPlayerApi local = Networking.LocalPlayer;
        if (local == null) return;

        doll.StartChasePlayerId(local.playerId);
    }

    // Optional: stop chase if the item is dropped
    // public override void OnDrop()
    // {
    //     if (doll == null) return;
    //     doll.StopChase();
    // }
}