using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    private PlayerInput _playerInput;
    private InputAction shootAction;
    [SerializeField] private Angular_Physics playerAG;
    [SerializeField] private PlayerLogic playerLogic;
    [SerializeField] private GameObject DropableWeaponPrefab;

    public float fireRate = 1f;
    public int magazineSize = 20;
    public float upperAccuracyLimit = 0f;
    public float lowerAccuracyLimit = 0f;
    public float barrelLength = 0f;
    public bool automatic = false;
    public bool infinite_ammo = false;


    //For certain weapons
    public int bullets_per_shot = 1;
    public float bullet_spread = 0.1f;

    private float lastShotTimestamp;
    private int shotsInChamber = -1;
    private bool released_trigger;

    void Start()
    {
        if (playerAG == null) playerAG = GameObject.Find("Player").GetComponent<Angular_Physics>();
        if (playerLogic == null) playerLogic = GameObject.Find("Player").GetComponent<PlayerLogic>();
        _playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
        shootAction = _playerInput.actions["Fire"];

        lastShotTimestamp = Time.time;
        if (shotsInChamber == -1) shotsInChamber = magazineSize * 2; //If initialized by Droppable then we don't want to rewrite the value

        //Assert correct values & no break
        lowerAccuracyLimit = Mathf.Min(lowerAccuracyLimit, upperAccuracyLimit);
        upperAccuracyLimit = Mathf.Max(lowerAccuracyLimit, upperAccuracyLimit);
    }


    // Update is called once per frame
    void Update()
    {
        if (!shootAction.IsPressed()) released_trigger = true;
        else if (shootAction.IsPressed() && (shotsInChamber > 0 || infinite_ammo) && (Time.time - lastShotTimestamp) > fireRate && (automatic || (released_trigger && !automatic)))
        {
            Shoot();
            released_trigger = false;
            lastShotTimestamp = Time.time;
            shotsInChamber--;
        }
    }

    void Shoot()
    {
        float ini_angle = playerAG.getActualAngle();
        float ring_radius = playerAG.getActualRadius() + Random.Range(-0.5f, 0.5f);
        bool PlayerFacingRight = playerLogic.isFacingRight();

        // 1 SHOT
        if (bullets_per_shot == 1)
        {
            GameObject new_bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            new_bullet.GetComponent<Bullet_Physics>().init(ini_angle, ring_radius, barrelLength,  PlayerFacingRight, Random.Range(lowerAccuracyLimit, upperAccuracyLimit));
        }

        // FIRE MANY SHOTS
        else
        {
            float angle_per_bullet = (upperAccuracyLimit - lowerAccuracyLimit) / bullets_per_shot;
            for (int i = 0; i < bullets_per_shot; i++)
            {
                GameObject new_bullet = Instantiate(bulletPrefab, firePoint.position + new Vector3(0, (-bullets_per_shot / 2 + i) * bullet_spread, 0), firePoint.rotation);
                new_bullet.GetComponent<Bullet_Physics>().init(ini_angle, ring_radius, barrelLength, PlayerFacingRight, lowerAccuracyLimit + angle_per_bullet * i * Random.Range(0.7f, 1.3f));
            }
        }
        FindObjectOfType<WeaponsControllerUI>().DecreaseAmmoByOne();
    }

    public float getBarrelLengthOffset() { return barrelLength; }

    public int getBulletsLeftInMagazine() { return shotsInChamber; }

    public void setBulletsLeftInMagazien(int bullet_amount) { shotsInChamber = bullet_amount; }

    public GameObject getDroppableVersion() { return DropableWeaponPrefab; }
}
