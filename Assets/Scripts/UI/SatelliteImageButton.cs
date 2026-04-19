using System;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SatelliteImageButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public SatellitItemDisplayScript displayScript;

    public Image image;
    public Sprite activeImage;
    public Sprite notBuyableImage;
    public Sprite buyableImage;

    public UnityEvent onClick;

    public enum SatelliteButtonType : UInt64
    {
        CAM,
        SCAN,
        COMM,
        LEO,
        MEO,
        GEO,
        MAXFUEL,
        REFUEL
    }

    public SatelliteButtonType buttonType = SatelliteButtonType.CAM;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (onClick == null)
        {
            onClick = new UnityEvent();
        }
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
        onClick.Invoke();
    }

    private bool CanAfford()
    {
        switch (buttonType)
        {
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
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print("Click on button: " + buttonType + " for: " + displayScript.satelliteInstance.displayName);
    }
}