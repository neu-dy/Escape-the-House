
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ButtonDirection : UdonSharpBehaviour
{
    public ObjectAxisMover objectMovedByButton; // This object should have ObjectAxisMover script attached
    public Vector3 moveDirection; // Direction of movement this button makes, assign in Inspector (e.g. 1, 0, 0)

    public override void Interact()
    {
        if (objectMovedByButton != null)
        {
            objectMovedByButton.MoveObjectByUnit(moveDirection); // Call MoveObjectByUnit function with assigned direction input
        }
    }
}
