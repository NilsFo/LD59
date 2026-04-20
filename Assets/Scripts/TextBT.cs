using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TextBT : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text myText;
    public string text;
    private bool hovered;
    public UnityEvent onClick;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = myText.text;
        hovered = false;

        if (onClick == null)
        {
            onClick = new UnityEvent();
        }
    }

    // Update is called once per frame
    void Update()
    {
        string s = text;
        if (hovered)
        {
            s = ">" + s;
        }

        myText.text = s;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Click();
    }

    public void Click()
    {
        onClick.Invoke();
    }
}