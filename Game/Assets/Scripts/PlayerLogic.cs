using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLogic : MonoBehaviour
{
    public CharacterController controller;
    public Animation_Controller animationController;
    Angular_Physics angularPhysics;
    public BoxCollider boxCollider;
    public PlayerInput _playerInput;
    [SerializeField] private int player_health = 125;
    [SerializeField] private PlayerHealthBar PlayerHealthBar;

    // DMG Handlers
    public float IFramesDuration = 1.5f;
    private float timestamp_last_dmg_taken = 0;
    private DMG_Flash _DamageFlashEffect;
    private FeetDetector _FeetDetector;

    // PHYSISCS VALUES
    public float moveSpeed = 5f, jumpSpeed = 10f, rollSpeed = 10f, slow_fall_gravity = 0.45f, fast_fall_gravity = 0.7f;
    public float radiusRing = 17f;
    private bool isThereWallAhead = false;
    private bool isThereEnemyAhead = false;

    //INPUT VALUES
    private InputAction MoveAction;
    private InputAction RollAction;
    private bool roll_was_pressed = false;
    private InputAction JumpAction;
    private bool up_was_pressed = false;
    private InputAction CrouchAction;

    // MOVEMENT STAT VARIABLES LOGIC
    private bool doubled_jumped_already = false;
    private bool up_is_still_pressed = false;
    private bool facingRight = false;
    private bool RollingUpwards = false;
    private bool movement_is_blocked = false;

    // NEXT RING JUMPING PHYSICS
    private bool jumping_to_the_next_ring = false;
    public int max_jump_frames;
    private int current_jump_frames = 0;

    // INTERNAL-EXTERNAL RING MOVEMENT
    private bool inInternalOrExternalPlatform;
    public bool jumping_internally_or_externally;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        controller = GetComponent<CharacterController>();
        _DamageFlashEffect = GetComponent<DMG_Flash>();
        animationController = GameObject.Find("Player_Animation_Controller").gameObject.GetComponent<Animation_Controller>();
        angularPhysics = GetComponent<Angular_Physics>();
        angularPhysics.init(radiusRing, 0);

        //load input watchers
        _playerInput = GetComponent<PlayerInput>();
        MoveAction = _playerInput.actions["Move"];
        RollAction = _playerInput.actions["Roll"];
        JumpAction = _playerInput.actions["Jump"];
        CrouchAction = _playerInput.actions["Crouch"];

        //set health
        PlayerHealthBar = FindObjectOfType<PlayerHealthBar>();
        PlayerHealthBar.SetMaxHealth(player_health);
        _FeetDetector = GetComponentInChildren<FeetDetector>();
    }

    // Catch 
    private void Update()
    {
        checkIfMovementIsBlocked();

        if (!movement_is_blocked)
        {
            up_was_pressed = up_was_pressed || JumpAction.WasPressedThisFrame();
            roll_was_pressed = roll_was_pressed || RollAction.WasPressedThisFrame();
        }
    }

    // Physics Update is called once every 0.02 seconds
    void FixedUpdate()
    {
        float selected_gravity = fast_fall_gravity;
        float step = 0;
        string next_Animation = "Jump";
        float movementInput = MoveAction.ReadValue<Vector2>().x;

        //Check animation
        if (animationController.getActualState() == "Death") return;
        if (RollingUpwards)
        {
            if (animationController.getActualState() == "Fast_Roll" && !animationController.animationHasFinished()) next_Animation = "Fast_Roll";
            else
            {
                next_Animation = "Air_Fall";
                RollingUpwards = false;
            }
        }
        else if (animationController.getActualState() == "Roll" && !animationController.animationHasFinished())
        {
            if (isThereWallAhead) angularPhysics.moveObject(0, selected_gravity);
            else
            {
                if (facingRight) angularPhysics.moveObject(rollSpeed, selected_gravity);
                else angularPhysics.moveObject(-rollSpeed, selected_gravity);
            }
            return;
        }

        if (!movement_is_blocked)
        {
            
            // SELECT GRAVITY
            if (up_was_pressed)
            {
                if (controller.isGrounded)
                {
                    angularPhysics.applyJump(jumpSpeed);
                    up_is_still_pressed = true;
                    doubled_jumped_already = false;
                    next_Animation = "Jump";
                }
                else if (!doubled_jumped_already)
                {
                    angularPhysics.applyJump(jumpSpeed);
                    up_is_still_pressed = true;
                    doubled_jumped_already = true;
                    next_Animation = "Mid_Air_Jump";
                }
                up_was_pressed = false;
            }

            if (!controller.isGrounded)
            {
                if (!RollingUpwards && angularPhysics.getVerticalSpeed() <= jumpSpeed * 0.4f) next_Animation = "Air_Fall";
                if (up_is_still_pressed)
                {
                    if (JumpAction.IsPressed()) selected_gravity = slow_fall_gravity;
                    else up_is_still_pressed = false;
                }
            }

            // HORIZONTAL MOVEMENT
            if (movementInput == -1f)
            {
                step = -moveSpeed;
                if (facingRight == true)
                {
                    facingRight = false;
                    animationController.flipX(false);
                    boxCollider.center = new Vector3(-0.04f, 0.038f, 0);
                }
                else if (isThereWallAhead || isThereEnemyAhead) step = 0;
                if (controller.isGrounded) next_Animation = "Run";
            }
            else if (movementInput == 1f)
            {
                step = moveSpeed;
                if (facingRight == false)
                {
                    facingRight = true;
                    animationController.flipX(true);
                    boxCollider.center = new Vector3(0.04f, 0.038f, 0);
                }
                else if (isThereWallAhead || isThereEnemyAhead) step = 0;
                if (controller.isGrounded) next_Animation = "Run";
            }
            else if (controller.isGrounded)
            {
                if (CrouchAction.IsPressed()) next_Animation = "Crouch";
                else next_Animation = "Idle";
            }


            if (roll_was_pressed)
            {
                next_Animation = "Roll";
                roll_was_pressed = false;
                isThereEnemyAhead = false;
            }

            // KILL PLAYER
            //if (Input.GetKey(KeyCode.X)) animationController.setAlive(false);

        }
        else if (jumping_to_the_next_ring)
        {
            if (current_jump_frames <= max_jump_frames) {
                angularPhysics.applyJump(jumpSpeed * 3.7f);
                next_Animation = "Jump";
                ++current_jump_frames;
            }
            else
            {
                jumping_to_the_next_ring = false;
                current_jump_frames = 0;
            }
        }
        else if (jumping_internally_or_externally)
        {
            angularPhysics.applyJump(jumpSpeed * 2.0f);
            next_Animation = "Jump";
            jumping_internally_or_externally = false;
        }

        // UPDATE PLAYER MOVEMENT & ANIMATION
        angularPhysics.moveObject(step, selected_gravity);
        animationController.changeAnimation(next_Animation);

    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == ("Terrain"))
            isThereWallAhead = true;

        else if (hit.gameObject.tag == ("Enemy"))
            isThereEnemyAhead = true;

        else if (hit.gameObject.tag == ("Platform"))
        {
            inInternalOrExternalPlatform = true;
        }
    }

    private void OnTriggerExit(Collider hit)
    {
        if (hit.gameObject.tag == ("Terrain"))
            isThereWallAhead = false;

        else if (hit.gameObject.tag == ("Enemy")) 
            isThereEnemyAhead = false;

        else if (hit.gameObject.tag == ("Platform")) 
            inInternalOrExternalPlatform = false;
    }

    public bool TriggerUppwardRollAfterEnemyStomp()
    {
        // HIT ENEMY AND PROPEL UPWARD
        if (_FeetDetector.isThereEnemyBellow() && angularPhysics.getVerticalSpeed() < 0 && Time.time - timestamp_last_dmg_taken > IFramesDuration)
        {
            RollingUpwards = true;
            animationController.changeAnimation("Fast_Roll");
            doubled_jumped_already = false;
            angularPhysics.applyJump(jumpSpeed * 1.25f);
            return true;
        }
        return false;
    }

    public bool isFacingRight() { return facingRight; }


    // Take DMG and handle death
    public bool TakeDamage(int damage)
    {
        if (player_health > 0 && Time.time - timestamp_last_dmg_taken > IFramesDuration && animationController.getActualState() != "Roll")
        {
            if (animationController.getActualState() != "Fast_Roll") animationController.changeAnimation("Air_Fall");
            player_health -= damage;
            PlayerHealthBar.TakeDamage(damage);
            timestamp_last_dmg_taken = Time.time;

            //Die
            if (player_health <= 0) animationController.changeAnimation("Death");

            //activate Iframes
            else
            {
                _DamageFlashEffect.GenerateDamageFlash();
                return true;
            }
        }
        return false;
        
    }

    public bool TakeDamageAndHitBack(int damage)
    {
        if (TakeDamage(damage))
        {
            angularPhysics.applyJump(jumpSpeed*2/3);
            animationController.changeAnimation("Jump");
            doubled_jumped_already = false;
            return true;
        }
        return false;
    }

    private void checkIfMovementIsBlocked()
    {
        movement_is_blocked = jumping_to_the_next_ring || jumping_internally_or_externally; // ... || ... || ... missing
    }

    public void triggerToJumpToTheNextRing()
    {
        jumping_to_the_next_ring = true;
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public bool canChangeToInternalOrExternalRing() {
        return inInternalOrExternalPlatform;
    }

    public void changeToInternalOrExternalRing(float newRadius) {
        jumping_internally_or_externally = true;
        angularPhysics.setActualRadius(newRadius);
    }
}