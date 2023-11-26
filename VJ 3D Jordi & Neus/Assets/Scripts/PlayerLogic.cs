using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    public CharacterController controller;
    public Animation_Controller animationController;
    Angular_Physics angularPhysics;

    public float moveSpeed = 5f, jumpSpeed = 10f, slow_fall_gravity = 0.45f, fast_fall_gravity = 0.7f;
    public float radiusRing = 9f;

    // JUMP LOGIC
    private bool doubled_jumped_already = false;
    private bool up_was_pressed = false;
    private bool up_is_still_pressed = false;
    private bool facingRight = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animationController = GameObject.Find("Player_Animation_Controller").gameObject.GetComponent<Animation_Controller>();
        angularPhysics = GetComponent<Angular_Physics>();
        angularPhysics.init(radiusRing, 3f * Mathf.PI / 4f);
    }

    // Catch 
    private void Update()
    {
        up_was_pressed = up_was_pressed || (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow));
    }

    // Physics Update is called once every 0.02 seconds
    void FixedUpdate()
    {

        float selected_gravity = fast_fall_gravity;
        float step = 0;
        int next_Animation = 2;

        // SELECT GRAVITY
        if (up_was_pressed)
        {
            if (controller.isGrounded)
            {
                angularPhysics.applyJump(jumpSpeed);
                up_is_still_pressed = true;
                doubled_jumped_already = false;
                next_Animation = 2;
            }
            else if (!doubled_jumped_already)
            {
                angularPhysics.applyJump(jumpSpeed);
                up_is_still_pressed = true;
                doubled_jumped_already = true;
                next_Animation = 3;
            }
            up_was_pressed = false;
        }

        if (!controller.isGrounded && angularPhysics.getVerticalSpeed() <= jumpSpeed * 0.4f)
            next_Animation = 4;

        // FAST OR SLOW FALL
        if (up_is_still_pressed)
        {
            if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))) selected_gravity = slow_fall_gravity;
            else up_is_still_pressed = false;
        }


        // HORIZONTAL MOVEMENT
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            step = -moveSpeed;
            if (facingRight == true)
            {
                facingRight = false;
                animationController.flipX(false);
            }
            if (controller.isGrounded) next_Animation = 1;
        }

        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            step = moveSpeed;
            if (facingRight == false)
            {
                facingRight = true;
                animationController.flipX(true);
            }
            if (controller.isGrounded) next_Animation = 1;
        }
        else if (controller.isGrounded) next_Animation = 0;

        // UPDATE PLAYER MOVEMENT & ANIMATION
        angularPhysics.moveObject(step, selected_gravity);
        animationController.changeAnimation(next_Animation);


        // KILL PLAYER
        if (Input.GetKey(KeyCode.X)) animationController.setAlive(false);
    }
}

public enum anim
{
    IDLE,
    RUNNING,
    JUMP,
    AIR_JUMP,
}