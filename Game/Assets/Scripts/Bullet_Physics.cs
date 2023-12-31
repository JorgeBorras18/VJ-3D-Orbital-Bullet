using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Bullet_Physics : MonoBehaviour
{


    public float moveSpeed = 10f;
    public float gravity = 0f;
    public float lifetime = 2f;
    public int allowed_bounces = 0;
    public bool explosive = false;

    private float iniTimestamp;
    private bool visible = false;
    private bool destroyed;

    //Angular Movement
    private float radiusRing;
    private float actualAngle;
    private bool facingRight;
    private Vector2 speed;

    private CharacterController controller;
    private GameObject Trail;

    bool aux = true;

    // Start is called before the first frame update
    void Start()
    {
        // GET ORIENTATION, POSITION FROM FIREPOINT...
        Trail = transform.GetChild(1).gameObject;
        controller = GetComponent<CharacterController>();
        Angular_Physics playerAG = GameObject.Find("Player").GetComponent<Angular_Physics>();
        float barrelLengthOffset = GameObject.Find("Gun").GetComponent<Weapon>().getBarrelLengthOffset();
        radiusRing = playerAG.getActualRadius() + Random.Range(-0.5f, 0.5f);

        facingRight = GameObject.Find("Player").GetComponent<PlayerLogic>().isFacingRight();
        if (!facingRight)
        {
            speed.x = moveSpeed * -1f;
            actualAngle = playerAG.getActualAngle() - barrelLengthOffset;
            transform.Rotate(new Vector3(0, 0, 90));
            if (explosive) transform.GetChild(2).transform.position += new Vector3(0, 1, 0);
        }
        else
        {
            speed.x = moveSpeed;
            actualAngle = playerAG.getActualAngle() + barrelLengthOffset;
            transform.Rotate(new Vector3(0, 0, -90));
        }
        iniTimestamp = Time.time;

    }

    // SET VERTICAL ANGULATION
    public void init(float initialDeviation)
    {
        speed.y = moveSpeed * Mathf.Sin(initialDeviation * Mathf.PI / 180f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if bullet was destroyed still render frames
        if (destroyed)
        {
            if ((Time.time - iniTimestamp) > 0.5)
            {
                Destroy(Trail.GetComponent<TrailRenderer>());
                Destroy(this.gameObject);
            }
            else return;
        }

        // Destroy bullet
        if ((Time.time - iniTimestamp) > lifetime)
        {
            destroyBullet();
            return;
        }

        //MOVE BULLET
        float timeDelta = Time.deltaTime;

        // Calculate Horitzontal desplacement && turn object so it faces camera
        actualAngle += (speed.x / radiusRing) * timeDelta;
        float newX = Mathf.Cos(actualAngle) * radiusRing - transform.position.x;
        float newZ = Mathf.Sin(actualAngle) * radiusRing - transform.position.z;
        transform.LookAt(new Vector3(0, transform.position.y, 0));

        //Rotate trajectory acording to direction
        float new_angle = 90f;
        if (facingRight) new_angle = -90f;
        new_angle += Mathf.Atan(speed.y / speed.x) * (180f / Mathf.PI);
        transform.Rotate(new Vector3(0, 0, new_angle));

        // Vertical Movement
        speed.y = Mathf.Max(-10, speed.y - gravity);

        // Move Bullet
        controller.Move(new Vector3(newX, speed.y * timeDelta, newZ));

    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == "Terrain")
        {
            Debug.Log(hit.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position) - transform.position);
            if (allowed_bounces == 0 || (hit.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position) - transform.position).y >= 0) destroyBullet();
            else
            {
                allowed_bounces -= 1;
                speed.y = -speed.y;
            }
        }
    }

    private void destroyBullet()
    {
        destroyed = true;
        iniTimestamp = Time.time;
        GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
        if (explosive) transform.GetChild(2).GetComponent<ParticleSystem>().Play(true);
    }
}