using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class TransformMovement : MonoBehaviour
{
    private CharacterController cController;
    private Vector3 speed;

    public float moveSpeed = 5.0f, jumpSpeed = 5.0f, gravity = 0.3f;
    public float radiusRing = 9f;
    [SerializeField] private float actualAngle = 1.5f * Mathf.PI;

    private bool midAir = false;
    private float verticalSpeed = 0f;
    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        // LATERAL MOVEMENT
        speed.x = 0;
        speed.z = 0;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            actualAngle -= (moveSpeed / radiusRing) * Time.deltaTime;
            speed.x = Mathf.Cos(actualAngle) * radiusRing - transform.position.x;
            speed.z = Mathf.Sin(actualAngle) * radiusRing - transform.position.z;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            actualAngle += (moveSpeed / radiusRing) * Time.deltaTime;
            speed.x = Mathf.Cos(actualAngle) * radiusRing - transform.position.x;
            speed.z = Mathf.Sin(actualAngle) * radiusRing - transform.position.z;
        }
        transform.LookAt(new Vector3(0, transform.position.y, 0));


        // JUMPING MOVEMENT
        // JUMPING MOVEMENT (correct bug where jump is applied multiple times)
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && controller.isGrounded) verticalSpeed = jumpSpeed;
        else verticalSpeed = Mathf.Max(-10, verticalSpeed - gravity);

        controller.Move(new Vector3(speed.x, verticalSpeed * Time.deltaTime, speed.z));

        Debug.Log(verticalSpeed);
    }
}