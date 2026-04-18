using System;
using UI;
using UnityEngine;

public class SatelliteInstance : MonoBehaviour
{
    ////////////////////////////////////////
    // Params
    ////////////////////////////////////////
    [Header("Params")] public string displayName;
    public int fuelMax, fuelCurrent;
    public int height;

    [Header("Properties")] public Vector2 position;

    private GameState _gameState;
    private SatellitDisplayScript _displayScript;
    
    public event Action OnSatelliteDestroy;

    private void Awake()
    {
        _gameState = FindFirstObjectByType<GameState>();
        _displayScript = FindFirstObjectByType<SatellitDisplayScript>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _displayScript.AddSatellit(this);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
        OnSatelliteDestroy?.Invoke();
    }
}