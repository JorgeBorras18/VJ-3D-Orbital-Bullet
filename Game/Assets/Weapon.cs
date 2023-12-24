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
    public float barrelLength = 0f;

    private float lastShotTimestamp;
    private float shotsInChamber;


    void Start()
    {
        _playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
        shootAction = _playerInput.actions["Fire"];

        lastShotTimestamp = Time.time;
        shotsInChamber = magazineSize;
    }


    // Update is called once per frame
    void Update()
    {
        if (shootAction.IsPressed() && (Time.time - lastShotTimestamp) > fireRate)
        {
            Shoot();
            lastShotTimestamp = Time.time;
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    public float getBarrelLengthOffset() { return barrelLength; }
}
