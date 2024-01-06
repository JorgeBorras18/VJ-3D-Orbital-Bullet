using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using static AmmoBox;


// This Class holds the key to grab and swap Weapons
// At all times player will hold either hold 1 weapon or 2 and can grab another one
// if 1 held, the picked up weapon will become the 2 one, if 2 are held then the main Weapon will be the one swaped.
public class Gun_Controller : MonoBehaviour
{

    [SerializeField] private GameObject Main_Weapon;
    [SerializeField] private GameObject Offhand_Weapon;
    [SerializeField] private WeaponsControllerUI WeaponInventoryPlaceholder;
    [SerializeField] private GameObject PickUpWeaponBillboard;

    private PlayerInput _playerInput;
    private InputAction swapWeaponAction;
    private InputAction pickUpWeaponAction;
    private bool button_released = true;
    private bool pickup_button_released = true;

    void Start()
    {
        if (WeaponInventoryPlaceholder == null) WeaponInventoryPlaceholder = FindObjectOfType<WeaponsControllerUI>();

        _playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
        swapWeaponAction = _playerInput.actions["Swap Weapon"];
        pickUpWeaponAction = _playerInput.actions["Use"];

        Main_Weapon.SetActive(true);
        WeaponInventoryPlaceholder.SubstituteWeaponByNew(Main_Weapon.name, Main_Weapon.GetComponent<Weapon>().getBulletsLeftInMagazine());
        if (Offhand_Weapon != null)
        {
            WeaponInventoryPlaceholder.SwapToOffhandWeapon();
            WeaponInventoryPlaceholder.SubstituteWeaponByNew(Offhand_Weapon.name, Offhand_Weapon.GetComponent<Weapon>().getBulletsLeftInMagazine());
            WeaponInventoryPlaceholder.SwapToOffhandWeapon();
            Offhand_Weapon.SetActive(false);
        }

        PickUpWeaponBillboard.SetActive(false);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!swapWeaponAction.IsPressed()) button_released = true;
        else if (Offhand_Weapon != null && swapWeaponAction.IsPressed() && button_released) {
            button_released = false;
            SwapWeapon();
        }

        if (!pickUpWeaponAction.IsPressed()) pickup_button_released = true;
    }

    private void SwapWeapon()
    {
        // Logical Swap
        GameObject temp = Main_Weapon;
        Main_Weapon = Offhand_Weapon;
        Offhand_Weapon = temp;

        // Physically Swap
        Main_Weapon.SetActive(true);
        Offhand_Weapon.SetActive(false);

        // Modify Player UI
        FindObjectOfType<WeaponsControllerUI>().SwapToOffhandWeapon();
    }

    private void PickUpWeapon(GameObject NewGun, int BulletsInNewGun)
    {
        if (Offhand_Weapon == null)
        {
            Offhand_Weapon = Main_Weapon;
            Offhand_Weapon.SetActive(false);
            FindObjectOfType<WeaponsControllerUI>().SwapToOffhandWeapon();

        }
        else
        {
            //Drop Old Weapon to floor (convert to dropable and destroy from player hands)
            Weapon old_weapon = Main_Weapon.GetComponent<Weapon>();
            GameObject DroppableWeaponPrefab = old_weapon.getDroppableVersion();
            GameObject DroppedWeapon = Instantiate(DroppableWeaponPrefab, transform.position + new Vector3(0, -0.25f, 0), transform.rotation);
            DroppedWeapon.transform.GetChild(0).GetComponent<DroppableWeapon>().setBulletsInMagazine(old_weapon.getBulletsLeftInMagazine());
            Destroy(Main_Weapon);
        }

        // Set New Weapon as Main Weapon
        Main_Weapon = Instantiate(NewGun, transform.position, transform.rotation);
        Main_Weapon.transform.parent = transform;
        Main_Weapon.transform.localScale = NewGun.transform.localScale;
        Main_Weapon.transform.localPosition = NewGun.transform.localPosition;
        Weapon weapon_script = Main_Weapon.GetComponent<Weapon>();
        weapon_script.setBulletsLeftInMagazien(BulletsInNewGun);

        //Modify Player UI
        WeaponInventoryPlaceholder.SubstituteWeaponByNew(Main_Weapon.name, BulletsInNewGun);
    }

    // Called by Ammo Box (initialize based on actual ammo Type)
    // Returns the type of Bullets that at least One of the Weapons Consume
    public AMMO_TYPE getRandomAvailableAmmoType()
    {
        if (Offhand_Weapon == null) return Main_Weapon.GetComponent<Weapon>().GetAmmoType();
        else if (Random.Range(0, 2) < 1) return Main_Weapon.GetComponent<Weapon>().GetAmmoType();
        return Offhand_Weapon.GetComponent<Weapon>().GetAmmoType();
    }

    // Add ammo to Weapons that ammo_type = Weapon.ammo_type
    // if both share same ammo type then divide by half the amount
    public void addAmmoToCurrentWeapons(AMMO_TYPE ammo_type)
    {
        int ammo_added = 0;
        if (Offhand_Weapon != null && ammo_type == Offhand_Weapon.GetComponent<Weapon>().GetAmmoType())
        {
            // Share ammo between both weapons ?
            bool divide_ammo_in_half = (ammo_type == Main_Weapon.GetComponent<Weapon>().GetAmmoType());
            if (divide_ammo_in_half)
            {
                ammo_added = Main_Weapon.GetComponent<Weapon>().increaseBulletReserves(true);
                WeaponInventoryPlaceholder.IncreaseAmmoMainWeapon(ammo_added);
            }

            Offhand_Weapon.SetActive(true);
            ammo_added = Offhand_Weapon.GetComponent<Weapon>().increaseBulletReserves(divide_ammo_in_half);
            Offhand_Weapon.SetActive(false);
            WeaponInventoryPlaceholder.IncreaseAmmoOffHandWeapon(ammo_added);
        }
        else if (ammo_type == Main_Weapon.GetComponent<Weapon>().GetAmmoType())
        {
            ammo_added = Main_Weapon.GetComponent<Weapon>().increaseBulletReserves(false);
            WeaponInventoryPlaceholder.IncreaseAmmoMainWeapon(ammo_added);
        }
    }


    private void OnTriggerEnter(Collider hit)
    {
        if (hit.tag == "DropeableWeapon")
        {
            PickUpWeaponBillboard.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider hit)
    {
        if (hit.tag == "DropeableWeapon")
        {
            PickUpWeaponBillboard.SetActive(true);
            if (pickUpWeaponAction.IsPressed() && pickup_button_released)
            {
                pickup_button_released = false;
                DroppableWeapon drop = hit.gameObject.GetComponent<DroppableWeapon>();
                PickUpWeapon(drop.PickUpWeapon(), drop.getBulletsInMagazine());
                PickUpWeaponBillboard.SetActive(false);

                // Delete Picked Up Weapon from Ground
                Destroy(hit.transform.parent.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider hit)
    {
        if (hit.tag == "DropeableWeapon")
        {
            PickUpWeaponBillboard.SetActive(false);
        }
    }
}
