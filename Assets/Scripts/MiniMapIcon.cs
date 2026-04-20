using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MiniMapIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public HoverDescription sourceDescription;
    private GameState _gameState;

    private void Awake()
    {
        _gameState = FindFirstObjectByType<GameState>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sourceDescription.DisplayText();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _gameState.ClearDisplayDescription();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }
}