using System;
using UnityEngine;
using UnityEngine.UI;

public class UiSkinImageButton : MonoBehaviour
{
    public Image image;

    public Sprite spriteNormal;
    public Sprite spriteHovered;
    public Sprite spriteClicked;

    public bool isHovered;
    public bool isClicked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        image.sprite = spriteNormal;

        if (isClicked)
        {
            image.sprite = spriteClicked;
        }
        else
        {
            if (isHovered)
            {
                image.sprite = spriteHovered;
            }
            else
            {
                image.sprite = spriteNormal;
            }
        }
    }

    private void OnMouseEnter()
    {
        isHovered = true;
    }

    private void OnMouseExit()
    {
        isClicked = false;
        isHovered = false;
    }

    private void OnMouseDown()
    {
        isClicked = true;
    }

    private void OnMouseUp()
    {
        isClicked = false;
    }
}