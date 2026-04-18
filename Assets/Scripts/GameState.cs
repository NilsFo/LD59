using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameState : MonoBehaviour
{

    [SerializeField]
    private int costOfSatellite = 1000;
    
    public TimeScaler TimeScaler => _timeScaler;
    private TimeScaler _timeScaler;
    public Economy economy;

    public GameObject prefabSatellite;

    private void Awake()
    {
        _timeScaler = FindFirstObjectByType<TimeScaler>();
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
        return Camera.current;
    }

    public bool AddSatellite()
    {
        var newBalance = economy.Money - costOfSatellite;
        if (newBalance >= 0)
        {
            economy.Money = newBalance;
            Instantiate(prefabSatellite, transform);
            return true;
        }

        return false;
    }
}
