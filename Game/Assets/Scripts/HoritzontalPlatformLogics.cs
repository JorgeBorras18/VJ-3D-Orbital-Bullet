using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoritzontalPlatformLogics : MonoBehaviour
{
    private float amplitude = 0.5f;
    private float frequency = 1f;
    private float phaseShift = 0f;
    private float verticalShift = 0f;
    private float height;
    private float movementSpeed = 1f;

    private GameObject movingRing;
    // Start is called before the first frame update
    void Start()
    {
        movingRing = GameObject.Find("movingRing").gameObject;
        height = movingRing.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the position based on the sinusoidal function
        float yPos = height + amplitude * Mathf.Sin(2 * Mathf.PI * frequency * Time.time + phaseShift) + verticalShift;
        movingRing.transform.position = new Vector3(movingRing.transform.position.x, yPos, movingRing.transform.position.z);

    }
}
