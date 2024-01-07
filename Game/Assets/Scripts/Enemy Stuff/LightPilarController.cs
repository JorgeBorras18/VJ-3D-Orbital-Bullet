using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPilarController : MonoBehaviour
{
    [SerializeField] private GameObject LightPillarPrefab;
    [SerializeField] private int number_of_pillars = 360;
    [SerializeField] private float RingRadius = 9;


    private LightPilar[] pilar_Array;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, transform.position.y, 0);
        pilar_Array = new LightPilar[number_of_pillars];

        for (int i = 0; i < number_of_pillars; i++) {
            float angle = 2 * Mathf.PI / number_of_pillars * i;
            Vector3 pilar_pos = new Vector3(Mathf.Cos(angle) * RingRadius, transform.position.y, Mathf.Sin(angle) * RingRadius);
            GameObject new_pilar = Instantiate(LightPillarPrefab, pilar_pos, Quaternion.identity);
            new_pilar.transform.parent = transform;
            pilar_Array[i] = new_pilar.GetComponent<LightPilar>();

        }
    }

    public void DeployLineAttack()
    {
        for (int i = 0; i < number_of_pillars; i++)
        {
            pilar_Array[i].RaisePillar(0f, 2.5f, true);
        }
    }

    public void DeployByGroups(int Agrupations)
    {
        for (int i = 0; i < Agrupations; ++i)
        {
            for (int j = 0; j < number_of_pillars / Agrupations + 1; j += Agrupations)
            {
                for (int k = 0; k < Agrupations; ++k)
                {
                    if ((i + j) * Agrupations + k < number_of_pillars)
                    {
                        if (Agrupations != 2) pilar_Array[(i + j) * Agrupations + k].RaisePillar(i * 1f, 2.5f, false);
                        else pilar_Array[(i + j) * Agrupations + k].RaisePillar(i * 1.75f, 2.5f, false);
                    }
                }
            }
        }
        /*int actual_group_id = 0;
        for (int i = (number_of_pillars/Agrupations)*Agrupations; i < number_of_pillars; ++i)
        {
            pilar_Array[i].RaisePillar(actual_group_id/Agrupations * 2.5f, 2.5f, true);
            actual_group_id++;
        }*/
    }
}
