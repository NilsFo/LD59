using System;
using System.Collections.Generic;
using UI;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SatelliteInstance : MonoBehaviour
{
    const string Glyphs = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public static Dictionary<String, SatelliteInstance> nameLookup = new Dictionary<string, SatelliteInstance>();
    
    ////////////////////////////////////////
    // Params
    ////////////////////////////////////////
    [Header("Params")] public string displayName;
    public int fuelMax, fuelCurrent;
    public int height;

    [Header("Properties")] public Vector2 position;
    public bool showNameTF;
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
        set {
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
        _displayScript.AddSatellit(this);
    }

    // Update is called once per frame
    void Update()
    {
        // orbit
        omega += Time.deltaTime * 10;
        transform.localPosition = orbit.GetOrbitPosition(omega);

        // name tf
        nameTF.text = displayName;
        nameTF.gameObject.transform.parent.gameObject.SetActive(showNameTF);
    }

    private void OnMouseEnter()
    {
        showNameTF = true;
    }

    private void OnMouseExit()
    {
        showNameTF = false;
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
        }
    }
    
    private string GenerateName()
    {
        string newName = "";
        for(int i=0; i<3; i++)
        {
            newName += Glyphs[Random.Range(0, Glyphs.Length)];
        }
        newName += " " + Random.Range(1, 10);
        return newName;
    }
}
