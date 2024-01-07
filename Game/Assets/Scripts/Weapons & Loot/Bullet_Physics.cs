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
    public int damage_per_bullet = 0;

    public bool enemy_bullet = false;
    private float iniTimestamp;
    private bool destroyed;

    //Angular Movement
    private float radiusRing;
    private float actualAngle;
    private bool facingRight;
    private Vector2 speed;

    private CharacterController controller;
    private GameObject Trail;

    // Start is called before the first frame update
    void Start()
    {
        // GET ORIENTATION, POSITION FROM FIREPOINT...
        Trail = transform.GetChild(1).gameObject;
        controller = GetComponent<CharacterController>();
        iniTimestamp = Time.time;
    }

    // SET VERTICAL ANGULATION
    public void init(float ini_angle, float ini_radius, float barrel_length_offset, bool isFacingRight, float initialDeviation)
    {
        radiusRing = ini_radius;
        actualAngle = ini_angle;
        facingRight = isFacingRight;
        if (!facingRight)
        {
            speed.x = moveSpeed * -1f;
            actualAngle -= barrel_length_offset;
            transform.Rotate(new Vector3(0, 0, 90));
            if (explosive) transform.GetChild(2).transform.position += new Vector3(0, 1, 0);
        }
        else
        {
            speed.x = moveSpeed;
            actualAngle += barrel_length_offset;
            transform.Rotate(new Vector3(0, 0, -90));
        }
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

        // Calculate Horitzontal desplacement && turn object so it faces center
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
        speed.y = Mathf.Max(-2*Mathf.Abs(speed.x), speed.y - gravity);

        // Move Bullet
        controller.Move(new Vector3(newX, speed.y * timeDelta, newZ));
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == "Terrain")
        {
            if (allowed_bounces == 0 || (hit.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position) - transform.position).y >= 0) destroyBullet();
            else
            {
                allowed_bounces -= 1;
                speed.y = -speed.y;
            }
        }
        else if (enemy_bullet && hit.gameObject.tag == "Player")
        {
            bool dealt_dmg = false;
            if (transform.lossyScale.x > 0.5) dealt_dmg = hit.GetComponent<PlayerLogic>().TakeDamageAndHitBack(damage_per_bullet);
            else
            {
                dealt_dmg = hit.GetComponent<PlayerLogic>().TakeDamage(damage_per_bullet);
                if (dealt_dmg) destroyBullet();
            }
        }

        else if (!enemy_bullet && hit.gameObject.tag == "Enemy")
        {
            hit.GetComponent<Enemy>().TakeDamage(damage_per_bullet, GetHashCode());
            destroyBullet();
        }
    }

    private void destroyBullet()
    {
        destroyed = true;
        iniTimestamp = Time.time;
        GetComponent<MeshRenderer>().enabled = false;
        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
        if (explosive)
        {
            GetComponent<BoxCollider>().size = new Vector3(3, 3, 3);
            transform.GetChild(2).GetComponent<ParticleSystem>().Play(true);
        }
    }
}
