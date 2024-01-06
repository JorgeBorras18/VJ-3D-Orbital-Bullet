using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMG_Flash : MonoBehaviour
{
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashTime = 0.25f;
    [SerializeField] private AnimationCurve _flashSpeedCurve;

    private SpriteRenderer[] _spriteRenderers;
    private Material[] _materials;
    private Coroutine _damageFlashCoroutine;

    void Awake()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _materials = new Material[_spriteRenderers.Length];

        //assign sprite renderers materials to _materials
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _materials[i] = _spriteRenderers[i].material;
        }
    }

    public void GenerateDamageFlash()
    {
        _damageFlashCoroutine = StartCoroutine(DamageFlasher());
    }

    private IEnumerator DamageFlasher()
    {
        SetFlashColor();
        float currentFlashAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < _flashTime)
        {
            elapsedTime += Time.deltaTime;
            currentFlashAmount = _flashSpeedCurve.Evaluate(elapsedTime);
            SetFlashAmount(currentFlashAmount);
            yield return null;
        }
    }

    private void SetFlashColor()
    {
        //set the color
        for (int i = 0; i < _materials.Length; i++) 
            _materials[i].SetColor("_FlashColor", _flashColor);
    }

    private void SetFlashAmount(float amount)
    {
        //set the color
        for (int i = 0; i < _materials.Length; i++)
            _materials[i].SetFloat("_FlashAmount", amount);
    }
}
