using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetDetector : MonoBehaviour
{
    private bool isEnemyBellow = false;
    // Start is called before the first frame update


    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == ("Enemy"))
        {
            isEnemyBellow = true;
        }
        else if (hit.gameObject.tag == ("Terrain"))
        {
            isEnemyBellow = false;
        }
    }

    private void OnTriggerExit(Collider hit)
    {
        if (hit.gameObject.tag == ("Enemy"))
            isEnemyBellow = false;
    }

    public bool isThereEnemyBellow() { return isEnemyBellow; }
    public void resetDetector() { isEnemyBellow = false; }
}
