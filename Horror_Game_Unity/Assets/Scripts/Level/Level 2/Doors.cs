using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Doors : MonoBehaviour
{
    [Header("Unique")]
    [SerializeField] public float duration = 2.0f; // Duration in seconds
    [SerializeField] public bool open = false; // Flag to control door state
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float elapsedTime = 0f;
    [SerializeField] private bool isMoving = false;

    void Start()
    {
        // Set initial position of door
        transform.position = transform.position + new Vector3(0, -10, 0);
        initialPosition = transform.position;

        // Calculate the target position by moving -30 on the Y axis
        targetPosition = initialPosition + new Vector3(0, -30, 0);
    }

    void Update()
    {
        // If the door is opening and it's not already moving
        if (open && !isMoving)
        {
            isMoving = true;
            elapsedTime = 0f;
        }

        // If the door is moving, update its position
        if (isMoving)
        {
            elapsedTime += Time.deltaTime;

            // Interpolate between the initial and target positions
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);

            // If the elapsed time is greater than or equal to the duration, stop moving
            if (elapsedTime >= duration)
            {
                isMoving = false;
                open = false; // Reset the open flag, so the door doesn't move again
            }
        }
    }

    public void OpenDoor()
    {
        open = true;
    }
}
