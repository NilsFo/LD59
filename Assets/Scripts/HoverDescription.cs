using UnityEngine;

public class HoverDescription : MonoBehaviour
{
    private GameState _gameState;
    [Header("Description:")] public string description;

    [Header("Config")] public bool keep = false;
    public bool instantly;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameState = FindFirstObjectByType<GameState>();
    }

    public void DisplayText()
    {
        print("New description: " + gameObject.name);
        _gameState.DisplayDescription(description, instantly);
    }

    private void OnMouseEnter()
    {
        DisplayText();
    }

    private void OnMouseExit()
    {
        if (!keep)
        {
            _gameState.ClearDisplayDescription();
        }
    }
}