using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class NearZoneTracker : UdonSharpBehaviour
{
    public PickupTriggersDoll pickupScript;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        if (pickupScript == null) return;

        pickupScript.SetInsideTrue();
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        if (pickupScript == null) return;

        pickupScript.SetInsideFalse();
    }
}