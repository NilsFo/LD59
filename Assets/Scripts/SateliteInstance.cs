using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class SatelliteInstance : MonoBehaviour
{
    public const int CamCost = 100;
    public const int ScanCost = 50;
    public const int CommCost = 500;

    public const int RefuleCost = 1000;
    public const int FulePlusCost = 200;

    public const int LeoCostFule = 25;
    public const int MeoCostFule = 50;
    public const int GeoCostFule = 100;

    public enum SatFunktions
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

    public Color color;
    public Color colorMuted;

    public SatFunktions satFunktion = SatFunktions.CAM;
    public event Action<SatFunktions> OnSatFunktionChanged;

    [Header("Properties")] public Vector2 position;
    public bool isHighLighted = false;
    [SerializeField] private bool isSelected = false;

    [Header("World hookup")] public TextMeshProUGUI nameTF;

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

        // name tf
        nameTF.text = displayName;
        nameTF.gameObject.transform.parent.gameObject.SetActive(isHighLighted || isSelected);

        nameTF.color = colorMuted;
        if (isSelected)
        {
            nameTF.color = color;
        }
    }

    private void FixedUpdate()
    {
        UpdateObjectivesInSight();
        ObjectivePayday();
    }

    private float[] discoverAngle =
        { 
            Mathf.Cos(20f * Mathf.Deg2Rad), 
            Mathf.Cos(30f * Mathf.Deg2Rad), 
            Mathf.Cos(50f * Mathf.Deg2Rad) 
        };

    private float[] abandonedSiteAngles =
    {
        Mathf.Cos(20f * Mathf.Deg2Rad), 
        Mathf.Cos(30f * Mathf.Deg2Rad), 
        Mathf.Cos(50f * Mathf.Deg2Rad)
    };

    private float[] colonyAngles =
    {
        Mathf.Cos(20f * Mathf.Deg2Rad), 
        Mathf.Cos(30f * Mathf.Deg2Rad), 
        Mathf.Cos(50f * Mathf.Deg2Rad)
    };

    private float[] surveyAngles =
    {
        Mathf.Cos(20f * Mathf.Deg2Rad), 
        Mathf.Cos(30f * Mathf.Deg2Rad), 
        Mathf.Cos(50f * Mathf.Deg2Rad)
    };

    public bool[] objectiveInSight;

    public void UpdateObjectivesInSight()
    {
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
                    inSight = discoverAngle[heightIndex] < dot;
                    break;
                case Objective.ObjectiveStateEnum.Unexplored:
                    inSight = discoverAngle[heightIndex] < dot;
                    break;
                case Objective.ObjectiveStateEnum.Explored:
                    switch (objective.objectiveType)
                    {
                        case Objective.ObjectiveTypeEnum.AbandonedSite:
                            inSight = abandonedSiteAngles[heightIndex] < dot && heightIndex == 0;
                            break;
                        case Objective.ObjectiveTypeEnum.MineralSurvey:
                            inSight = surveyAngles[heightIndex] < dot && heightIndex <= 1;
                            break;
                        case Objective.ObjectiveTypeEnum.Colony:
                            inSight = colonyAngles[heightIndex] < dot;
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

    public bool BuyCam()
    {
        if (_gameState.economy.Money >= CamCost)
        {
            _gameState.economy.Money -= CamCost;
            satFunktion = SatFunktions.CAM;
            OnSatFunktionChanged?.Invoke(satFunktion);
            return true;
        }

        return false;
    }

    public bool BuyScan()
    {
        if (_gameState.economy.Money >= ScanCost)
        {
            _gameState.economy.Money -= ScanCost;
            satFunktion = SatFunktions.SCAN;
            OnSatFunktionChanged?.Invoke(satFunktion);
            return true;
        }

        return false;
    }

    public bool BuyComm()
    {
        if (_gameState.economy.Money >= CommCost)
        {
            _gameState.economy.Money -= CommCost;
            satFunktion = SatFunktions.COMM;
            OnSatFunktionChanged?.Invoke(satFunktion);
            return true;
        }

        return false;
    }

    public bool BuyLeo()
    {
        if (fuelCurrent >= LeoCostFule)
        {
            fuelCurrent -= LeoCostFule;
            orbit.SetLeo();
            return true;
        }

        return false;
    }

    public bool BuyMeo()
    {
        if (fuelCurrent >= MeoCostFule)
        {
            fuelCurrent -= MeoCostFule;
            orbit.SetMeo();
            return true;
        }

        return false;
    }

    public bool BuyGeo()
    {
        if (fuelCurrent >= GeoCostFule)
        {
            fuelCurrent -= GeoCostFule;
            orbit.SetMeo();
            return true;
        }

        return false;
    }

    public bool BuyRefule()
    {
        if (_gameState.economy.Money >= RefuleCost)
        {
            _gameState.economy.Money -= RefuleCost;
            fuelCurrent = fuelMax;
            return true;
        }

        return false;
    }

    public bool BuyPlusFule()
    {
        if (_gameState.economy.Money >= FulePlusCost)
        {
            _gameState.economy.Money -= FulePlusCost;
            fuelMax += 25;
            fuelCurrent = fuelMax;
            return true;
        }

        return false;
    }
}