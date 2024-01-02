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
    private string Identifier;

    void Start()
    {
        // Initialize private variables
        rotation_frames = 0;
        is_rotating = false;
        rotated = false;
    }

    void Update()
    {
        if (!rotated && is_rotating)
        {
            MovementIfNeeded();
        }
    }

    public void MovementIfNeeded() {
             
        if (rotation_frames <= max_rotation_frames) {
                // Rotate the component in the Y-axis
                MovePlatformOneTick();
                ++rotation_frames;
        }
        else {
                is_rotating = false;
                rotated = true;
        }
        
    }

    public void triggerPlatformMovementToStart()
    {
        if (!rotated) is_rotating = true;
    }

    void MovePlatformOneTick()
    {
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y += rotationSpeed;
        transform.eulerAngles = currentRotation;
    }
    
    public bool isFinished() {
        return rotated;
    }

}
