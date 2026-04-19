using UnityEngine;
using UnityEngine.EventSystems;

public class HoverDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameState _gameState;
    [Header("Description:")] [TextArea] public string description;

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

    private void UnitedEnter()
    {
        DisplayText();
    }

    private void UnitedExit()
    {
        if (!keep)
        {
            _gameState.ClearDisplayDescription();
        }
    }

    private void OnMouseEnter()
    {
        UnitedEnter();
    }

    private void OnMouseExit()
    {
        UnitedExit();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        UnitedEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnitedExit();
    }
}