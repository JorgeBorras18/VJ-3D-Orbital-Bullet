using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

/* CLASS THAT APPLIES ANGULAR PHYSICS TO OBJECTS,
 * Modifying movement so it runs around a radius & a Center
 */

public class Angular_Physics : MonoBehaviour
{
    public float actualAngle = 0f;

    CharacterController controller;
    public float radiusRing = 9f;
    private float verticalSpeed = 0f;
    private Vector3 speed;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }


    public void init(float RadiusRing, float initialAngle)
    {
        radiusRing = RadiusRing;
        actualAngle = initialAngle;
    }


    // Apply Movement to Parent
    public void moveObject(float XSpeed, float Gravity = 0.7f)
    {
        speed.x = 0;
        speed.z = 0;
        float timeDelta = Time.deltaTime;

        // Calculate Horitzontal desplacement && turn object so it faces camera
        actualAngle += (XSpeed / radiusRing) * timeDelta;
        speed.x = Mathf.Cos(actualAngle) * radiusRing - transform.position.x;
        speed.z = Mathf.Sin(actualAngle) * radiusRing - transform.position.z;
        transform.LookAt(new Vector3(0, transform.position.y, 0));

        // Vertical Movement
        verticalSpeed = Mathf.Max(-10, verticalSpeed - Gravity);

        // Move object
        controller.Move(new Vector3(speed.x, verticalSpeed * timeDelta, speed.z));
    }

    // Apply Vertical Movement to Parent
    public void applyJump(float YSpeed)
    {
        verticalSpeed = YSpeed;
    }

    public float getVerticalSpeed()
    {
        return verticalSpeed;
    }

    public float getActualAngle() { return actualAngle; }
    public float getActualRadius() { return radiusRing; }
}
