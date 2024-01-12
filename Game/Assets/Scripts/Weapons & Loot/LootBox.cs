using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    //Prefabs
    [SerializeField] private GameObject AmmoBoxPrefab;
    [SerializeField] private GameObject AR;
    [SerializeField] private GameObject GLauncher;
    [SerializeField] private GameObject Shotgun;
    [SerializeField] private GameObject SMG;

    [SerializeField] private float chancesGettingGun = 0.33f;
    private Gun_Controller _Gun_Controller;
    private GameObject[] random_gun;
    private Transform[] loot_placement;

    private void Awake()
    {
        _Gun_Controller = FindAnyObjectByType<Gun_Controller>();
        loot_placement = new Transform[3];
        for (int i = 0; i < loot_placement.Length; ++i) loot_placement[i] = transform.GetChild(i);

        random_gun = new GameObject[4];
        random_gun[0] = AR;
        random_gun[1] = GLauncher;
        random_gun[2] = Shotgun;
        random_gun[3] = SMG;
    }

    public void OpenLootBox() 
    {
        for (int i = 0; i < 3; i++)
        {
            float result = Random.Range(0f, 1f);
            if (result > chancesGettingGun)
            {
                //Drop Ammo
                Instantiate(AmmoBoxPrefab, loot_placement[i].position, Quaternion.identity);
            }
            else
            {
                // Drop Weapon that Player Doesn't Own
                int result2 = Random.Range(0, random_gun.Length);
                while (_Gun_Controller.AlreadyOwnsThatWeapon(random_gun[result2].name.Substring(0, 2))) result2 = (result2 + 1) % random_gun.Length;
                Instantiate(random_gun[result2], loot_placement[i].position, Quaternion.identity);
            }
        }
        Destroy(transform.parent.gameObject);
    
    }

}
