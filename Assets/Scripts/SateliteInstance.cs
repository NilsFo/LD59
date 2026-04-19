using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SatelliteInstance : MonoBehaviour
{
    public enum SatFunctions
    {
        CAM,
        SCAN,
        COMM
    }

    const string Glyphs = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public static Dictionary<String, SatelliteInstance> nameLookup = new Dictionary<string, SatelliteInstance>();

    ////////////////////////////////////////
    // Params
    ////////////////////////////////////////
    [Header("Params")] public string displayName;
    public int fuelMax, fuelCurrent;
    
    public float discoverAngle = 10f;
    public float abandonedSiteAngle = 10f;
    public float colonyAngle = 30f;
    public float surveyAngle = 10f;

    public Color color;
    public Color colorMuted;

    public SatFunctions satFunction = SatFunctions.CAM;
    public event Action<SatFunctions> OnSatFunktionChanged;

    [Header("Properties")] public Vector2 position;
    public bool isHighLighted = false;
    [SerializeField] private bool isSelected = false;

    [Header("World hookup")] public TextMeshProUGUI nameTF;
    public HoverDescription myDescription;

    private GameState _gameState;
    private SatellitDisplayScript _displayScript;

    public event Action OnSatelliteDestroy;

    public float omega;
    public Orbit orbit;

    public bool IsSelected
    {
        get => isSelected;
        set
        {
            if (isSelected == value) return;
            isSelected = value;
            if (isSelected)
            {
                _gameState.SetSelectedSatellite(this);
            }
        }
    }

    public HaloViz signalHalo;

    private void Awake()
    {
        _gameState = FindFirstObjectByType<GameState>();
        _displayScript = FindFirstObjectByType<SatellitDisplayScript>();
        SetNewName();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _displayScript.RegisterSatellite(this);
        objectiveInSight = new bool[_gameState.GetNumObjectives()];
    }

    // Update is called once per frame
    void Update()
    {
        // orbit
        omega += Time.deltaTime * orbit.rotationSpeed;
        transform.localPosition = orbit.GetOrbitPosition(omega);
        transform.LookAt(Vector3.zero);

        // name tf
        nameTF.text = displayName;
        nameTF.gameObject.transform.parent.gameObject.SetActive(false);
        myDescription.description = displayName;

        nameTF.color = colorMuted;
        if (isSelected)
        {
            nameTF.color = color;
        }

        UpdateHaloViz();
    }

    private void FixedUpdate()
    {
        UpdateObjectivesInSight();
        ObjectivePayday();
    }

    private void UpdateHaloViz()
    {
        
        if (isSelected)
        {
            signalHalo.gameObject.SetActive(true);
            signalHalo.height = orbit.height - 1f;
            switch (satFunction)
            {
                case SatFunctions.CAM:
                    signalHalo.angle = discoverAngle;
                    break;
                case SatFunctions.SCAN:
                    signalHalo.angle = surveyAngle;
                    break;
                case SatFunctions.COMM:
                    signalHalo.angle = colonyAngle;
                    break;
                default:
                    signalHalo.angle = discoverAngle;
                    break;
            }
        }
        else
        {
            signalHalo.gameObject.SetActive(false);
        }
    }


    public bool[] objectiveInSight;

    public void UpdateObjectivesInSight()
    {
        float discover = Mathf.Cos(Mathf.Atan(Mathf.Tan(discoverAngle * Mathf.Deg2Rad) * (orbit.height - 1)));
        float colony = Mathf.Cos(Mathf.Atan(Mathf.Tan(colonyAngle * Mathf.Deg2Rad) * (orbit.height - 1)));
        float abandon = Mathf.Cos(Mathf.Atan(Mathf.Tan(abandonedSiteAngle * Mathf.Deg2Rad) * (orbit.height - 1)));
        float survey = Mathf.Cos(Mathf.Atan(Mathf.Tan(surveyAngle * Mathf.Deg2Rad) * (orbit.height - 1)));

        int heightIndex = (int)orbit.orbitState;
        for (var index = 0; index < _gameState.objectives.Length; index++)
        {
            var objective = _gameState.objectives[index];
            //Debug.Log(Vector3.Dot(objective.transform.position.normalized, transform.position.normalized));
            var dot = Mathf.Abs(Vector3.Dot(objective.transform.position.normalized, transform.position.normalized));
            var inSight = false;
            switch (objective.ObjectiveState)
            {
                case Objective.ObjectiveStateEnum.Hidden:
                    inSight = discover < dot;
                    break;
                case Objective.ObjectiveStateEnum.Unexplored:
                    inSight = discover < dot;
                    break;
                case Objective.ObjectiveStateEnum.Explored:
                    switch (objective.objectiveType)
                    {
                        case Objective.ObjectiveTypeEnum.AbandonedSite:
                            inSight = abandon < dot && heightIndex == 0;
                            break;
                        case Objective.ObjectiveTypeEnum.MineralSurvey:
                            inSight = survey < dot && heightIndex <= 1;
                            break;
                        case Objective.ObjectiveTypeEnum.Colony:
                            inSight = colony < dot;
                            break;
                    }

                    break;
                case Objective.ObjectiveStateEnum.Completed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            objectiveInSight[index] = inSight;
        }
    }

    public void ObjectivePayday()
    {
        for (var index = 0; index < _gameState.objectives.Length; index++)
        {
            if (objectiveInSight[index])
            {
                var objective = _gameState.objectives[index];
                objective.Payday(this);
            }
        }
    }

    public void SwitchOrbit(Orbit newOrbit, float newOmega)
    {
        Destroy(orbit.gameObject);
        orbit = newOrbit;
        omega = newOmega;
    }

    private void OnMouseEnter()
    {
        isHighLighted = true;
    }

    private void OnMouseExit()
    {
        isHighLighted = false;
    }

    private void OnMouseDown()
    {
        if (!IsSelected)
        {
            IsSelected = true;
        }
    }

    private void OnDestroy()
    {
        OnSatelliteDestroy?.Invoke();
    }

    private void SetNewName()
    {
        if (displayName == null || displayName.Length == 0)
        {
            string candidateName;
            do
            {
                candidateName = GenerateName();
            } while (nameLookup.ContainsKey(candidateName));

            displayName = candidateName;
            nameLookup[candidateName] = this;

            float h = Random.value;
            color = Color.HSVToRGB(
                H: h,
                S: 0.85f,
                V: 0.8f
            );

            colorMuted = Color.HSVToRGB(
                H: h,
                S: 0.5f,
                V: 0.5f
            );
        }
    }

    private string GenerateName()
    {
        string newName = "";
        for (int i = 0; i < 3; i++)
        {
            newName += Glyphs[Random.Range(0, Glyphs.Length)];
        }

        newName += "-" + Random.Range(1, 10);
        return newName;
    }

    public bool CanAffordCam()
    {
        return _gameState.economy.Money >= _gameState.camCost;
    }

    public bool BuyCam()
    {
        if (CanAffordCam())
        {
            _gameState.economy.Money -= _gameState.camCost;
            satFunction = SatFunctions.CAM;
            OnSatFunktionChanged?.Invoke(satFunction);
            return true;
        }

        return false;
    }

    public bool BuyScan()
    {
        if (CanAffordScan())
        {
            _gameState.economy.Money -= _gameState.scanCost;
            satFunction = SatFunctions.SCAN;
            OnSatFunktionChanged?.Invoke(satFunction);
            return true;
        }

        return false;
    }

    public bool CanAffordScan()
    {
        return _gameState.economy.Money >= _gameState.scanCost;
    }

    public bool BuyComm()
    {
        if (CanAffordComm())
        {
            _gameState.economy.Money -= _gameState.camCost;
            satFunction = SatFunctions.COMM;
            OnSatFunktionChanged?.Invoke(satFunction);
            return true;
        }

        return false;
    }

    public bool CanAffordComm()
    {
        return _gameState.economy.Money >= _gameState.camCost;
    }

    public bool BuyLeo()
    {
        if (CanAffordLeo())
        {
            fuelCurrent -= _gameState.leoCostFuel;
            orbit.SetLeo();
            return true;
        }

        return false;
    }

    public bool CanAffordLeo()
    {
        return fuelCurrent >= _gameState.leoCostFuel;
    }

    public bool BuyMeo()
    {
        if (CanAffordMeo())
        {
            fuelCurrent -= _gameState.meoCostFuel;
            orbit.SetMeo();
            return true;
        }

        return false;
    }

    public bool CanAffordMeo()
    {
        return fuelCurrent >= _gameState.meoCostFuel;
    }

    public bool BuyGeo()
    {
        if (CanAffordGeo())
        {
            fuelCurrent -= _gameState.meoCostFuel;
            orbit.SetGeo();
            return true;
        }

        return false;
    }

    public bool CanAffordGeo()
    {
        return fuelCurrent >= _gameState.meoCostFuel;
    }

    public bool BuyRefuel()
    {
        if (CanAffordRefuel())
        {
            _gameState.economy.Money -= _gameState.refuelCost;
            fuelCurrent = fuelMax;
            return true;
        }

        return false;
    }

    public bool CanAffordRefuel()
    {
        return _gameState.economy.Money >= _gameState.refuelCost;
    }

    public bool BuyPlusFuel()
    {
        if (CanAffordPlusFuel())
        {
            _gameState.economy.Money -= _gameState.refuelCost;
            fuelMax += 25;
            fuelCurrent = fuelMax;
            return true;
        }

        return false;
    }

    public bool CanAffordPlusFuel()
    {
        return _gameState.economy.Money >= _gameState.refuelCost;
    }
}
