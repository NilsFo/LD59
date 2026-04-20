using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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

    [SerializeField] public float winUptime = 30f;

    [SerializeField] public List<SatelliteInstance> listOfSatellites;

    public TimeScaler TimeScaler => _timeScaler;
    private TimeScaler _timeScaler;
    public Economy economy;

    public Objective[] objectives;
    public Objective[] colonies;
    public Objective home;

    public float winning = 0f;

    [SerializeField] [CanBeNull] private SatelliteInstance selectedSatellite;
    public event Action<SatelliteInstance> OnSelectedSatelliteChanged;

    private Camera _mainCamera;

    [Header("World Hookup")] public GameObject prefabSatellite;
    public GameObject prefabMiniMapIcon;
    public GameObject prefabFloatingText;
    public GameObject prefabOrbit;
    public Transform homeBasePosition;
    public Orbit templateOrbit;

    [FormerlySerializedAs("_musicManager")]
    public MusicManager musicManager;

    [SerializeField] private TextScroller radioDisplay;
    [SerializeField] private TextScroller descriptionDisplay;
    public RectTransform miniMapTransform;
    private GlobalSignalStrength _globalSignalStrength;
    private RadioTextStack _radioTextStack;

    [Header("Audio")] public AudioClip satSelect;

    // [Header("Tutorial texte")] public List<string> welcomeTutorialTexts;

    private void Awake()
    {
        _timeScaler = FindFirstObjectByType<TimeScaler>();
        _mainCamera = FindFirstObjectByType<Camera>();
        musicManager = FindAnyObjectByType<MusicManager>();
        _radioTextStack = FindFirstObjectByType<RadioTextStack>();
        templateOrbit.Hide();
        _globalSignalStrength = FindFirstObjectByType<GlobalSignalStrength>();

        objectives = FindObjectsByType<Objective>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        colonies = objectives.Where(objective => objective.objectiveType == Objective.ObjectiveTypeEnum.Colony)
            .ToArray();
        home = objectives.First(objective => objective.objectiveType == Objective.ObjectiveTypeEnum.Home);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (listOfSatellites == null) listOfSatellites = new List<SatelliteInstance>();

        Application.targetFrameRate = 60;
        musicManager.Stop();
        musicManager.Play(1, stopOthers: true);
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

        UpdateOrbitPreview();

        if (selectionState == SelectionState.Init)
        {
            _currentDelay -= Time.unscaledDeltaTime;
            if (_currentDelay <= 0f)
            {
                selectionState = SelectionState.Selected;
            }
        }

        CalcUptime();
    }

    private void CalcUptime()
    {
        float avgUptime = 0f;
        for (int i = 0; i < colonies.Length; i++)
        {
            var colony = colonies[i];
            avgUptime += colony.CommunicationUptimeProzent;
        }

        avgUptime /= colonies.Length;
        winning = avgUptime;
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

        var cost = CostOfNextSatellite();
        var newBalance = economy.Money - cost;
        if (newBalance >= 0)
        {
            print("Buying a sat.");

            economy.Money = newBalance;

            GameObject orbitInstance = Instantiate(prefabOrbit, Vector3.zero, Quaternion.identity);
            GameObject satInstance = Instantiate(prefabSatellite, transform);
            SatelliteInstance instance = satInstance.GetComponent<SatelliteInstance>();
            listOfSatellites.Add(instance);

            Orbit orbit = orbitInstance.GetComponent<Orbit>();
            //orbit.SetFromIncEq(Random.Range(-80, 80), Random.Range(0, 359));
            orbit.SetFromIncEq(34f, 90f);

            SatelliteInstance sat = satInstance.GetComponent<SatelliteInstance>();
            sat.orbit = orbit;
            sat.omega = 90f;

            ShowFloatingText(homeBasePosition.position, instance.displayName + " launched!", instance.color);

            if (listOfSatellites.Count == 1)
            {
                DisplayRadioMsg("> ORFS Mission Control:\n" +
                                "Congratulations on your first launch!\n" +
                                "You can change the equipment and orbit of your satellite with the panels on the right.\n" +
                                "You can launch more satellites as you seem fit.\n\n" +
                                "[Click here to continue.]");
            }

            return true;
        }
        else
        {
            DisplayDescription("Insufficient funding!", false);
            print("I can't give credit. Come back when you are... richer!");
        }

        return false;
    }

    public int CostOfNextSatellite()
    {
        int index = listOfSatellites.Count;
        if (index > costOfSatellite.Length - 1)
        {
            index = costOfSatellite.Length - 1;
        }

        return costOfSatellite[index];
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
            sat.IsSelected = true;
            musicManager.CreateAudioClip(satSelect, new Vector3());
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
                templateOrbit.orbitState = selectedSatellite.orbit.orbitState;
                templateOrbit.targetOrbitState = selectedSatellite.orbit.targetOrbitState;
                templateOrbit.Show(selectedSatellite.GetColorForViz());
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    if (selectedSatellite.CanAffordChangeOrbit())
                    {
                        selectedSatellite.payChangeOrbit();
                        var newOrbit = Instantiate(templateOrbit, Vector3.zero, Quaternion.identity);
                        newOrbit.SetFromOrbit(templateOrbit);
                        newOrbit.orbitViz3D.isPreview = false;
                        selectedSatellite.SwitchOrbit(newOrbit, newOmega);
                        musicManager.CreateAudioClip(selectedSatellite.satManeuver, Vector3.zero);
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
                templateOrbit.Hide();
            }
        }
        else
        {
            templateOrbit.Hide();
        }
    }

    public void DisplayRadioMsg(string text, bool instantly = false)
    {
        _radioTextStack.DisplayMsg(text);
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

    [ContextMenu("Win!")]
    public void Win()
    {
        print("A winner is you!");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public GameObject ShowFloatingText(Vector3 position, string text, Color color)
    {
        GameObject textObj = Instantiate(prefabFloatingText, Vector3.zero, Quaternion.identity);
        TMP_Text textTF = textObj.GetComponentInChildren<TMP_Text>();
        textTF.text = text;
        textTF.color = color;

        textObj.transform.LookAt(position);
        return textObj;
    }

    public void OnAllColoniesDiscovered()
    {
        Debug.Log("YOU DISCOVERED ALL COLONIES!");
        DisplayRadioMsg(
            "> ORFS Mission Control:\n" +
            "That should be all colonies! Great job so far!\n" +
            "Now you must establish a communication network!\n" +
            "Remember, only satellites in communication mode can relay signals from the colonies. " +
            "If you keep them in GMO they make the best relay."
        );
    }
}