using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour
{
    enum BATTLE_ACTION { COOLDOWN, TELEPORT, CIRCLE_ATTACK, SQUARE_ATTACK, LINE_ATTACK}

    private BATTLE_ACTION ActualAction = BATTLE_ACTION.COOLDOWN;
    private float last_action_timestamp = 0;
    private float time_cooldown = 0;
    private bool sleeping = true;
    private bool decidedActionThisFrame = false;

    // vertical Movement
    [SerializeField] private float up_down_range = 0.25f;
    [SerializeField] private float vertical_speed = 0.75f;
    [SerializeField] private float y_tp_range = 1f;
    [SerializeField] private float ring_radius = 17f;
    private float y_tp_bottom_limit;
    private float y_tp_upper_limit;
    private float ideal_y;
    private bool going_up = true;

    bool waiting_for_tp = true;

    //RING ID
    public RingIdentifierLogic _RingIdentifierLogic;
    public RingIdentifierLogic PlayerIdentifierLogic;

    private Enemy _EnemyHealth;
    private Animator _Animator;
    private Angular_Physics _Angular_Physics;
    private Billboard_Facing_Player _Billboard;

    // first phase
    [SerializeField] private float start_second_phase = 0.5f;
    [SerializeField] private int actions_between_tp_1st_phase = 3;
    [SerializeField] private int actions_between_tp_2nd_phase = 4;
    [SerializeField] private float tp_cooldown = 3f;
    [SerializeField] private float circle_delay = 0.5f;
    [SerializeField] private float circle_cooldown = 3f;
    [SerializeField] private GameObject CircleProjectile;
    [SerializeField] private float barrel_Length_Offset;
    [SerializeField] private LightPilarController _LightPilarController;
    [SerializeField] private float line_cooldown;
    [SerializeField] private float square_cooldown;
    [SerializeField] private GameObject BossHealthBar;
    private int action_count = 0;

    private void Awake()
    {
        _EnemyHealth = GetComponent<Enemy>();
        _Animator = GetComponent<Animator>();
        _Angular_Physics = GetComponent<Angular_Physics>();
        _Billboard = GetComponent<Billboard_Facing_Player>();
        _LightPilarController = FindAnyObjectByType<LightPilarController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        float actual_angle = Angular_Physics.getAngleFromCoordinades(transform.position.x, transform.position.z);
        //ring_radius = Angular_Physics.getRadiusFromPosition(actual_angle, transform.position.x, transform.position.z);
        _Angular_Physics.init(ring_radius, actual_angle);
        ideal_y = transform.position.y;
        _Angular_Physics.setVerticalSpeed(vertical_speed);

        y_tp_bottom_limit = transform.position.y - y_tp_range;
        y_tp_upper_limit = transform.position.y + y_tp_range;
    }

    // Decide next action
    void DecideOnNextAction()
    {
        int random_action = (int)Random.Range(0, 100);
        if (_EnemyHealth.GetHealthPercent() > start_second_phase)
        {
            // First Phase
            if (action_count == actions_between_tp_1st_phase) {
                action_count = 0;
                ActualAction = BATTLE_ACTION.TELEPORT;
            }
            else if (random_action < 45) ActualAction = BATTLE_ACTION.CIRCLE_ATTACK;
            else ActualAction = BATTLE_ACTION.SQUARE_ATTACK;
        }
        else
        {
            // Second Pahse
            if (action_count == actions_between_tp_2nd_phase)
            {
                action_count = 0;
                ActualAction = BATTLE_ACTION.TELEPORT;
            }
            else if (random_action < 20) ActualAction = BATTLE_ACTION.LINE_ATTACK;
            else if (random_action < 60) ActualAction = BATTLE_ACTION.CIRCLE_ATTACK;
            else ActualAction = BATTLE_ACTION.SQUARE_ATTACK;
        }
        ++action_count;
    }

    private void FixedUpdate()
    {
        //vertical movement to simulate drone
        if (going_up && transform.position.y > ideal_y + up_down_range)
        {
            going_up = false;
            _Angular_Physics.setVerticalSpeed(-vertical_speed);
        }
        else if (!going_up && transform.position.y < ideal_y - up_down_range)
        {
            going_up = true;
            _Angular_Physics.setVerticalSpeed(vertical_speed);
        }


        // Is sleeping
        if (sleeping)
        {
            _Angular_Physics.moveObject(0, 0);
            _Billboard.turn_to_camera(_Angular_Physics.getActualAngle(), 180, true);
            return;
        }


        // WAIT FOR COOLDOWN && START ACTION
        if (ActualAction == BATTLE_ACTION.COOLDOWN)
        {
            if (Time.time - last_action_timestamp > time_cooldown)
            {
                DecideOnNextAction(); // Choose next State
                decidedActionThisFrame = true;
                Debug.Log(ActualAction);
            }
        }

        // PERFORM SQUARE PILAR ATTACK AGAINST PLAYER
        if (ActualAction == BATTLE_ACTION.SQUARE_ATTACK)
        {
            if (decidedActionThisFrame)
            {
                _Animator.Play("Square_Attack", 0, 0f);
                decidedActionThisFrame = false;
                last_action_timestamp = Time.time;
            }
            else if (Time.time - last_action_timestamp > circle_delay)
            {
                _LightPilarController.DeployByGroups(Random.Range(2,6));
                last_action_timestamp = Time.time;
                time_cooldown = square_cooldown;
                ActualAction = BATTLE_ACTION.COOLDOWN;
            }
        }

        // PERFORM LINE PILAR ATTACK AGAINST PLAYER
        if (ActualAction == BATTLE_ACTION.LINE_ATTACK) 
        {
            if (decidedActionThisFrame) 
            {
                _Animator.Play("Line_Attack", 0, 0f);
                decidedActionThisFrame = false;
                last_action_timestamp = Time.time;
            }
            else if (Time.time - last_action_timestamp > circle_delay)
            {
                _LightPilarController.DeployLineAttack();
                last_action_timestamp = Time.time;
                time_cooldown = line_cooldown;
                ActualAction = BATTLE_ACTION.COOLDOWN;
            }
        }

        // PERFORM CIRCLE ATTACK AGAINST PLAYER
        if (ActualAction == BATTLE_ACTION.CIRCLE_ATTACK)
        {
            if (decidedActionThisFrame)
            {
                _Animator.Play("Circle_Attack", 0, 0f);
                decidedActionThisFrame = false;
                last_action_timestamp = Time.time;
            }
            else if (Time.time - last_action_timestamp > circle_delay) 
            {
                float angle = _Angular_Physics.getActualAngle();
                float relative_angle_player = GameObject.Find("Player").GetComponent<Angular_Physics>().getRelativeAngle(angle);
                bool PlayerIsRightSide = (relative_angle_player > Mathf.PI);
                float aux_l = barrel_Length_Offset;
                if (PlayerIsRightSide) aux_l = -aux_l;

                //Instantiate Projectiles
                GameObject projectile = Instantiate(CircleProjectile, transform.GetChild(0).transform.position, transform.rotation);
                projectile.GetComponent<Homing_Bullet_Physics>().init(angle, ring_radius, PlayerIsRightSide, Mathf.PI/4, aux_l);

                projectile = Instantiate(CircleProjectile, transform.GetChild(1).transform.position, transform.rotation);
                projectile.GetComponent<Homing_Bullet_Physics>().init(angle, ring_radius, PlayerIsRightSide, Mathf.PI, 0);

                projectile = Instantiate(CircleProjectile, transform.GetChild(2).transform.position, transform.rotation);
                projectile.GetComponent<Homing_Bullet_Physics>().init(angle, ring_radius, !PlayerIsRightSide, Mathf.PI / 4, -aux_l);

                last_action_timestamp = Time.time;
                time_cooldown = circle_cooldown;
                ActualAction = BATTLE_ACTION.COOLDOWN;
            }
        }


        // PERFORM TELEPORT TO RANDOM SPOT IN RING
        if (ActualAction == BATTLE_ACTION.TELEPORT)
        {
            if (decidedActionThisFrame)
            {
                _Animator.Play("Start_Teleport", 0, 0f);
                waiting_for_tp = true;
                decidedActionThisFrame = false;
            }
            else if (waiting_for_tp && _Animator.GetCurrentAnimatorStateInfo(0).IsName("End_Teleport")) 
            {
                float new_angle = Random.Range(0, Mathf.PI * 2);
                float ang_diff = _Angular_Physics.getRelativeAngle(new_angle);                          //tries to separate start from end
                if (ang_diff < Mathf.PI * 1.5 && ang_diff > Mathf.PI) new_angle += Mathf.PI / 2;
                else if (ang_diff > Mathf.PI * 0.5f && ang_diff < Mathf.PI) new_angle -= Mathf.PI / 2;
                _Angular_Physics.setActualAngle((new_angle + Mathf.PI * 2) % (Mathf.PI * 2));

                // Y movement
                transform.position = new Vector3(transform.position.x, Random.Range(y_tp_bottom_limit, y_tp_upper_limit), transform.position.z);
                ideal_y = transform.position.y;

                last_action_timestamp = Time.time;
                time_cooldown = tp_cooldown;
                ActualAction = BATTLE_ACTION.COOLDOWN;
            }
        }


        //Debug.Log(ActualAction);
        _Angular_Physics.moveObject(0, 0);
        _Billboard.turn_to_camera(_Angular_Physics.getActualAngle(), 180, true);
    }

    public void WakeUpBoss() 
    { 
        sleeping = false;
        BossHealthBar.SetActive(true);
    }
}
