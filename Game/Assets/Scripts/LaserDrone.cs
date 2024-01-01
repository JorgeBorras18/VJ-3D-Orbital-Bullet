using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LaserDrone : MonoBehaviour
{
    enum STATE { IDLE, PLAYER_DETECTED, FIRING, FIRING_THEN_IDLE }

    [SerializeField] private float time_between_shoots = 1f;
    [SerializeField] private float time_between_bullets = 0.3f;
    [SerializeField] private int bullets_per_pulse = 3;
    [SerializeField] private GameObject Bullet_Prefab;
    [SerializeField] private Transform Fire_Point;
    [SerializeField] private Static_Billboard_Facing_Player billboard;

    private STATE actual_state = STATE.IDLE;
    private float last_shot_timestamp = 0f;
    private float actual_angle;
    private float ring_radius;
    private float bullet_count;

    // Start is called before the first frame update
    void Start()
    {
        if (Fire_Point == null) Fire_Point = transform.GetChild(0).transform;
        if (billboard == null) billboard = transform.parent.gameObject.GetComponent<Static_Billboard_Facing_Player>();
        actual_angle = (Mathf.Atan(transform.position.z / transform.position.x) + Mathf.PI * 2) % (Mathf.PI * 2);
        ring_radius = Mathf.Max(Mathf.Abs(transform.position.z / Mathf.Sin(actual_angle)) , Mathf.Abs(transform.position.x / Mathf.Cos(actual_angle)));
    }

    // Update is called once per frame
    void Update()
    {
        // Wait Cooldown
        if (actual_state == STATE.PLAYER_DETECTED && last_shot_timestamp + time_between_shoots < Time.time)
        {
            bullet_count = 0;
            actual_state = STATE.FIRING;
        }

        // Channel Bullet & Fire
        if (actual_state == STATE.FIRING || actual_state == STATE.FIRING_THEN_IDLE)
        {
            if (Time.time - last_shot_timestamp > time_between_bullets)
            {
                last_shot_timestamp = Time.time;
                bullet_count++;
                if (bullet_count == bullets_per_pulse)
                {
                    if (actual_state == STATE.FIRING) actual_state = STATE.PLAYER_DETECTED;
                    else actual_state = STATE.IDLE;
                }

                //fire bullet
                GameObject new_bullet = Instantiate(Bullet_Prefab, Fire_Point.transform.position, Fire_Point.transform.rotation);
                new_bullet.GetComponent<Bullet_Physics>().init(actual_angle, ring_radius, 0f, billboard.isFacingRight(), 0f);
            }
        }
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (actual_state == STATE.IDLE && hit.tag == "Player")
        {
            actual_state = STATE.PLAYER_DETECTED;
        }
    }

    private void OnTriggerExit(Collider hit)
    {
        if (hit.tag == "Player")
        {
            if (actual_state == STATE.FIRING) actual_state = STATE.FIRING_THEN_IDLE;
            else if (actual_state == STATE.PLAYER_DETECTED) actual_state = STATE.IDLE;
        }
    }

}
