using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class PickupTriggersDoll : UdonSharpBehaviour
{
    [Header("References")]
    public DollChaser doll;   // drag the Doll root (with DollChaser + VRC Object Sync) here

    [Header("State (read-only)")]
    [SerializeField] private bool localInsideZone;

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

    // VRChat event: fires when THIS pickup is picked up
    public override void OnPickup()
    {
        // Debug.Log("[PickupTriggersDoll] OnPickup localInsideZone=" + localInsideZone);

        if (!localInsideZone) return;
        if (doll == null) return;

        VRCPlayerApi local = Networking.LocalPlayer;
        if (local == null) return;

        // Multiplayer: set doll to chase the picker (by synced playerId)
        doll.StartChasePlayerId(local.playerId);
    }

    // Optional: stop chase if the item is dropped
    // public override void OnDrop()
    // {
    //     if (doll == null) return;
    //     doll.StopChase();
    // }
}