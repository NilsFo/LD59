using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GameState : MonoBehaviour
{
    [Header("Kaufhaus")] public int maxSatellites = 9;
    public int[] costOfSatellite = { 500, 1000, 1500, 2000, 2500 };

    public int camCost = 100;
    public int scanCost = 50;
    public int commCost = 500;

    public int refuelCost = 1000;
    public int fuelPlusCost = 200;
    public int fuelPlusAmount = 25;

    public int changeOrbitCostFuel = 25;
    public int leoCostFuel = 25;
    public int meoCostFuel = 50;
    public int geoCostFuel = 100;

    public enum SelectionState
    {
        None,
        Init,
        Selected,
        SatelliteReroute
    }

    [Header("Config")] public SelectionState selectionState = SelectionState.None;
    [SerializeField] private float _currentDelay = 0f;
    [SerializeField] private float _maxDelay = 5f;

    [SerializeField] private float uptimeThreshold = 0.8f;
    [SerializeField] private float winUptime = 30f;

    public List<GameObject> listOfSatellites;

    public TimeScaler TimeScaler => _timeScaler;
    private TimeScaler _timeScaler;
    public Economy economy;

    public Objective[] objectives;
    public Objective[] colonies;
    public float commUptime = 0f;

    [SerializeField] [CanBeNull] private SatelliteInstance selectedSatellite;
    public event Action<SatelliteInstance> OnSelectedSatelliteChanged;

    private Camera _mainCamera;

    [Header("World Hookup")] public GameObject prefabSatellite;
    public GameObject prefabMiniMapIcon;
    public GameObject prefabOrbit;
    public Orbit templateOrbit;
    private MusicManager _musicManager;
    [SerializeField] private TextScroller radioDisplay;
    [SerializeField] private TextScroller descriptionDisplay;
    public RectTransform miniMapTransform;

    private void Awake()
    {
        _timeScaler = FindFirstObjectByType<TimeScaler>();
        _mainCamera = FindFirstObjectByType<Camera>();
        _musicManager = FindAnyObjectByType<MusicManager>();
        templateOrbit.gameObject.SetActive(false);

        objectives = FindObjectsByType<Objective>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        colonies = objectives.Where(objective => objective.objectiveType == Objective.ObjectiveTypeEnum.Colony)
            .ToArray();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (listOfSatellites == null) listOfSatellites = new List<GameObject>();

        Application.targetFrameRate = 60;
        _musicManager.Stop();
        _musicManager.Play(1, stopOthers: true);
        MusicManager.userDesiredMasterVolume = 0.5f;

        DisplayRadioMsg("Hey kid.\n" +
                        "You are a new guy on the job. Since you graduated idiot school, " +
                        "I will show you the ropes.\n" +
                        "This is gonna take a lot of text to describe all you have to do.\n" +
                        "But this text is very long to compensate for your stupidity.");
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
                selectionState = SelectionState.Selected;
            }
        }

        if (HasAllColoniesConnection())
        {
            commUptime += Time.deltaTime;
            //TODO Maybe Set State and Display Connections on all Satellites
        }
        else
        {
            commUptime = 0f;
        }

        if (commUptime >= winUptime)
        {
            playWin();
        }
    }

    private bool HasAllColoniesConnection()
    {
        if (colonies.Length < 3) return false;
        Objective[] belowThreshold =
            colonies.Where(objective => objective.CommunicationUptime < uptimeThreshold).ToArray();
        return belowThreshold.Length == 0;
    }

    private void playWin()
    {
        //TODO
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
        if (!HasSpaceForMoreSatellites())
        {
            print("It's crowded up there! No more Sattellites");
            return false;
        }

        int index = listOfSatellites.Count;
        if (index > costOfSatellite.Length - 1)
        {
            index = costOfSatellite.Length - 1;
        }

        var newBalance = economy.Money - costOfSatellite[index];
        if (newBalance >= 0)
        {
            print("Buying a sat.");

            economy.Money = newBalance;

            GameObject orbitInstance = Instantiate(prefabOrbit, Vector3.zero, Quaternion.identity);
            GameObject satInstance = Instantiate(prefabSatellite, transform);
            listOfSatellites.Add(satInstance);

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

    public void SetSelectedSatellite(SatelliteInstance sat = null, bool skipDelay = false)
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
            if (skipDelay) selectionState = SelectionState.Selected;
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
                templateOrbit.height = selectedSatellite.orbit.height;
                templateOrbit.gameObject.SetActive(true);
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    if (selectedSatellite.CanAffordChangeOrbit())
                    {
                        selectedSatellite.payChangeOrbit();
                        var newOrbit = Instantiate(templateOrbit, Vector3.zero, Quaternion.identity);
                        newOrbit.SetFromOrbit(templateOrbit);
                        newOrbit.GetComponentInChildren<OrbitViz3D>().isPreview = false;
                        selectedSatellite.SwitchOrbit(newOrbit, newOmega);
                        SetSelectedSatellite(); //Reset
                    }
                    else
                    {
                        //TODO Feedback User can afford ChangeOrbit
                        print("Ups! You can't afford that! Not enough fule left!");
                    }
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

    public void DisplayRadioMsg(string text, bool instantly = false)
    {
        radioDisplay.DisplayText(text, instantly);
    }

    public void DisplayDescription(string text, bool instantly = false)
    {
        descriptionDisplay.DisplayText(text, instantly);
    }

    public void ClearDisplayRadioMsg()
    {
        radioDisplay.Clear();
    }

    public void ClearDisplayDescription()
    {
        descriptionDisplay.Clear();
    }

    public int GetNumObjectives()
    {
        return objectives.Length;
    }

    public bool HasSpaceForMoreSatellites()
    {
        if (listOfSatellites.Count >= maxSatellites)
        {
            return false;
        }

        return true;
    }

    public void SetReroute()
    {
        if (selectionState != SelectionState.Selected)
        {
            return;
        }

        selectionState = SelectionState.SatelliteReroute;
    }
}