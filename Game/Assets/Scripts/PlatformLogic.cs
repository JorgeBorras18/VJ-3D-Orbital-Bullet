using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLogic : MonoBehaviour
{
    public float rotationSpeed; // Adjust the rotation speed as needed
    private int rotation_frames;
    public int max_rotation_frames;
    private bool is_rotating;
    private bool rotated;

    void Start()
    {
        // Initialize private variables
        rotation_frames = 0;
        is_rotating = false;
        rotated = false;
    }

    void Update()
    {
        MovementIfNeeded();
    }

    void MovementIfNeeded()
    {
        // Check if the "L" key is pressed
        if (!rotated && Input.GetKeyDown(KeyCode.L))
        {
            is_rotating = true;

        }

        if (is_rotating)
        {
            if (rotation_frames <= max_rotation_frames)
            {
                // Rotate the component in the Y-axis
                MovePlatformOneTick();
                ++rotation_frames;
            }
            else
            {
                is_rotating = false;
                rotated = true;
            }
        }

    }

    void MovePlatformOneTick()
    {
        // Get the current rotation of the component
        Vector3 currentRotation = transform.eulerAngles;

        // Update the Y-axis rotation
        currentRotation.y += rotationSpeed;

        // Apply the new rotation to the component
        transform.eulerAngles = currentRotation;
    }
}
