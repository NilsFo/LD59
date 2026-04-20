using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LaunchButtonCostText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameState _gameState;
    public TMP_Text costTF;
    public UnityEvent onClick;

    private void Awake()
    {
        _gameState = FindFirstObjectByType<GameState>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (onClick == null)
        {
            onClick = new UnityEvent();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        costTF.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke();
    }
}