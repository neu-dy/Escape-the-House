
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ItemId : UdonSharpBehaviour
{
    [Tooltip("Unique ID for this item (1,2,3)")]
    public int itemId = 1;
}
