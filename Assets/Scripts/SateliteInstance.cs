using System;
using UI;
using TMPro;
using UnityEngine;

public class SatelliteInstance : MonoBehaviour
{
    ////////////////////////////////////////
    // Params
    ////////////////////////////////////////
    [Header("Params")] public string displayName;
    public int fuelMax, fuelCurrent;
    public int height;

    [Header("Properties")] public Vector2 position;
    public bool showNameTF;

    [Header("World hookup")] public TextMeshProUGUI nameTF;

    private GameState _gameState;
    private SatellitDisplayScript _displayScript;
    
    public event Action OnSatelliteDestroy;

    private void Awake()
    {
        _gameState = FindFirstObjectByType<GameState>();
        _displayScript = FindFirstObjectByType<SatellitDisplayScript>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _displayScript.AddSatellit(this);
    }

    // Update is called once per frame
    void Update()
    {
        nameTF.text = displayName;
        nameTF.gameObject.transform.parent.gameObject.SetActive(showNameTF);
    }

    private void OnMouseEnter()
    {
        showNameTF = true;
        Debug.Log("Sat: Mouse Enter!", gameObject);
    }

    private void OnMouseExit()
    {
        showNameTF = false;
        Debug.Log("Sat: Mouse Exit!", gameObject);
    }


    private void OnDestroy()
    {
        OnSatelliteDestroy?.Invoke();
    }
}
