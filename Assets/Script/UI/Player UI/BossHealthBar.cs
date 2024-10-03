using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Slider healthslider;
    public Slider Easehealthslider;
    private float lerpSpeed = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        if (FinalBoss.Instance != null)
        {
            healthslider.maxValue = FinalBoss.Instance.maxHealth;
            Easehealthslider.maxValue = FinalBoss.Instance.maxHealth;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (FinalBoss.Instance != null)
        {
            float health = FinalBoss.Instance.Health;

            if (healthslider.value != health)
            {
                healthslider.value = health;
            }

            if (Easehealthslider.value != health)
            {
                Easehealthslider.value = Mathf.Lerp(Easehealthslider.value, health, lerpSpeed);
            }
        }
    }
}
