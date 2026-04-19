using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = System.Object;
using Random = UnityEngine.Random;

public class GameState : MonoBehaviour
{
    [SerializeField] private int costOfSatellite = 1000;

    public enum SelectionState
    {
        None,
        Init,
        SatelliteReroute
    }

    public SelectionState selectionState = SelectionState.None;
    [SerializeField] private float _currentDelay = 0f;
    [SerializeField] private float _maxDelay = 5f;

    public TimeScaler TimeScaler => _timeScaler;
    private TimeScaler _timeScaler;
    public Economy economy;

    public Objective[] objectives;

    [SerializeField] [CanBeNull] private SatelliteInstance selectedSatellite;
    public event Action<SatelliteInstance> OnSelectedSatelliteChanged;

    private Camera _mainCamera;

    [Header("World Hookup")] public GameObject prefabSatellite;
    public GameObject prefabOrbit;
    public Orbit templateOrbit;
    private MusicManager _musicManager;

    private void Awake()
    {
        _timeScaler = FindFirstObjectByType<TimeScaler>();
        _mainCamera = FindFirstObjectByType<Camera>();
        _musicManager = FindAnyObjectByType<MusicManager>();
        templateOrbit.gameObject.SetActive(false);

        objectives = FindObjectsByType<Objective>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;
        _musicManager.Play(0);
        MusicManager.userDesiredMasterVolume = 0.5f;
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

        if (selectionState == SelectionState.SatelliteReroute)
        {
            UpdateOrbitPreview();
        }
        else if (selectionState == SelectionState.Init)
        {
            _currentDelay -= Time.unscaledDeltaTime;
            if (_currentDelay <= 0f)
            {
                selectionState = SelectionState.SatelliteReroute;
            }
        }
    }

    private void OnNewSelectionState()
    {
        print("Entered a new selection state: " + selectionState);
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
            orbit.SetFromIncEq(Random.Range(-80, 80), Random.Range(0, 359));

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
            if (selectedSatellite != null)
            {
                selectedSatellite.IsSelected = false;
                selectedSatellite = null;
            }

            selectionState = SelectionState.None;
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

            _currentDelay = _maxDelay;
            selectionState = SelectionState.Init;
            selectedSatellite = sat;
            OnSelectedSatelliteChanged?.Invoke(sat);
        }
    }

    public void UpdateOrbitPreview()
    {
        if (selectionState == SelectionState.SatelliteReroute && selectedSatellite != null)
        {
            RaycastHit hit;
            var mousePos = Mouse.current.position.value;
            Ray ray = GetCamera().ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                float newOmega = templateOrbit.SetNewOrbit(selectedSatellite.transform.position, hit.point);
                templateOrbit.gameObject.SetActive(true);
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    var newOrbit = Instantiate(templateOrbit, Vector3.zero, Quaternion.identity);
                    newOrbit.SetFromOrbit(templateOrbit);
                    newOrbit.GetComponentInChildren<OrbitViz3D>().isPreview = false;
                    selectedSatellite.SwitchOrbit(newOrbit, newOmega);
                    SetSelectedSatellite(); //Reset
                }
            }
            else
            {
                templateOrbit.gameObject.SetActive(false);
            }
        }
        else
        {
            templateOrbit.gameObject.SetActive(false);
        }
    }

    public int GetNumObjectives()
    {
        return objectives.Length;
    }
}