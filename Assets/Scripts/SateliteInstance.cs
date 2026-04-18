using System;
using UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public float omega;
    public Orbit orbit;

    public bool setNewOrbit = true;

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
        // orbit
        omega += Time.deltaTime * 10;
        transform.localPosition = orbit.GetOrbitPosition(omega);

        // name tf
        nameTF.text = displayName;
        nameTF.gameObject.transform.parent.gameObject.SetActive(showNameTF);
        
        if (setNewOrbit)
        {
            RaycastHit hit;
            var mousePos = Mouse.current.position.value;
            Ray ray = _gameState.GetCamera().ScreenPointToRay(mousePos);
            
            if (Physics.Raycast(ray, out hit)) {
                Transform objectHit = hit.transform;
                
                float newOmega = _gameState.templateOrbit.SetNewOrbit(transform.position, hit.point);
                _gameState.templateOrbit.gameObject.SetActive(true);
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    var newOrbit = Instantiate(_gameState.templateOrbit, Vector3.zero, Quaternion.identity);
                    newOrbit.SetFromOrbit(_gameState.templateOrbit);
                    SwitchOrbit(newOrbit, newOmega);
                }
            }
            else
            {
                _gameState.templateOrbit.gameObject.SetActive(false);
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
