using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIPointerListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public UnityEvent onMouseEnter;
    public UnityEvent onMouseExit;
    public UnityEvent onMouseClick;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (onMouseEnter == null)
        {
            onMouseEnter = new UnityEvent();
        }
        if (onMouseExit == null)
        {
            onMouseExit = new UnityEvent();
        }
        if (onMouseClick == null)
        {
            onMouseClick = new UnityEvent();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onMouseEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onMouseExit.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onMouseClick.Invoke();
    }
}