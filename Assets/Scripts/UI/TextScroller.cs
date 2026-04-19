using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TextScroller : MonoBehaviour
{
    [Header("World Hookup")] public TMP_Text myTMPText;
    [SerializeField] private string text;

    public UnityEvent onNewText;
    public float scrollSpeed = 1f;
    private float _charsDisplayed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (onNewText == null)
        {
            onNewText = new UnityEvent();
        }

        OnNewText();
    }

    // Update is called once per frame
    void Update()
    {
        myTMPText.text = text;
        _charsDisplayed += scrollSpeed * Time.unscaledDeltaTime;
        myTMPText.maxVisibleCharacters = (int)(_charsDisplayed);
    }

    private void OnNewText()
    {
        onNewText.Invoke();
        _charsDisplayed = 0;
    }

    public void DisplayText(string newText, bool instantly)
    {
        OnNewText();
        text = newText;
        if (instantly)
        {
            _charsDisplayed = newText.Length * 2;
        }
    }

    public void Clear()
    {
        text = "";
    }
}