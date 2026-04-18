using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameState : MonoBehaviour
{
    [SerializeField] private int costOfSatellite = 1000;

    public TimeScaler TimeScaler => _timeScaler;
    private TimeScaler _timeScaler;
    public Economy economy;
    private Camera _mainCamera;

    [Header("World Hookup")] public GameObject prefabSatellite;
    public GameObject prefabOrbit;
    public Orbit templateOrbit;

    private void Awake()
    {
        _timeScaler = FindFirstObjectByType<TimeScaler>();
        _mainCamera = FindFirstObjectByType<Camera>();
        templateOrbit.gameObject.SetActive(false);
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

    public bool AddSatellite()
    {
        var newBalance = economy.Money - costOfSatellite;
        if (newBalance >= 0)
        {
            economy.Money = newBalance;

            GameObject orbitInstance = Instantiate(prefabOrbit, Vector3.zero, Quaternion.identity);
            GameObject satInstance = Instantiate(prefabSatellite, transform);

            Orbit orbit = orbitInstance.GetComponent<Orbit>();
            orbit.equator = Random.Range(0, 359);
            orbit.inclination = Random.Range(-80, 80);

            SatelliteInstance sat = satInstance.GetComponent<SatelliteInstance>();
            sat.orbit = orbit;

            return true;
        }

        return false;
    }
}