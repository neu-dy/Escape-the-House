
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Player_Teleport : UdonSharpBehaviour
{
    [Header("Teleport Settings")]
    public Transform teleportTarget;

    [Header("Control Light")]
    public Light_Controller lightController;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player) //only trigger when collide with PLAYER
    {
        // Only affect the LOCAL player so not teleport everyone when one collided
        if (!player.isLocal) return;

        // Safety check
        if (teleportTarget == null) return;

        // Teleport player
        player.TeleportTo(
            teleportTarget.position,
            teleportTarget.rotation
        );
        //for Light Up when finished puzzle
        /*if (lightController != null)
        {
            lightController.TurnOnLights();
        }*/
    }
}
