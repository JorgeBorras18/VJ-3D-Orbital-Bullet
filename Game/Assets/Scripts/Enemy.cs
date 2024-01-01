using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float max_health = 100;
    private float health;

    public GameObject deathEffect;
    [SerializeField] FloatingHealthBar healthBar;

    private void Awake()
    {
        if (healthBar == null) healthBar = GetComponentInChildren<FloatingHealthBar>(); 
    }

    private void Start()
    {
        health = max_health;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.UpdateHealthBar(health / max_health);
        if (health <= 0) Die();
    }

    void Die()
    {
        //Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
