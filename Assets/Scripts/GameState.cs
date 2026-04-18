using System;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public TimeScaler TimeScaler => _timeScaler;
    private TimeScaler _timeScaler;
    public Economy economy;
    private Camera _mainCamera;

    private void Awake()
    {
        _timeScaler = FindFirstObjectByType<TimeScaler>();
        _mainCamera = FindFirstObjectByType<Camera>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public Camera GetCamera()
    {
        return _mainCamera;
    }
}
