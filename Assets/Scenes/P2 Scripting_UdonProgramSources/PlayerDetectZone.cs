
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerDetectZone : UdonSharpBehaviour
{
    [Header("Player Detection")]
    private int playerCount = 0;
    private bool setOpen = false;

    [Header("Door Settings")]
    public Transform smallDoorTrans;
    public Vector3 doorOffset = new Vector3(1.0f, 0, 0);
    public float offsetSpeed = 2.5f;

    private Vector3 doorClosedPos;
    private Vector3 doorOpenPos;

    void Start()
    {
        if (smallDoorTrans != null)
        {
            doorClosedPos = smallDoorTrans.position;
            doorOpenPos = doorClosedPos + doorOffset;
        }
    }

    private void Update()
    {
        // Change playerCount value to dictate required players for interaction
        if (playerCount == 1)
        {
            setOpen = true;
        }

        else
        {
            setOpen = false;
        }

        openingDoor(setOpen);
    }
    
    // I'm leaving the base.OnPlayer triggers in comments for now because they were included by default upon function call
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        //base.OnPlayerTriggerEnter(player);
        playerCount++;
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        //base.OnPlayerTriggerExit(player);
        playerCount--;
    }   

    private void openingDoor(bool setActive)
    {
        // Open and close door based on player count being fulfilled
        if(setActive)
        {
            smallDoorTrans.position = Vector3.MoveTowards(smallDoorTrans.position, doorOpenPos, offsetSpeed * Time.deltaTime);
        }

        else
        {
            smallDoorTrans.position = Vector3.MoveTowards(smallDoorTrans.position, doorClosedPos, offsetSpeed * Time.deltaTime);
        }
    }
}
