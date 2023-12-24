using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{


    public float moveSpeed = 10f;
    public float gravity = 0f;
    public float lifetime = 2f;
    private Angular_Physics angularPhysics;
    private float iniTimestamp;
    private bool visible = false;

    private GameObject Trail;
    bool destroyed;

    // Start is called before the first frame update
    void Start()
    {
        angularPhysics = GetComponent<Angular_Physics>();
        Trail = transform.GetChild(1).gameObject;
        Angular_Physics playerAG = GameObject.Find("Player").GetComponent<Angular_Physics>();
        float barrelLengthOffset = GameObject.Find("Gun").GetComponent<Weapon>().getBarrelLengthOffset();

        if (!GameObject.Find("Player").GetComponent<PlayerLogic>().isFacingRight())
        {
            moveSpeed *= -1f;
            angularPhysics.init(playerAG.getActualRadius(), playerAG.getActualAngle() - barrelLengthOffset, 90f);
            transform.Rotate(new Vector3(0, 0, 90));
        }
        else
        {
            angularPhysics.init(playerAG.getActualRadius(), playerAG.getActualAngle() + barrelLengthOffset, -90f);
            transform.Rotate(new Vector3(0, 0, -90));
        }
        iniTimestamp = Time.time;

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
        if ((Time.time - iniTimestamp) > lifetime) destroyBullet();

        angularPhysics.moveObject(moveSpeed, gravity);

    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == "Terrain") destroyBullet();
    }

    private void destroyBullet()
    {
        destroyed = true;
        iniTimestamp = Time.time;
        GetComponent<MeshRenderer>().enabled = false;
    }
}
