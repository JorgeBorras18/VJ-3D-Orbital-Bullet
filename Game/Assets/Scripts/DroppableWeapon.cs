using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppableWeapon : MonoBehaviour
{
    [SerializeField] private GameObject OnHandWeaponPrefab;
    [SerializeField] private int BulletsInMagazine;


    private void Update()
    {
        Debug.Log(transform.position);
    }

    public int getBulletsInMagazine () { return BulletsInMagazine; }

    public void setBulletsInMagazine(int bullet_quantity) { BulletsInMagazine = bullet_quantity; }

    public GameObject PickUpWeapon(){ return OnHandWeaponPrefab; }
}
