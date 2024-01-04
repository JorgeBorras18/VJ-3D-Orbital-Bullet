using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.FilePathAttribute;
using UnityEngine.UIElements;
using Unity.VisualScripting;


// This Class holds the key to grab and swap Weapons
// At all times player will hold either hold 1 weapon or 2 and can grab another one
// if 1 held, the picked up weapon will become the 2 one, if 2 are held then the main Weapon will be the one swaped.
public class Gun_Controller : MonoBehaviour
{

    [SerializeField] private GameObject Main_Weapon;
    [SerializeField] private GameObject Offhand_Weapon;
    [SerializeField] private WeaponsControllerUI WeaponInventoryPlaceholder;

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

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.tag == "DropeableWeapon")
        {
            //Show "Pick Up Weapon Prompt"
        }
    }

    private void OnTriggerStay(Collider hit)
    {
        if (hit.tag == "DropeableWeapon" && pickUpWeaponAction.IsPressed() && pickup_button_released)
        {
            pickup_button_released = false;
            DroppableWeapon drop = hit.gameObject.GetComponent<DroppableWeapon>();
            PickUpWeapon(drop.PickUpWeapon(), drop.getBulletsInMagazine());

            // Delete Picked Up Weapon from Ground
            Destroy(hit.transform.parent.gameObject);
        }
    }
    private void OnTriggerExit(Collider hit)
    {
        if (hit.tag == "DropeableWeapon")
        {
            //Hide "Pick Up Weapon Prompt"
        }
    }
}
