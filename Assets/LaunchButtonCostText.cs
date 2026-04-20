using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LaunchButtonCostText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameState _gameState;
    public TMP_Text costTF;
    public TMP_Text inOrbitTF;
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
        int satCount = _gameState.listOfSatellites.Count;
        inOrbitTF.text = "In Orbit: " + satCount + "/" + _gameState.maxSatellites;
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