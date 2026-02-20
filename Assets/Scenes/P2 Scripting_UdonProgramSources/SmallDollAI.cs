using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// Small doll that wanders in an area. Player chases and clicks it (within clickableDistance)
/// to stop the big doll. On click, this doll freezes for stopDuration, then resumes wandering.
/// Assign bigDollReceiver to the UdonBehaviour that implements ReceiveStopSignal().
/// </summary>
public class SmallDollAI : UdonSharpBehaviour
{
    [Header("Big Doll")]
    public UdonBehaviour bigDollReceiver; // Must have ReceiveStopSignal() method

    [Header("Area")]
    public Transform areaCenter;          // Center of wander zone. If it has a Collider, uses its bounds.
    public Vector3 areaSize = new Vector3(5f, 1f, 5f); // Fallback when areaCenter has no Collider

    [Header("Movement")]
    public Transform smallDollTransform;  // Transform to move. Defaults to this object.
    public float moveSpeed = 2.5f;
    public float arrivalDistance = 0.5f;  // Pick new waypoint when this close to current one
    public float waypointMinTime = 1.5f;  // Minimum seconds between waypoint changes

    [Header("Catch")]
    public float clickableDistance = 1.2f; // Max distance for Interact to work.
    public float stopDuration = 5f;        // Seconds this doll stays still after being clicked

    private Vector3 _currentWaypoint;
    private float _stopTimer;              // Countdown while doll is frozen after click
    private float _waypointTime;            // Time since last waypoint change
    private Collider _areaCollider;        // Cached for bounds lookup

    void Start()
    {
        InteractionText = "Catch";
        if (smallDollTransform == null) smallDollTransform = transform;
        if (areaCenter == null) areaCenter = transform;
        _areaCollider = areaCenter.GetComponent<Collider>();
        _currentWaypoint = PickRandomPointInArea();
    }

    void Update()
    {
        if (_stopTimer > 0f)
        {
            _stopTimer -= Time.deltaTime;
            return;
        }

        // Only show "Catch" prompt when player is close enough
        var local = Networking.LocalPlayer;
        if (local != null)
            DisableInteractive = Vector3.Distance(local.GetPosition(), smallDollTransform.position) > clickableDistance;

        // Wander: move toward waypoint, pick new one when arrived or timeout
        float dist = Vector3.Distance(smallDollTransform.position, _currentWaypoint);
        _waypointTime += Time.deltaTime;

        if (dist <= arrivalDistance || _waypointTime >= waypointMinTime)
        {
            _currentWaypoint = PickRandomPointInArea();
            _waypointTime = 0f;
        }
        else
        {
            var dir = (_currentWaypoint - smallDollTransform.position).normalized;
            smallDollTransform.position += dir * (moveSpeed * Time.deltaTime);
        }
    }

    // Returns a random point inside the wander area (Collider bounds or areaSize box).
    private Vector3 PickRandomPointInArea()
    {
        if (_areaCollider != null)
        {
            var b = _areaCollider.bounds;
            return new Vector3(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y),
                Random.Range(b.min.z, b.max.z));
        }

        // No Collider on areaCenter: use position + areaSize as a box
        var center = areaCenter != null ? areaCenter.position : smallDollTransform.position;
        var half = areaSize * 0.5f;
        return center + new Vector3(
            Random.Range(-half.x, half.x),
            Random.Range(-half.y, half.y),
            Random.Range(-half.z, half.z));
    }

    // VRC Interact. Only processes when player is within clickableDistance.
    public override void Interact()
    {
        var local = Networking.LocalPlayer;
        if (local == null) return;
        if (Vector3.Distance(local.GetPosition(), smallDollTransform.position) > clickableDistance) return;

        _stopTimer = stopDuration; // Freeze this doll
        SendStopSignalToBigDoll();
    }

    // Sends ReceiveStopSignal to big doll. Can be called from other scripts.
    public void SendStopSignalToBigDoll()
    {
        if (bigDollReceiver != null)
            bigDollReceiver.SendCustomEvent("ReceiveStopSignal");
    }

    /// <summary>Resets doll and big doll state. Call when round restarts or puzzle resets.</summary>
    public void Reset()
    {
        _stopTimer = 0f;
        _currentWaypoint = PickRandomPointInArea();
        _waypointTime = 0f;
        if (bigDollReceiver != null)
            bigDollReceiver.SendCustomEvent("Reset");
    }
}
