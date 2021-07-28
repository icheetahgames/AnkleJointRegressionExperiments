using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public Gradient grad;

    public Image fill;
    // Start is called before the first frame update

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        fill.color = grad.Evaluate(1);
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        fill.color = grad.Evaluate(slider.normalizedValue);
    }

    private void Start()
    {
        SetHealth(0);
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetHealth(slider.value + 10);
        }
    }
}
