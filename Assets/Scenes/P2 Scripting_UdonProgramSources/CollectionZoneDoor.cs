
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CollectionZoneDoor : UdonSharpBehaviour
{
    [Header("Door")]
    public Transform door;
    public Vector3 openOffset = new Vector3(0f, 2.2f, 0f);
    public float openSpeed = 1.5f;

    [Header("Required IDs (must match ItemId.itemId)")]
    public int requiredId1 = 1;
    public int requiredId2 = 2;
    public int requiredId3 = 3;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool opening;

    private bool has1, has2, has3;

    void Start()
    {
        if (door != null)
        {
            closedPos = door.position;
            openPos = closedPos + openOffset;
        }
    }

    void Update()
    {
        if (!opening || door == null) return;
        door.position = Vector3.MoveTowards(door.position, openPos, openSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryMark(other, true);
        CheckOpen();
    }

    private void OnTriggerExit(Collider other)
    {
        TryMark(other, false);
        // Optional: if you want door to close when items leave, you can handle it here.
    }

    private void OnTriggerStay(Collider other)
    {
        // Ensures "throwing" works: only count if the item is not being held.
        // Also handles cases where enter happens while still held.
        TryMarkIfDropped(other);
        CheckOpen();
    }

    private void TryMarkIfDropped(Collider other)
    {
        var pickup = other.GetComponentInParent<VRC_Pickup>();
        if (pickup == null) return;

        // Only count items that are not currently held
        if (pickup.currentPlayer != null) return;

        var id = pickup.GetComponent<ItemId>();
        if (id == null) return;

        SetHas(id.itemId, true);
    }

    private void TryMark(Collider other, bool entering)
    {
        var idComp = other.GetComponentInParent<ItemId>();
        if (idComp == null) return;

        // If entering, we still only want to count once it's dropped (handled in Stay).
        // If exiting, clear immediately.
        if (!entering)
            SetHas(idComp.itemId, false);
    }

    private void SetHas(int itemId, bool value)
    {
        if (itemId == requiredId1) has1 = value;
        else if (itemId == requiredId2) has2 = value;
        else if (itemId == requiredId3) has3 = value;
    }

    private void CheckOpen()
    {
        if (opening) return;
        if (door == null) return;

        if (has1 && has2 && has3)
            opening = true;
    }
}