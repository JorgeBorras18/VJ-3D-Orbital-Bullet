using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private int max_health = 125;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI health_number;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill;

    private float actual_health = 125;

    //newHealthValue must be a value between 0 and 100
    public void UpdateHealthBar(float newHealthValue)
    {
        float new_percentage = newHealthValue / max_health;
        slider.value = new_percentage;
        fill.color = gradient.Evaluate(new_percentage);

        health_number.text = ((int)Mathf.Round(newHealthValue)).ToString();
    }

    // Update is called once per frame
    public void TakeDamage(float dmg_value)
    {
        actual_health = Mathf.Max(0, actual_health - dmg_value);
        float percentage_health = actual_health / max_health;
        slider.value = percentage_health;
        fill.color = gradient.Evaluate(percentage_health);

        health_number.text = ((int)Mathf.Round(actual_health)).ToString();
    }

    private void Start()
    {
        UpdateHealthBar(125);
    }
    private void FixedUpdate()
    {
        if (Input.GetKey("space")) TakeDamage(1);
    }
}
