using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    Slider slider;

    void Awake() {
        slider = GetComponentInChildren<Slider>();
    }

    public void SetHealth(int health) {
        if (health <= 0)
            Destroy(this.gameObject);
        slider.value = health;
    }

    public void SetMaxHealth(int health) {
        slider.maxValue = health;
        slider.value = health;
    }
}
