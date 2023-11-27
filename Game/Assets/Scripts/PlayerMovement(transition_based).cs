using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody rb;
    private CharacterController cController;
    [SerializeField] float horitzontalSpeed = 5;
    [SerializeField] float radiusRing = 9;
    float actualAngle;
    float lastUpdate;
    bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cController = GetComponent<CharacterController>();
        actualAngle = 1.5f * Mathf.PI;
        lastUpdate = 0f;
        isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        // GET PLAYER INPUT VALUES
        bool left_is_pressed = Input.GetKey("left");
        bool right_is_pressed = Input.GetKey("right");

        if (left_is_pressed || right_is_pressed)
        {
            float timeDelta = Time.deltaTime;
            float angular_step = (horitzontalSpeed / radiusRing) * timeDelta;
            if (left_is_pressed)
            {
                actualAngle -= angular_step;
                transform.Rotate(new Vector3(0, 1, 0) * (horitzontalSpeed / radiusRing) * 180 / Mathf.PI * timeDelta);
            }
            else
            {
                actualAngle = actualAngle += angular_step;
                transform.Rotate(new Vector3(0, -1, 0) * (horitzontalSpeed / radiusRing) * 180 / Mathf.PI * timeDelta);
            }

            float new_x = radiusRing * Mathf.Cos(actualAngle);
            float new_z = radiusRing * Mathf.Sin(actualAngle);
            transform.position = new Vector3(new_x, transform.position.y, new_z);
        }

        if (Input.GetKey("up") && rb.velocity.y == 0 && isGrounded)
        {
            rb.velocity = new Vector3(0f, 5f, 0f);
            isGrounded = false;
        }

        Debug.Log(isGrounded);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isGrounded = true;
        }
    }
}
