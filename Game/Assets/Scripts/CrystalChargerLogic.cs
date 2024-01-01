using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CrystalChargerLogic : MonoBehaviour
{
    enum STATE { IDLE, PLAYER_DETECTED, FIRING, FIRING_THEN_IDLE }

    [SerializeField] private float time_between_shoots = 1f;
    [SerializeField] private float channeling_time = 1f;
    [SerializeField] private float scale_offset = 1f;
    [SerializeField] private GameObject channeling_bullet;
    [SerializeField] private GameObject Launchable_Projectile_Prefab;

    private STATE actual_state = STATE.IDLE;
    private Vector3 original_scale_bullet;
    private float last_shot_timestamp = 0f;
    private float last_frame = 0f;
    private float max_size_channeling_time;
    private float actual_angle;
    private float ring_radius;

    private void Awake()
    {
        if (channeling_bullet == null) channeling_bullet = transform.GetChild(0).gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        original_scale_bullet = channeling_bullet.transform.localScale;
        channeling_bullet.GetComponent<MeshRenderer>().enabled = false;
        max_size_channeling_time = channeling_time * 4f / 5f;
        actual_angle = (Mathf.Atan(transform.position.z / transform.position.x) + Mathf.PI * 2) % (Mathf.PI * 2);
        ring_radius = Mathf.Abs(transform.position.x / Mathf.Cos(actual_angle));
    }

    // Update is called once per frame
    void Update()
    {
        // Wait for channeling
        if (actual_state == STATE.PLAYER_DETECTED)
        {
            if (last_shot_timestamp + time_between_shoots < Time.time)
            {
                actual_state = STATE.FIRING;
                channeling_bullet.GetComponent<MeshRenderer>().enabled = true;
                channeling_bullet.transform.localScale = original_scale_bullet;
                last_shot_timestamp = Time.time;
            }
        }

        // Channel Bullet & Fire
        if (actual_state == STATE.FIRING || actual_state == STATE.FIRING_THEN_IDLE)
        {
            if (Time.time - last_shot_timestamp < max_size_channeling_time) channeling_bullet.transform.localScale *= (1 + (Time.time - last_frame) * scale_offset);
            else if (Time.time - last_shot_timestamp > channeling_time)
            {
                channeling_bullet.GetComponent<MeshRenderer>().enabled = false;
                last_shot_timestamp = Time.time;
                if (actual_state == STATE.FIRING) actual_state = STATE.PLAYER_DETECTED;
                else actual_state = STATE.IDLE;

                //fire bullet
                GameObject new_bullet = Instantiate(Launchable_Projectile_Prefab, channeling_bullet.transform.position, channeling_bullet.transform.rotation);
                new_bullet.GetComponent<Bullet_Physics>().init(actual_angle, ring_radius, 0f, transform.rotation.y == 0, 0f);
            }
        }

        last_frame = Time.time;
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
            else actual_state = STATE.IDLE;
        }
    }

}
