using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SatelliteSelector : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public SatelliteInstance sat;
    
    private GameState _gameState;

    private void Start()
    {
        _gameState = FindFirstObjectByType<GameState>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(sat != null) _gameState.SetSelectedSatellite(sat, true);
    }
}
