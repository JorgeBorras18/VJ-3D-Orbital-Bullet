using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float max_health = 100;
    private float health;
    private float last_damage_timestamp = 0;


    public GameObject deathEffect;
    [SerializeField] FloatingHealthBar healthBar;
    [SerializeField] GameObject AmmoBoxReward;
    private DMG_Flash DamageFlashComponent;

    private void Awake()
    {
        if (healthBar == null) healthBar = GetComponentInChildren<FloatingHealthBar>();
        DamageFlashComponent = GetComponent<DMG_Flash>();
    }

    private void Start()
    {
        health = max_health;
    }

    public void TakeDamage(int damage)
    {
        if (Time.time - last_damage_timestamp > 0.1) // Avoid 2-Times Dmg Bug
        {
            health -= damage;
            healthBar.UpdateHealthBar(health / max_health);
            if (health <= 0) Die();
            else DamageFlashComponent.GenerateDamageFlash();
            last_damage_timestamp = Time.time;
        }
    }

    void Die()
    {
        //Instantiate(deathEffect, transform.position, Quaternion.identity);
        float result = Random.Range(0f, 1f);
        Debug.Log(result);
        if (result > 0.5f) Instantiate(AmmoBoxReward, transform.position, Quaternion.identity); //if lucky drop Ammo Box
        Destroy(gameObject);
    }
}
