using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TextScroller : MonoBehaviour
{
    [Header("World Hookup")] public TMP_Text myTMPText;
    public string text;
    private string _text;

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
        if (_text != text)
        {
            _text = text;
            OnNewText();
        }

        myTMPText.text = text;
        _charsDisplayed += scrollSpeed * Time.unscaledDeltaTime;
        myTMPText.maxVisibleCharacters = (int)(_charsDisplayed);
    }

    private void OnNewText()
    {
        onNewText.Invoke();
        _charsDisplayed = 0;
    }

    public void Clear()
    {
        text = "";
    }
}