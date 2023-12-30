using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeTitle : MonoBehaviour
{
    [SerializeField] private Image font;
    [SerializeField] private TextMeshProUGUI text;
    private float timer = 1;
    [SerializeField] private float readTimer = 1;
    [SerializeField] [Range(1,5)] private float fadeSpeed = 1;
    private bool canPlaySound = true;

    void Update()
    {
        readTimer -= Time.deltaTime;
        if(readTimer <= 0)
        {
            FadeOut();
            if (canPlaySound)
            {
                FindObjectOfType<AudioManager>().Play("Fade");
                canPlaySound = false;
            }
        }
    }

    private void FadeOut()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime * fadeSpeed;
            font.color = new Color(font.color.r, font.color.g, font.color.b, timer);
            text.color = new Color(text.color.r, text.color.g, text.color.b, timer);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
