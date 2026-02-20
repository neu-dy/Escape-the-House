using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class DollChaser : UdonSharpBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.2f;
    public float turnSpeed = 360f;
    public float stopDistance = 1.2f;

    [Header("Networked State")]
    [UdonSynced] private bool chaseEnabled;
    [UdonSynced] private int targetPlayerId = -1;

    [Header("Temporary Disable")]
    [UdonSynced] private bool frozen;
    [UdonSynced] private float unfreezeTime = -1f;

    private VRCPlayerApi targetPlayer;

    private void Update()
    {
        // IMPORTANT: only the OWNER moves the doll (authoritative)
        if (!Networking.IsOwner(gameObject)) return;

        if (!chaseEnabled) return;

        if (frozen)
        {
            if (unfreezeTime > 0f && Time.time >= unfreezeTime)
            {
                frozen = false;
                unfreezeTime = -1f;
                RequestSerialization();
            }
            else return;
        }

        if (targetPlayer == null || !targetPlayer.IsValid())
        {
            targetPlayer = VRCPlayerApi.GetPlayerById(targetPlayerId);
            if (targetPlayer == null || !targetPlayer.IsValid()) return;
        }

        Vector3 myPos = transform.position;
        Vector3 targetPos = targetPlayer.GetPosition();

        Vector3 toTarget = targetPos - myPos;
        toTarget.y = 0f;

        float dist = toTarget.magnitude;
        if (dist <= stopDistance) return;

        if (toTarget.sqrMagnitude > 0.0001f)
        {
            Quaternion desiredRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                desiredRot,
                turnSpeed * Time.deltaTime
            );
        }

        transform.position = myPos + toTarget.normalized * (moveSpeed * Time.deltaTime);
    }

    // Call this when the pickup condition is met (from the picker client)
    public void StartChasePlayerId(int playerId)
    {
        VRCPlayerApi local = Networking.LocalPlayer;
        if (local == null) return;

        // Make the picker own the doll so THEIR movement replicates to everyone
        Networking.SetOwner(local, gameObject);

        targetPlayerId = playerId;
        chaseEnabled = true;
        frozen = false;
        unfreezeTime = -1f;

        targetPlayer = VRCPlayerApi.GetPlayerById(targetPlayerId);

        RequestSerialization();
    }

    public void StopChase()
    {
        if (!Networking.IsOwner(gameObject)) return;
        chaseEnabled = false;
        targetPlayerId = -1;
        targetPlayer = null;
        RequestSerialization();
    }

    public void FreezeForSeconds(float seconds)
    {
        if (!Networking.IsOwner(gameObject)) return;
        frozen = true;
        unfreezeTime = Time.time + Mathf.Max(0f, seconds);
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        // Rebuild target reference on non-owners
        targetPlayer = VRCPlayerApi.GetPlayerById(targetPlayerId);
    }
}