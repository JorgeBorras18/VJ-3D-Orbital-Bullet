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

    // PHYSISCS VALUES
    public float moveSpeed = 5f, jumpSpeed = 10f, rollSpeed = 10f, slow_fall_gravity = 0.45f, fast_fall_gravity = 0.7f;
    public float radiusRing = 9f;
    private bool isThereWallAhead = false;

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

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        controller = GetComponent<CharacterController>();
        animationController = GameObject.Find("Player_Animation_Controller").gameObject.GetComponent<Animation_Controller>();
        angularPhysics = GetComponent<Angular_Physics>();
        angularPhysics.init(radiusRing, 3f * Mathf.PI / 4f, 0f);

        //load input watchers
        _playerInput = GetComponent<PlayerInput>();
        MoveAction = _playerInput.actions["Move"];
        RollAction = _playerInput.actions["Roll"];
        JumpAction = _playerInput.actions["Jump"];
        CrouchAction = _playerInput.actions["Crouch"];
    }

    // Catch 
    private void Update()
    {
        up_was_pressed = up_was_pressed || JumpAction.WasPressedThisFrame();
        roll_was_pressed = roll_was_pressed || RollAction.WasPressedThisFrame();
    }

    // Physics Update is called once every 0.02 seconds
    void FixedUpdate()
    {

        float selected_gravity = fast_fall_gravity;
        float step = 0;
        string next_Animation = "Jump";
        float movementInput = MoveAction.ReadValue<Vector2>().x;

        //Check animation
        if (animationController.getActualState() == "Roll" && !animationController.animationHasFinished())
        {
            if (isThereWallAhead) angularPhysics.moveObject(0, selected_gravity);
            else
            {
                if (facingRight) angularPhysics.moveObject(rollSpeed, selected_gravity);
                else angularPhysics.moveObject(-rollSpeed, selected_gravity);
            }
            return;
        }

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
            if (angularPhysics.getVerticalSpeed() <= jumpSpeed * 0.4f) next_Animation = "Air_Fall";
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
            else if (isThereWallAhead) step = 0;
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
            else if (isThereWallAhead) step = 0;
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
        }


        // UPDATE PLAYER MOVEMENT & ANIMATION
        angularPhysics.moveObject(step, selected_gravity);
        animationController.changeAnimation(next_Animation);

        // KILL PLAYER
        //if (Input.GetKey(KeyCode.X)) animationController.setAlive(false);
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == ("Terrain"))
        {
            isThereWallAhead = true;
        }
    }

    private void OnTriggerExit(Collider hit)
    {
        if (hit.gameObject.tag == ("Terrain"))
        {
            isThereWallAhead = false;
        }
    }

    public bool isFacingRight() { return facingRight; }
}

public enum anim
{
    IDLE,
    RUNNING,
    JUMP,
    AIR_JUMP,
}