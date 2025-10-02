using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  

public class UIController : MonoBehaviour
{
    public Image cursorImage;
    public Sprite cursorSprite;
    public Sprite activeCursorSprite;

    private bool isActive = false;

    void Start()
    {
        // InvokeRepeating(nameof(ToggleCursor), 0f, 3f);
    }

    public void ToggleCursor()
    {
        isActive = !isActive;
        cursorImage.sprite = isActive ? activeCursorSprite : cursorSprite;
    }
}