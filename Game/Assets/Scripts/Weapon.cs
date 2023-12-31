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

    public float fireRate = 1f;
    public float reloadSpeed = 2f;
    public float magazineSize = 20;
    public float upperAccuracyLimit = 0f;
    public float lowerAccuracyLimit = 0f;
    public float barrelLength = 0f;
    public bool automatic = false;


    //For certain weapons
    public int bullets_per_shot = 1;
    public float bullet_spread = 0.1f;

    private float lastShotTimestamp;
    private float shotsInChamber;
    private bool released_trigger;

    void Start()
    {
        _playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
        shootAction = _playerInput.actions["Fire"];

        lastShotTimestamp = Time.time;
        shotsInChamber = magazineSize;

        //Assert correct values & no break
        lowerAccuracyLimit = Mathf.Min(lowerAccuracyLimit, upperAccuracyLimit);
        upperAccuracyLimit = Mathf.Max(lowerAccuracyLimit, upperAccuracyLimit);
    }


    // Update is called once per frame
    void Update()
    {
        if (!automatic)
        {
            if (!shootAction.IsPressed() && (Time.time - lastShotTimestamp) > fireRate) released_trigger = true;
            else if (shootAction.IsPressed() && released_trigger)
            {
                Shoot();
                released_trigger = false;
                lastShotTimestamp = Time.time;
            }
        }
        else if (shootAction.IsPressed() && (Time.time - lastShotTimestamp) > fireRate)
        {
            Shoot();
            lastShotTimestamp = Time.time;
        }
    }

    void Shoot()
    {
        // 1 SHOT
        if (bullets_per_shot == 1)
        {
            GameObject new_bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            new_bullet.GetComponent<Bullet_Physics>().init(Random.Range(lowerAccuracyLimit, upperAccuracyLimit));
        }

        // FIRE MANY SHOTS
        else
        {
            float angle_per_bullet = (upperAccuracyLimit - lowerAccuracyLimit) / bullets_per_shot;
            for (int i = 0; i < bullets_per_shot; i++)
            {
                GameObject new_bullet = Instantiate(bulletPrefab, firePoint.position + new Vector3(0, (-bullets_per_shot / 2 + i) * bullet_spread, 0), firePoint.rotation);
                new_bullet.GetComponent<Bullet_Physics>().init(lowerAccuracyLimit + angle_per_bullet * i * Random.Range(0.7f, 1.3f));
            }
        }
    }

    public float getBarrelLengthOffset() { return barrelLength; }
}