using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundAnimation : MonoBehaviour
{
    private RawImage backgroundImage;
    private Vector2 speed;

    private void Start()
    {
        backgroundImage = GetComponent<RawImage>();
        speed = new Vector2 (0.02f, 0.02f);
        Time.timeScale = 1f;
    }

    void Update()
    {
        backgroundImage.uvRect = new Rect(backgroundImage.uvRect.position + speed * Time.deltaTime, backgroundImage.uvRect.size);
    }
}
