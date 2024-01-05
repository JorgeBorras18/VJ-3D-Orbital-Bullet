using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{

    [SerializeField] private Slider slider;
    [SerializeField] public Transform Camera;

    //newHealthValue must be a value between 0 and 100
    public void UpdateHealthBar(float newHealthValue)
    {
        slider.value = newHealthValue;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + Camera.forward);
    }
}
