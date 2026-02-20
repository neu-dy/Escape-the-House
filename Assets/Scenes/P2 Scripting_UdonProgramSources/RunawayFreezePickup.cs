using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class RunawayFreezePickup : UdonSharpBehaviour
{
    [Header("References")]
    public DollChaser doll;              // drag Doll root here
    public BoxCollider roomBounds;       // drag RoomBounds BoxCollider here (IsTrigger ON)

    [Header("Runaway Movement")]
    public float moveSpeed = 8f;         // fast
    public float turnSpeed = 720f;       // snappy turning
    public float waypointReachDist = 0.6f;
    public float keepAwayRadius = 2.0f;  // runs away if players get close
    public float keepAwayStrength = 6f;  // higher = more “panic”

    [Header("Freeze Effect")]
    public bool stopChaseOnPickup = true;   // if true: StopChase()
    public float freezeSeconds = 0f;        // if > 0: FreezeForSeconds(freezeSeconds)

    private Vector3 _waypoint;
    private bool _held;

    private void Start()
    {
        // Make movement authoritative: instance master owns the runaway button by default
        VRCPlayerApi local = Networking.LocalPlayer;
        if (local != null && local.isMaster)
        {
            Networking.SetOwner(local, gameObject);
            PickNewWaypoint();
        }
    }

    private void Update()
    {
        // Don't move while held in someone's hand
        if (_held) return;

        // Only the owner moves the object; others just receive synced transform via VRC Object Sync
        if (!Networking.IsOwner(gameObject)) return;

        // Ensure we have bounds
        if (roomBounds == null) return;

        // If waypoint reached, pick a new one
        Vector3 pos = transform.position;
        Vector3 toWp = _waypoint - pos;
        toWp.y = 0f;

        if (toWp.magnitude <= waypointReachDist)
        {
            PickNewWaypoint();
            toWp = _waypoint - pos;
            toWp.y = 0f;
        }

        // Run away from nearby players (repulsion)
        Vector3 repel = GetRepelVector(pos);

        // Desired move direction = toward waypoint + repel
        Vector3 dir = (toWp.normalized + repel).normalized;
        if (dir.sqrMagnitude < 0.0001f) dir = toWp.normalized;

        // Rotate
        Quaternion desiredRot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, turnSpeed * Time.deltaTime);

        // Move
        transform.position = pos + dir * (moveSpeed * Time.deltaTime);
    }

    private Vector3 GetRepelVector(Vector3 fromPos)
    {
        Vector3 repel = Vector3.zero;

        // Repel from all players in instance
        VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
        VRCPlayerApi.GetPlayers(players);

        for (int i = 0; i < players.Length; i++)
        {
            VRCPlayerApi p = players[i];
            if (p == null || !p.IsValid()) continue;

            Vector3 pPos = p.GetPosition();
            Vector3 away = fromPos - pPos;
            away.y = 0f;

            float dist = away.magnitude;
            if (dist < 0.001f) continue;

            if (dist <= keepAwayRadius)
            {
                float t = 1f - Mathf.Clamp01(dist / keepAwayRadius);
                repel += away.normalized * (t * keepAwayStrength);
            }
        }

        repel.y = 0f;
        return repel;
    }

    private void PickNewWaypoint()
    {
        if (roomBounds == null) return;

        Bounds b = roomBounds.bounds;

        // Random point inside bounds (keep same height as current so it stays on floor level)
        float x = Random.Range(b.min.x, b.max.x);
        float z = Random.Range(b.min.z, b.max.z);

        _waypoint = new Vector3(x, transform.position.y, z);
    }

    public override void OnPickup()
    {
        _held = true;

        // Picker becomes owner so any state changes they trigger replicate correctly
        VRCPlayerApi local = Networking.LocalPlayer;
        if (local != null) Networking.SetOwner(local, gameObject);

        // Stop the doll chasing (multiplayer-safe: take ownership before changing doll state)
        if (doll != null && local != null)
        {
            Networking.SetOwner(local, doll.gameObject);

            if (stopChaseOnPickup) doll.StopChase();
            if (freezeSeconds > 0f) doll.FreezeForSeconds(freezeSeconds);
        }
    }

    public override void OnDrop()
    {
        _held = false;

        // Optional: when dropped, let master own it again so it resumes running around consistently
        VRCPlayerApi local = Networking.LocalPlayer;
        if (local != null && local.isMaster)
        {
            Networking.SetOwner(local, gameObject);
            PickNewWaypoint();
        }
    }
}