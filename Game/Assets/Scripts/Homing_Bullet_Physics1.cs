using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Homing_Bullet_Physics : MonoBehaviour
{
    public float MaxMoveSpeed = 50f;
    public float IniMoveSpeed = 5f;
    public float acceleration = 0.1f;
    public float lifetime = 2f;
    public int damage_per_bullet = 0;
    public float angle_correction = 0.1f;
    private float iniTimestamp;
    private bool destroyed;
    private float actualAngleTrajectory;

    //Angular Movement
    private float radiusRing;
    private float actualAngle;
    private bool facingRight;
    private Vector2 speed;
    private float linear_speed = 0;

    //Follow
    [SerializeField] private int delay_frames = 10;
    private Queue<float> old_pos = new Queue<float>();

    private CharacterController controller;
    private GameObject Trail;

    // Start is called before the first frame update
    void Start()
    {
        // GET ORIENTATION, POSITION FROM FIREPOINT...
        Trail = transform.GetChild(1).gameObject;
        controller = GetComponent<CharacterController>();
        iniTimestamp = Time.time;
    }

    // SET VERTICAL ANGULATION
    public void init(float ini_angle, float ini_radius, bool isFacingRight, float initial_trajectory_angle, float BarrelLengthOffset)
    {
        radiusRing = ini_radius;
        actualAngle = ini_angle;
        facingRight = isFacingRight;
        actualAngleTrajectory = initial_trajectory_angle;
        linear_speed = IniMoveSpeed;
        if (initial_trajectory_angle != Mathf.PI) actualAngle += BarrelLengthOffset;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if bullet was destroyed still render frames
        if (destroyed)
        {
            if ((Time.time - iniTimestamp) > 0.5)
            {
                Destroy(Trail.GetComponent<TrailRenderer>());
                Destroy(this.gameObject);
            }
            else return;
        }

        // Destroy bullet
        if ((Time.time - iniTimestamp) > lifetime)
        {
            destroyBullet();
            return;
        }

        //MOVE BULLET
        float timeDelta = Time.deltaTime;

        // Calc angle to player (with delay)
        float player_y = GameObject.Find("Player").transform.position.y;
        float angle_diff = GameObject.Find("Player").GetComponent<Angular_Physics>().getActualAngle();
        old_pos.Enqueue(player_y);
        old_pos.Enqueue(angle_diff);
        if (delay_frames == 0)
        {
            player_y = old_pos.Dequeue();
            angle_diff = old_pos.Dequeue();
        }
        else delay_frames--;

        if (facingRight) angle_diff = ((angle_diff - actualAngle) + 2 * Mathf.PI) % (Mathf.PI * 2);
        else angle_diff = ((actualAngle-angle_diff) + 2 * Mathf.PI) % (Mathf.PI * 2);
        float temp_angle_to_player = Mathf.Atan((player_y - transform.position.y) / Mathf.Abs(angle_diff * radiusRing));

        // Calculate X & Y Speed
        linear_speed = Mathf.Min(MaxMoveSpeed, linear_speed + acceleration);
        if (actualAngleTrajectory < temp_angle_to_player) actualAngleTrajectory += angle_correction;
        else actualAngleTrajectory -= angle_correction;
        speed.y = linear_speed * Mathf.Sin(actualAngleTrajectory);
        if (facingRight) speed.x = linear_speed * Mathf.Cos(actualAngleTrajectory);
        else speed.x = -linear_speed * Mathf.Cos(actualAngleTrajectory);

        // Calculate Horitzontal desplacement && turn object so it faces center
        actualAngle += (speed.x / radiusRing) * timeDelta;
        float newX = Mathf.Cos(actualAngle) * radiusRing - transform.position.x;
        float newZ = Mathf.Sin(actualAngle) * radiusRing - transform.position.z;
        transform.LookAt(new Vector3(0, transform.position.y, 0));

        // Move Bullet
        controller.Move(new Vector3(newX, speed.y * timeDelta, newZ));
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == "Terrain")
        {
            destroyBullet();
        }
        else if (hit.gameObject.tag == "Player")
        {
            bool dealt_dmg = false;
            if (transform.lossyScale.x > 0.5) dealt_dmg = hit.GetComponent<PlayerLogic>().TakeDamageAndHitBack(damage_per_bullet);
            if (dealt_dmg) destroyBullet();
        }
    }

    private void destroyBullet()
    {
        destroyed = true;
        iniTimestamp = Time.time;
        GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
