using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CrystalCharger : MonoBehaviour
{
    enum STATE { IDLE, PLAYER_DETECTED, FIRING, FIRING_THEN_IDLE }

    [SerializeField] private float time_between_shoots = 1f;
    [SerializeField] private float channeling_time = 1f;
    [SerializeField] private float scale_offset = 1f;
    [SerializeField] private GameObject channeling_bullet;
    [SerializeField] private GameObject Launchable_Projectile_Prefab;
    [SerializeField] private Static_Billboard_Facing_Player billboard;
    [SerializeField] private float ring_radius = 17f;

    private STATE actual_state = STATE.IDLE;
    private Vector3 original_scale_bullet;
    private float last_shot_timestamp = 0f;
    private float last_frame = 0f;
    private float max_size_channeling_time;
    private float actual_angle;
    private float temp_angle_to_player;
    private bool calc_angle_player = true;

    public RingIdentifierLogic ringIdentifierLogic;
    public RingIdentifierLogic playerRingIdentifierLogic;

    private PlaySound soundEffects;
    public AudioSource player;
    public AudioClip shootSound;

    private void Awake()
    {
        if (channeling_bullet == null) channeling_bullet = transform.GetChild(0).gameObject;
        if (billboard == null) billboard = transform.parent.gameObject.GetComponent<Static_Billboard_Facing_Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
        original_scale_bullet = channeling_bullet.transform.localScale;
        channeling_bullet.GetComponent<MeshRenderer>().enabled = false;
        max_size_channeling_time = channeling_time * 4f / 5f;
        actual_angle = Angular_Physics.getAngleFromCoordinades(transform.position.x, transform.position.z);
        Angular_Physics angularPhysics = transform.parent.gameObject.GetComponent<Angular_Physics>();
        angularPhysics.init(ring_radius, actual_angle);
        angularPhysics.moveObject(0, 0);
        //ring_radius = Angular_Physics.getRadiusFromPosition(actual_angle, transform.position.x, transform.position.z);

        // sound
        soundEffects = player.GetComponent<PlaySound>();
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
                soundEffects.playThisSoundEffect(shootSound);
                channeling_bullet.GetComponent<MeshRenderer>().enabled = true;
                channeling_bullet.transform.localScale = original_scale_bullet;
                last_shot_timestamp = Time.time;
                calc_angle_player = true;
            }
        }

        // Channel Bullet & Fire
        if (actual_state == STATE.FIRING || actual_state == STATE.FIRING_THEN_IDLE)
        {
            if (Time.time - last_shot_timestamp < max_size_channeling_time) channeling_bullet.transform.localScale *= (1 + (Time.time - last_frame) * scale_offset);
            else if (calc_angle_player && Time.time - last_shot_timestamp > channeling_time - 0.15)
            {
                //locate player
                float player_y = GameObject.Find("Player").transform.position.y;
                float player_ang = GameObject.Find("Player").GetComponent<Angular_Physics>().getActualAngle();
                temp_angle_to_player = Mathf.Atan((player_y - channeling_bullet.transform.position.y) / Mathf.Abs((player_ang - actual_angle) * ring_radius)) * (180f / Mathf.PI);
                calc_angle_player = false;
            }
            else if (Time.time - last_shot_timestamp > channeling_time)
            {
                channeling_bullet.GetComponent<MeshRenderer>().enabled = false;
                last_shot_timestamp = Time.time;
                if (actual_state == STATE.FIRING) actual_state = STATE.PLAYER_DETECTED;
                else actual_state = STATE.IDLE;

                //fire bullet
                GameObject new_bullet = Instantiate(Launchable_Projectile_Prefab, channeling_bullet.transform.position, channeling_bullet.transform.rotation);
                new_bullet.GetComponent<Bullet_Physics>().init(actual_angle, ring_radius, 0f, billboard.isFacingRight(), temp_angle_to_player);
                temp_angle_to_player = -90;
            }
        }
        last_frame = Time.time;
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (actual_state == STATE.IDLE && ringIdentifierLogic.sameRingAs(playerRingIdentifierLogic) && hit.tag == "Player")
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
