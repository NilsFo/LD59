using System;
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

    public float omega;
    public Orbit orbit;

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
        omega += Time.deltaTime * 10;
        transform.localPosition = orbit.GetOrbitPosition(omega);
    }

    private void OnDestroy()
    {
    }
}