using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicadorLogic : MonoBehaviour
{
    public float amplitude = 1f;
    public float frequency = 1f;
    public float phaseShift = 0f;
    public float verticalShift = 0f;
    private float height;
    public float movementSpeed = 1f;
    private Vector3 originalScale;

    // Start is called before the first frame update
    void Start()
    {
        height = transform.position.y;
        originalScale = transform.localScale;
        transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
    }

    void Update()
    {
        // Update the position based on the sinusoidal function
        float yPos = height + amplitude * Mathf.Sin(2 * Mathf.PI * frequency * Time.time + phaseShift) + verticalShift;
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }

    public void turnIndicadorOn()
    {

        transform.localScale = originalScale;
    }
}

