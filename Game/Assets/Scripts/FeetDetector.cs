using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetDetector : MonoBehaviour
{
    [SerializeField] int stomp_damage = 25;
    private bool isEnemyBellow = false;
    // Start is called before the first frame update


    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == ("Enemy") && hit.transform.position.y < transform.position.y)
        {
            bool didStompEnemy = FindAnyObjectByType<PlayerLogic>().TriggerUppwardRollAfterEnemyStomp();
            if (didStompEnemy) hit.gameObject.GetComponent<Enemy>().TakeDamage(stomp_damage);
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
