using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class FreezeButton : UdonSharpBehaviour
{
    [Header("References")]
    public DollChaser doll;

    [Header("Settings")]
    public float freezeSeconds = 3f;

    public override void Interact()
    {
        if (doll == null) return;

        // Take ownership so network sync works
        Networking.SetOwner(Networking.LocalPlayer, doll.gameObject);

        // Freeze the doll
        doll.FreezeForSeconds(freezeSeconds);
    }
}