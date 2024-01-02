using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LaserDrone : MonoBehaviour
{
    enum STATE { PATROLING, PLAYER_DETECTED, FIRING, FIRING_THEN_IDLE }

    [SerializeField] private float time_between_shoots = 1f;
    [SerializeField] private float time_between_bullets = 0.3f;
    [SerializeField] private int bullets_per_pulse = 3;
    [SerializeField] private float patrol_range = 10;
    [SerializeField] private float desired_distance_from_player = 10f;
    [SerializeField] private float max_move_speed = 20;
    [SerializeField] private float patrolling_speed = 8;
    [SerializeField] private float acceleration = 2;
    [SerializeField] private float up_down_range = 0.5f;
    [SerializeField] private float vertical_speed = 0.2f;

    [SerializeField] private GameObject Bullet_Prefab;
    [SerializeField] private Transform Fire_Point;
    [SerializeField] private Billboard_Facing_Player billboard;
    [SerializeField] private Angular_Physics angularPhysics;

    private Animator animator;
    private STATE actual_state = STATE.PATROLING;
    private float last_shot_timestamp = 0f;
    private float ring_radius;
    private float bullet_count;
    private float temp_angle_to_player;
    private float offset_angle_rotation = 0f;
    private float ideal_y;
    private bool going_up = true;

    private bool patrolling_to_left = false;
    private float patrol_centerpoint;
    private float actual_speed = 0f;


    // Start is called before the first frame update
    void Start()
    {
        if (Fire_Point == null) Fire_Point = transform.GetChild(0).transform;
        if (billboard == null) billboard = transform.parent.gameObject.GetComponent<Billboard_Facing_Player>();
        if (angularPhysics == null) angularPhysics = transform.parent.gameObject.GetComponent<Angular_Physics>();
        animator = transform.parent.gameObject.GetComponent<Animator>();
        float actual_angle = Angular_Physics.getAngleFromCoordinades(transform.position.x, transform.position.z);
        ring_radius = Angular_Physics.getRadiusFromPosition(actual_angle, transform.position.x, transform.position.z);
        angularPhysics.init(ring_radius, actual_angle);
        patrol_centerpoint = actual_angle;
        patrol_range = patrol_range * Mathf.PI / 180f;
        ideal_y = transform.position.y;
        angularPhysics.setVerticalSpeed(vertical_speed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //vertical movement to simulate drone
        if (going_up && transform.position.y > ideal_y + up_down_range) {
            going_up = false;
            angularPhysics.setVerticalSpeed(-vertical_speed);
        }
        else if (!going_up && transform.position.y < ideal_y - up_down_range) {
            going_up = true;
            angularPhysics.setVerticalSpeed(vertical_speed);
        }

        // If player not detected, Roam Area
        if (actual_state == STATE.PATROLING)
        {
            //Roam Left
            if (patrolling_to_left)
            {
                angularPhysics.moveObject(-patrolling_speed, 0);
                offset_angle_rotation = 180;
                if (angularPhysics.getActualAngle() < patrol_centerpoint - patrol_range) patrolling_to_left = false;
            }
            //Roam Right
            else
            {
                angularPhysics.moveObject(patrolling_speed, 0);
                offset_angle_rotation = 0;
                if (angularPhysics.getActualAngle() > patrol_centerpoint + patrol_range) patrolling_to_left = true;
            }
        }
        else
        {
            //Move towards or away from player (mantain ideal distance
            float player_ang = GameObject.Find("Player").GetComponent<Angular_Physics>().getActualAngle();
            if (Mathf.Abs(player_ang - angularPhysics.getActualAngle()) * ring_radius < desired_distance_from_player)
            {
                //towards player
                actual_speed = Mathf.Min(max_move_speed, actual_speed + acceleration);
                if (billboard.isFacingRight()) angularPhysics.moveObject(-actual_speed, 0);
                else angularPhysics.moveObject(actual_speed, 0);
            }
            else if (Mathf.Abs(player_ang - angularPhysics.getActualAngle()) * ring_radius > desired_distance_from_player)
            {
                //away from player
                actual_speed = Mathf.Max(-max_move_speed, actual_speed - acceleration);
                if (billboard.isFacingRight()) angularPhysics.moveObject(-actual_speed, 0);
                else angularPhysics.moveObject(actual_speed, 0);
            }

            //Prepare to fire
            if (actual_state == STATE.PLAYER_DETECTED && last_shot_timestamp + time_between_shoots < Time.time)
            {
                bullet_count = 0;
                actual_state = STATE.FIRING;
                animator.PlayInFixedTime("Shoot", 0, 0f);
                offset_angle_rotation = 0;
            }

            // Aim & Fire Projectile
            if (actual_state == STATE.FIRING || actual_state == STATE.FIRING_THEN_IDLE)
            {
                if (Time.time - last_shot_timestamp > time_between_bullets)
                {
                    last_shot_timestamp = Time.time;
                    if (bullet_count == 0)
                    {
                        //Point at player
                        float player_y = GameObject.Find("Player").transform.position.y;
                        //float player_ang = GameObject.Find("Player").GetComponent<Angular_Physics>().getActualAngle();
                        temp_angle_to_player = Mathf.Atan((player_y - Fire_Point.transform.position.y) / Mathf.Abs((player_ang - angularPhysics.getActualAngle()) * ring_radius)) * (180f / Mathf.PI);
                    }
                    bullet_count++;
                    if (bullet_count == bullets_per_pulse)
                    {
                        if (actual_state == STATE.FIRING) actual_state = STATE.PLAYER_DETECTED;
                        else actual_state = STATE.PATROLING;
                    }

                    //fire bullet
                    animator.PlayInFixedTime("Shoot", 0, 0f);
                    GameObject new_bullet = Instantiate(Bullet_Prefab, Fire_Point.transform.position, Fire_Point.transform.rotation);
                    new_bullet.GetComponent<Bullet_Physics>().init(angularPhysics.getActualAngle(), ring_radius, 0f, billboard.isFacingRight(), temp_angle_to_player);
                }
            }
        }

        // Activate billboard (must be done after moving object)
        billboard.turn_to_camera(angularPhysics.getActualAngle(), offset_angle_rotation);
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (actual_state == STATE.PATROLING && hit.tag == "Player")
        {
            actual_state = STATE.PLAYER_DETECTED;
            actual_speed = patrolling_speed;
        }
    }

    private void OnTriggerExit(Collider hit)
    {
        if (hit.tag == "Player")
        {
            if (actual_state == STATE.FIRING) actual_state = STATE.FIRING_THEN_IDLE;
            else if (actual_state == STATE.PLAYER_DETECTED)
            {
                actual_state = STATE.PATROLING;
                offset_angle_rotation = 0;
                patrolling_to_left = true;
                patrol_centerpoint = angularPhysics.getActualAngle();
            }
        }
    }

}
