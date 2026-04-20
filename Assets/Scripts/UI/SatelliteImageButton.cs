using System;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SatelliteImageButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public SatellitItemDisplayScript displayScript;
    private GameState _gameState;

    [Header("Hookup")] public Image image;
    public Sprite activeImage;
    public Sprite notBuyableImage;
    public Sprite buyableImage;
    public HoverDescription myHoverDescription;

    public enum SatelliteButtonType
    {
        MOVE,
        CAM,
        SCAN,
        COMM,
        LEO,
        MEO,
        GEO,
        MAXFUEL,
        REFUEL
    }

    [Header("What is this button?")] public SatelliteButtonType buttonType = SatelliteButtonType.CAM;

    public UnityEvent onClick;


    private void Awake()
    {
        _gameState = FindFirstObjectByType<GameState>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (onClick == null)
        {
            onClick = new UnityEvent();
        }

        // myHoverDescription.description = myHoverDescription + "\n" +
        //                                  "[Cost: " + GetCost() + "!]";
    }

    // Update is called once per frame
    void Update()
    {
        image.sprite = notBuyableImage;

        if (CanAfford())
        {
            image.sprite = buyableImage;
        }

        if (IsActiveInSat())
        {
            image.sprite = activeImage;
        }
    }

    public void Click()
    {
        print("Click on button: " + buttonType + " for: " + displayScript.satelliteInstance.displayName);
        onClick.Invoke();
        bool actionSucceeded = false;

        if (!CanAfford())
        {
            Debug.Log("Sorry player, this satellite can't give credit. Come back when you are little bit... richer!");
            _gameState.DisplayDescription("Insufficient funding or fuel!", false);

            return;
        }

        switch (buttonType)
        {
            case SatelliteButtonType.MOVE:
                actionSucceeded = displayScript.satelliteInstance.BuyChangeOrbit();
                break;
            case SatelliteButtonType.CAM:
                actionSucceeded = displayScript.satelliteInstance.BuyCam();
                break;
            case SatelliteButtonType.COMM:
                actionSucceeded = displayScript.satelliteInstance.BuyComm();
                break;
            case SatelliteButtonType.GEO:
                actionSucceeded = displayScript.satelliteInstance.BuyGeo();
                break;
            case SatelliteButtonType.LEO:
                actionSucceeded = displayScript.satelliteInstance.BuyLeo();
                break;
            case SatelliteButtonType.MAXFUEL:
                actionSucceeded = displayScript.satelliteInstance.BuyPlusFuel();
                break;
            case SatelliteButtonType.MEO:
                actionSucceeded = displayScript.satelliteInstance.BuyMeo();
                break;
            case SatelliteButtonType.REFUEL:
                actionSucceeded = displayScript.satelliteInstance.BuyRefuel();
                break;
            case SatelliteButtonType.SCAN:
                actionSucceeded = displayScript.satelliteInstance.BuyScan();
                break;
            default:
                Debug.LogWarning("Unknown sat type!");
                actionSucceeded = false;
                break;
        }

        if (actionSucceeded)
        {
            print("The button did its job.");
        }
        else
        {
            Debug.LogWarning("THE BUTTON FAILED AT WHAT EVER IT TRIED TO DO!!", gameObject);
        }
    }

    public string GetCost()
    {
        switch (buttonType)
        {
            case SatelliteButtonType.MOVE:
                return _gameState.changeOrbitCostFuel + " fuel";
            case SatelliteButtonType.CAM:
                return _gameState.camCost + "€";
            case SatelliteButtonType.COMM:
                return _gameState.commCost + "€";
            case SatelliteButtonType.GEO:
                return displayScript.satelliteInstance.GetGeoCost() + " fuel";
            case SatelliteButtonType.LEO:
                return displayScript.satelliteInstance.GetLeoCost() + " fuel";
            case SatelliteButtonType.MAXFUEL:
                return _gameState.fuelPlusCost + "€";
            case SatelliteButtonType.MEO:
                return _gameState.meoCostFuel + " fuel";
            case SatelliteButtonType.REFUEL:
                return _gameState.refuelCost + "€";
            case SatelliteButtonType.SCAN:
                return _gameState.scanCost + "€";
            default:
                return "????";
        }
    }

    private bool CanAfford()
    {
        switch (buttonType)
        {
            case SatelliteButtonType.MOVE:
                return displayScript.satelliteInstance.CanAffordChangeOrbit();
            case SatelliteButtonType.CAM:
                return displayScript.satelliteInstance.CanAffordCam();
            case SatelliteButtonType.COMM:
                return displayScript.satelliteInstance.CanAffordComm();
            case SatelliteButtonType.GEO:
                return displayScript.satelliteInstance.CanAffordGeo();
            case SatelliteButtonType.LEO:
                return displayScript.satelliteInstance.CanAffordLeo();
            case SatelliteButtonType.MAXFUEL:
                return displayScript.satelliteInstance.CanAffordPlusFuel();
            case SatelliteButtonType.MEO:
                return displayScript.satelliteInstance.CanAffordMeo();
            case SatelliteButtonType.REFUEL:
                return displayScript.satelliteInstance.CanAffordRefuel();
            case SatelliteButtonType.SCAN:
                return displayScript.satelliteInstance.CanAffordScan();
        }

        Debug.LogWarning("Illegal button state!");
        return false;
    }

    private bool IsActiveInSat()
    {
        switch (buttonType)
        {
            case SatelliteButtonType.CAM:
                return displayScript.satelliteInstance.satFunction == SatelliteInstance.SatFunctions.CAM;
            case SatelliteButtonType.COMM:
                return displayScript.satelliteInstance.satFunction == SatelliteInstance.SatFunctions.COMM;
            case SatelliteButtonType.GEO:
                return displayScript.satelliteInstance.orbit.orbitState == Orbit.OrbitState.GEO;
            case SatelliteButtonType.LEO:
                return displayScript.satelliteInstance.orbit.orbitState == Orbit.OrbitState.LEO;
            case SatelliteButtonType.MEO:
                return displayScript.satelliteInstance.orbit.orbitState == Orbit.OrbitState.MEO;
            case SatelliteButtonType.SCAN:
                return displayScript.satelliteInstance.satFunction == SatelliteInstance.SatFunctions.SCAN;
        }

        return false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        displayScript.costTF.text = GetCost();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        displayScript.costTF.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Click();
    }
}