using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// This class controls the Player UI part responsible for showing the Player's Guns
// Ammo left in each of them, Type of Ammo they use and which weapon is the one being used at the moment
public class WeaponsControllerUI : MonoBehaviour
{

    //Gun Sprites
    [SerializeField] private Sprite PistolSprite;
    [SerializeField] private Sprite ARSprite;
    [SerializeField] private Sprite GLauncherSprite;
    [SerializeField] private Sprite ShotgunSprite;
    [SerializeField] private Sprite SMGSprite;

    //Ammo Types Sprites
    [SerializeField] private Sprite AmmoBulletSprite;
    [SerializeField] private Sprite AmmoEnergySprite;
    [SerializeField] private Sprite AmmoExplosiveSprite;
    [SerializeField] private Sprite AmmoShellSprite;

    private int active_gun_id = 0;
    private int ammo_primari_weapon = -1;
    private int ammo_secondary_weapon = -1;


    public void SubstituteWeaponByNew(string weapon_name, int ammo_reserves)
    {
        // Select Placeholder 1 or 2
        Transform WeaponPlacefolder;
        if (active_gun_id == 0)
        {
            WeaponPlacefolder = transform.GetChild(0);
            ammo_primari_weapon = ammo_reserves;
        }
        else
        {
            WeaponPlacefolder = transform.GetChild(1);
            ammo_secondary_weapon = ammo_reserves;
        }
        WeaponPlacefolder.GetComponent<Image>().color = Color.white;

        // Depending on Weapon Type, choose correct Sprite and AMMO
        // Defenetly needs work but right now it's enough
        if (weapon_name.Contains("AR"))
        {
            WeaponPlacefolder.GetChild(0).gameObject.GetComponent<Image>().sprite = ARSprite;
            WeaponPlacefolder.GetChild(1).gameObject.GetComponent<Image>().sprite = AmmoBulletSprite;
        }
        else if (weapon_name.Contains("GLauncher"))
        {
            WeaponPlacefolder.GetChild(0).gameObject.GetComponent<Image>().sprite = GLauncherSprite;
            WeaponPlacefolder.GetChild(1).gameObject.GetComponent<Image>().sprite = AmmoExplosiveSprite;
        }
        else if (weapon_name.Contains("Pistol"))
        {
            WeaponPlacefolder.GetChild(0).gameObject.GetComponent<Image>().sprite = PistolSprite;
            WeaponPlacefolder.GetChild(1).gameObject.GetComponent<Image>().sprite = AmmoBulletSprite;
        }
        else if (weapon_name.Contains("Shotgun"))
        {
            WeaponPlacefolder.GetChild(0).gameObject.GetComponent<Image>().sprite = ShotgunSprite;
            WeaponPlacefolder.GetChild(1).gameObject.GetComponent<Image>().sprite = AmmoShellSprite;
        }
        else if (weapon_name.Contains("SMG"))
        {
            WeaponPlacefolder.GetChild(0).gameObject.GetComponent<Image>().sprite = SMGSprite;
            WeaponPlacefolder.GetChild(1).gameObject.GetComponent<Image>().sprite = AmmoBulletSprite;
        }

        // Get Ammo
        if (ammo_reserves == 0) WeaponPlacefolder.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().color = Color.red;
        else WeaponPlacefolder.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        WeaponPlacefolder.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = ammo_reserves.ToString();
    }


    public void DecreaseAmmoByOne()
    {
        // Select Placeholder 1 or 2
        if (active_gun_id == 0)
        {
            ammo_primari_weapon -= 1;
            if (ammo_primari_weapon == 0) transform.GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().color = Color.red;
            transform.GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = ammo_primari_weapon.ToString();
            return;
        }
        ammo_secondary_weapon -= 1;
        if (ammo_secondary_weapon == 0) transform.GetChild(1).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().color = Color.red;
        transform.GetChild(1).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = ammo_secondary_weapon.ToString();
    }


    public void SwapToOffhandWeapon()
    {
        // Figure Out which gun Is active and which isnt'
        int inactive_gun_id = 0;
        if (active_gun_id == 0) inactive_gun_id = 1;

        //Deselect Old Gun
        GameObject OldWeapon = transform.GetChild(active_gun_id).gameObject;
        OldWeapon.GetComponent<Image>().color = Color.gray;
        OldWeapon.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.gray;
        OldWeapon.transform.GetChild(1).gameObject.GetComponent<Image>().color = Color.gray;
        TextMeshProUGUI aux = OldWeapon.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
        if (aux.color == Color.red) aux.color = Color.gray * Color.red;
        else aux.color = Color.gray;

        // Select New Gun
        GameObject NewWeapon = transform.GetChild(inactive_gun_id).gameObject;
        NewWeapon.GetComponent<Image>().color = Color.white;
        NewWeapon.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
        NewWeapon.transform.GetChild(1).gameObject.GetComponent<Image>().color = Color.white;
        aux = NewWeapon.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
        if (aux.color == Color.gray * Color.red) aux.color = Color.red;
        else aux.color = Color.white;
        active_gun_id = inactive_gun_id;
    }
}
