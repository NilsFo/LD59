using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameState : MonoBehaviour
{
    [SerializeField] private int costOfSatellite = 1000;
    public enum SelectionState
    {
        Normal,
        SatelliteReroute
    }

    public SelectionState selectionState;

    public TimeScaler TimeScaler => _timeScaler;
    private TimeScaler _timeScaler;
    public Economy economy;
    
    [SerializeField] [CanBeNull] private SatelliteInstance selectedSatellite;
    public event Action<SatelliteInstance> OnSelectedSatelliteChanged;
    
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
        // money cheat
        Keyboard k = Keyboard.current;
        if (k.mKey.wasPressedThisFrame)
        {
            economy.Money += 100;
        }

        if(selectionState == SelectionState.SatelliteReroute)
            SetNewOrbit();
    }

    public Camera GetCamera()
    {
        return _mainCamera;
    }

    public bool AddSatellite()
    {
        print("Gamestate: Sat requested.");
        var newBalance = economy.Money - costOfSatellite;
        if (newBalance >= 0)
        {
            print("Buying a sat.");

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
        else
        {
            print("I can't give credit. Come back when you are... richer!");
        }

        return false;
    }

    public void SetSelectedSatellite(SatelliteInstance sat = null)
    {
        if (sat == null)
        {
            if (selectedSatellite !=null)
            {
                selectedSatellite.IsSelected = false;
                selectedSatellite = null;
            }
            OnSelectedSatelliteChanged?.Invoke(null);
        }
        else
        {
            if (selectedSatellite != null
                && selectedSatellite.gameObject.GetInstanceID() != sat.gameObject.GetInstanceID())
            {
                selectedSatellite.IsSelected = false;
                selectedSatellite = null;
            }
            selectedSatellite = sat;
            OnSelectedSatelliteChanged?.Invoke(sat);
        }
    }

    public void SetNewOrbit()
    {
        if (selectedSatellite != null)
        {
            RaycastHit hit;
            var mousePos = Mouse.current.position.value;
            Ray ray = GetCamera().ScreenPointToRay(mousePos);
            
            if (Physics.Raycast(ray, out hit)) {
                Transform objectHit = hit.transform;
                
                float newOmega = templateOrbit.SetNewOrbit(transform.position, hit.point);
                templateOrbit.gameObject.SetActive(true);
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    var newOrbit = Instantiate(templateOrbit, Vector3.zero, Quaternion.identity);
                    newOrbit.SetFromOrbit(templateOrbit);
                    selectedSatellite.SwitchOrbit(newOrbit, newOmega);
                }
            }
            else
            {
                templateOrbit.gameObject.SetActive(false);
            }
        }else
        {
            templateOrbit.gameObject.SetActive(false);
        }
    }
    
}
