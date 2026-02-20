
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ObjectAxisMover : UdonSharpBehaviour
{
    [SerializeField] private float movingDistance = 1.0f; // Distance moved per button press
    [SerializeField] private float movingSpeed = 3.0f; // Speed of object movement

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position;
    }

    public void MoveObjectByUnit(Vector3 buttonDirection)
    {
        // Incrementally adds move distance to target object and allows stacking movements
        targetPosition += buttonDirection * movingDistance;
        isMoving = true;
    }

    void Update()
    {
        if (!isMoving) return;

        // Move this object towards direction target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movingSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }
}
