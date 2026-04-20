using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LaunchButtonCostText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameState _gameState;
    public TMP_Text costTF;
    public TMP_Text inOrbitTF;
    public UnityEvent onClick;

    public Image launchButtonImage;
    public Sprite canAffordSprite;
    public Sprite canNotAffordSprite;

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

        costTF.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        int satCount = _gameState.listOfSatellites.Count;
        inOrbitTF.text = "In Orbit: " + satCount + "/" + _gameState.maxSatellites;

        if (_gameState.economy.Money < _gameState.CostOfNextSatellite() || satCount >= _gameState.maxSatellites)
        {
            launchButtonImage.sprite = canNotAffordSprite;
        }
        else
        {
            launchButtonImage.sprite = canAffordSprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        costTF.text = _gameState.CostOfNextSatellite() + "€";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        costTF.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke();
        costTF.text = _gameState.CostOfNextSatellite() + "€";
    }
}