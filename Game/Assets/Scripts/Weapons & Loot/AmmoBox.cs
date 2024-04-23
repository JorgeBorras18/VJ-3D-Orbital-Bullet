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

    private void Start()
    {
        //Discover what Ammo Type Am I?
        Gun_Controller gun_Controller = GameObject.FindAnyObjectByType<Gun_Controller>();
        if (gun_Controller == null)
        {
            //Player is performin Roll, must take extra steps
            GameObject shoulder = GameObject.Find("Player_Animation_Controller").transform.GetChild(0).gameObject;
            shoulder.SetActive(true);
            ammo_type = shoulder.GetComponent<Gun_Controller>().getRandomAvailableAmmoType();
            shoulder.SetActive(false);
        }
        else ammo_type = gun_Controller.getRandomAvailableAmmoType();

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
                Gun_Controller temp = FindAnyObjectByType<Gun_Controller>();
                if (temp != null) {
                    temp.addAmmoToCurrentWeapons(ammo_type);
                    Destroy(transform.parent.gameObject);
                }
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
