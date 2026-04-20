using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimeScaleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image myImage;
    public Sprite onSprite;
    public Sprite offSprite;

    private GameState _gameState;

    public TimeScaler.TimeScaleState targetState;

    private void Awake()
    {
        _gameState = FindAnyObjectByType<GameState>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        myImage.sprite = offSprite;
        if (_gameState.TimeScaler.currentTimeScaleState == targetState)
        {
            myImage.sprite = onSprite;
        }
    }

    public void OnClick()
    {
        _gameState.TimeScaler.currentTimeScaleState = targetState;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }
}