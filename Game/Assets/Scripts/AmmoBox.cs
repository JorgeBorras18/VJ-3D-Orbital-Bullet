using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public enum AMMO_TYPE { BULLET, ENERGY, EXPLOSIVE, SHELL }

    private AMMO_TYPE ammo_type;
    private Transform playerTransform;
    private bool FollowingPlayer = false;

    //Ammo Types Sprites && Bullet Quantities
    [SerializeField] private Sprite AmmoBulletBoxSprite;
    [SerializeField] private Sprite AmmoEnergyBoxSprite;
    [SerializeField] private Sprite AmmoExplosiveBoxSprite;
    [SerializeField] private Sprite AmmoShellBoxSprite;

    // Physiscs
    [SerializeField] private float minimum_distance_pickup = 0.5f;
    [SerializeField] private float acceleration = 0.01f;
    [SerializeField] private float max_speed = 0.3f;
    private float actual_speed=0;
    int count = 0;

    private void Start()
    {
        //Discover what Ammo Type Am I?
        ammo_type = GameObject.FindAnyObjectByType<Gun_Controller>().getRandomAvailableAmmoType();

        if (ammo_type == AMMO_TYPE.BULLET) {
            transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite = AmmoBulletBoxSprite;
        }

        else if (ammo_type == AMMO_TYPE.ENERGY) {
            transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite = AmmoEnergyBoxSprite;
        }
        else if (ammo_type == AMMO_TYPE.EXPLOSIVE) {
            transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite = AmmoExplosiveBoxSprite;
        }
        else if (ammo_type == AMMO_TYPE.SHELL) {
            transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite = AmmoShellBoxSprite;
        }
    }

    private void FixedUpdate()
    {
        if (FollowingPlayer)
        {
            if (Vector3.Distance(transform.position, playerTransform.transform.position) <= minimum_distance_pickup)
            {
                FindAnyObjectByType<Gun_Controller>().addAmmoToCurrentWeapons(ammo_type);
                Destroy(transform.parent.gameObject);
            }
            else
            {
                actual_speed = Mathf.Min(max_speed, actual_speed + acceleration);
                transform.parent.transform.position += (playerTransform.position - transform.position).normalized * actual_speed * Time.deltaTime;
            }
        }
    }

    public AMMO_TYPE GetAmunitionType() { return ammo_type; }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.tag == "Player")
        {
            playerTransform = hit.transform;
            FollowingPlayer = true;
            transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
