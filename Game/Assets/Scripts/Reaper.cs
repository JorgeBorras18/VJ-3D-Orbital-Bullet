using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Reaper : MonoBehaviour
{
    enum STATE { PATROLING, PLAYER_DETECTED, CHARGING, SLASHING }

    [SerializeField] private float charge_time = 0.5f;
    [SerializeField] private float time_between_slashes = 1f;
    [SerializeField] private float patrol_range = 10;
    [SerializeField] private float max_charge_range = 10;
    [SerializeField] private float patrolling_speed = 8;
    [SerializeField] private float slash_speed = 20;
    [SerializeField] private int slash_damage = 20;
    [SerializeField] private float gravity = 0.7f;

    private Billboard_Facing_Player billboard;
    private Angular_Physics angularPhysics;
    private TrailRenderer ownTrail;
    private WallDetector wallDetector;

    private Animator animator;
    private STATE actual_state = STATE.PATROLING;
    private float last_charge_timestamp = 0f;
    private float ring_radius;
    private float offset_angle_rotation = 0f;

    private bool going_to_left = false;
    private float patrol_centerpoint;
    private float distance_slashed;
    private bool just_look_at_player = false;
    private float original_radar_size;


    // Start is called before the first frame update
    void Start()
    {
        billboard = transform.parent.gameObject.GetComponent<Billboard_Facing_Player>();
        angularPhysics = transform.parent.gameObject.GetComponent<Angular_Physics>();
        ownTrail = GetComponent<TrailRenderer>();
        animator = transform.parent.gameObject.GetComponent<Animator>();

        float actual_angle = Angular_Physics.getAngleFromCoordinades(transform.position.x, transform.position.z);
        ring_radius = Angular_Physics.getRadiusFromPosition(actual_angle, transform.position.x, transform.position.z);
        angularPhysics.init(ring_radius, actual_angle);
        patrol_centerpoint = actual_angle;
        patrol_range = patrol_range * Mathf.PI / 180f;
        ownTrail.enabled = false;

        //radar detection radius
        original_radar_size = GetComponent<CapsuleCollider>().radius;
        wallDetector = GetComponentInChildren<WallDetector>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool hit_wall = wallDetector.isWallAhead();

        // If player not detected, Roam Area
        if (actual_state == STATE.PATROLING)
        {
            //Roam Left
            if (going_to_left)
            {
                if (!hit_wall) angularPhysics.moveObject(-patrolling_speed, gravity);
                offset_angle_rotation = 180;
                wallDetector.UpdateOrientation(false);
                if (hit_wall || angularPhysics.getActualAngle() < patrol_centerpoint - patrol_range) going_to_left = false;
            }
            //Roam Right
            else
            {
                if (!hit_wall) angularPhysics.moveObject(patrolling_speed, gravity);
                offset_angle_rotation = 0;
                wallDetector.UpdateOrientation(true);
                if (hit_wall || angularPhysics.getActualAngle() > patrol_centerpoint + patrol_range) going_to_left = true;
            }
        }

        //Player detected but not yet slashing
        else if (actual_state == STATE.PLAYER_DETECTED)
        {
            if (Time.time - last_charge_timestamp > time_between_slashes)
            {
                // just_look_at_player = true;
                actual_state = STATE.CHARGING;
                animator.PlayInFixedTime("Charge", 0, 0f);
                last_charge_timestamp = Time.time;
            }
            else if (!hit_wall)
            {
                //move towards playerd
                float relative_angle_player = GameObject.Find("Player").GetComponent<Angular_Physics>().getRelativeAngle(angularPhysics.getActualAngle());
                if (relative_angle_player > Mathf.PI)
                {
                    wallDetector.UpdateOrientation(true);
                    if (relative_angle_player > Mathf.PI * 1.15) angularPhysics.moveObject(patrolling_speed * 1.3f, gravity);
                }
                else if (relative_angle_player < Mathf.PI)
                {
                    wallDetector.UpdateOrientation(false);
                    if (relative_angle_player < Mathf.PI * 0.85) angularPhysics.moveObject(-patrolling_speed * 1.3f, gravity);
                }
            }
        }

        // Charge Slash Animation, Wait for Slash Start Timestamp
        else if (actual_state == STATE.CHARGING) 
        {
            float relative_angle_player = GameObject.Find("Player").GetComponent<Angular_Physics>().getRelativeAngle(angularPhysics.getActualAngle());
            if (relative_angle_player > Mathf.PI) wallDetector.UpdateOrientation(true);
            else wallDetector.UpdateOrientation(false);

            if (Time.time - last_charge_timestamp > charge_time)
            {
                // just_look_at_player = true;
                actual_state = STATE.SLASHING;
                distance_slashed = 0;
                ownTrail.enabled = true;
                just_look_at_player = false;
                if (relative_angle_player > Mathf.PI) slash_speed = Mathf.Abs(slash_speed);
                else slash_speed = Mathf.Abs(slash_speed) * -1;
            }
        }

        else if (actual_state == STATE.SLASHING)
        {
            // Already finished Slashing?
            if (hit_wall || (max_charge_range < distance_slashed))
            {
                just_look_at_player = true;
                actual_state = STATE.PLAYER_DETECTED;
                animator.PlayInFixedTime("Reset", 0, 0f);
                last_charge_timestamp = Time.time;
                ownTrail.enabled = false;
            }
            else
            {
                //keep slashing
                angularPhysics.moveObject(slash_speed, gravity);
                distance_slashed += Mathf.Abs(slash_speed) * Time.deltaTime; 
            }
        }

        // Activate billboard (must be done after moving object)
        billboard.turn_to_camera(angularPhysics.getActualAngle(), offset_angle_rotation, just_look_at_player);
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.tag == "Player")
        {
            if (actual_state == STATE.PATROLING) 
            {
                actual_state = STATE.PLAYER_DETECTED;
                just_look_at_player = true;
                offset_angle_rotation = 0;

                //Change Hitbox Collider to dmg type (smaller)
                GetComponent<CapsuleCollider>().radius = 0.15f;
            }
            else if (actual_state == STATE.SLASHING)
            {
                //Deal Dmg To Player
                hit.GetComponent<PlayerLogic>().TakeDamageAndHitBack(slash_damage);
            }
        }
    }

    private void OnTriggerExit(Collider hit)
    {
        if (hit.tag == "Player" && false) //diferent radius
        {
            actual_state = STATE.PATROLING;
            GetComponent<CapsuleCollider>().radius = original_radar_size;
        }
    }

}
