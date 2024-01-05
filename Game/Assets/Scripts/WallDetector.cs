using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetector : MonoBehaviour
{
    private bool isThereWallAhead = false;
    private bool facingRight = false;
    // Start is called before the first frame update


    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == ("Terrain"))
        {
            Debug.Log("Terrain");
            isThereWallAhead = true;
        }
    }

    private void OnTriggerExit(Collider hit)
    {
        if (hit.gameObject.tag == ("Terrain"))
            isThereWallAhead = false;
    }

    public void UpdateOrientation(bool isNowFacingRight) { facingRight = isNowFacingRight; }

    private void Update()
    {
        transform.LookAt(new Vector3(0, transform.position.y, 0));
        if (facingRight) transform.Rotate(0, 180, 0);
        else transform.Rotate(0, 0, 0);
    }

    public bool isWallAhead() { return isThereWallAhead; }
}
