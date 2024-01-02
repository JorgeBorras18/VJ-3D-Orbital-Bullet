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

    public float getVerticalSpeed() { return verticalSpeed; }

    public void setVerticalSpeed(float new_vertical_speed) { verticalSpeed = new_vertical_speed; }

    public float getActualAngle() { return actualAngle; }
    public float getActualRadius() { return radiusRing; }

    // Get angle relative to another (where 0 would be on the opposite side of ring and pi would be origin_angle = actual angle)
    public float getRelativeAngle(float origin_angle)
    {
        float relative_angle = actualAngle % (Mathf.PI * 2) - (origin_angle - Mathf.PI);
        if (relative_angle < 0) relative_angle = (relative_angle + 4 * Mathf.PI) % (Mathf.PI * 2);
        return relative_angle;
    }

    static public float getAngleFromCoordinades(float posX, float posZ)
    {
        if (posX == 0)
        {
            if (posZ > 0) return Mathf.PI / 2;
            else return Mathf.PI * 3 / 2;
        }
        else if (posX > 0) return (Mathf.Atan(posZ / posX) + Mathf.PI * 2) % (Mathf.PI * 2);
        return Mathf.PI + Mathf.Atan(posZ / posX);
    }

    static public float getRadiusFromPosition(float angle, float posX, float posZ)
    {
        return Mathf.Max(Mathf.Abs(posZ / Mathf.Sin(angle)), Mathf.Abs(posX / Mathf.Cos(angle)));
    }

}
