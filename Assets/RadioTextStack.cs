using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RadioTextStack : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameState _gameState;
    public TextScroller scroller;
    public List<string> textHistory;

    public int selectedText;
    public TMP_Text counterTF;

    [Header("Initial Tutorial")] public List<string> initialTutorialTexts;
    public int basicTutorialProgress = 0;

    private void Awake()
    {
        _gameState = FindFirstObjectByType<GameState>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectedText = 0;
        AdvanceClickTutorial();
    }

    public void AdvanceClickTutorial()
    {
        if (basicTutorialProgress == initialTutorialTexts.Count)
        {
            print("Tutorial finished. You are on your own.");
            return;
        }

        _gameState.DisplayRadioMsg(initialTutorialTexts[basicTutorialProgress]);
        basicTutorialProgress += 1;
    }

    // Update is called once per frame
    void Update()
    {
        counterTF.text = (selectedText + 1) + "/" + textHistory.Count;
    }

    public void RequestPrev()
    {
        selectedText -= 1;
        selectedText = Math.Clamp(selectedText, 0, textHistory.Count - 1);
        SetMsg(textHistory[selectedText], true);
    }

    public void RequestNext()
    {
        selectedText += 1;
        selectedText = Math.Clamp(selectedText, 0, textHistory.Count - 1);
        SetMsg(textHistory[selectedText], true);
    }

    public void DisplayMsg(string msg)
    {
        selectedText = textHistory.Count;
        textHistory.Add(msg);
        SetMsg(msg, false);
    }

    private void SetMsg(string msg, bool instantly)
    {
        scroller.DisplayText(msg, instantly);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Click();
    }

    private void Click()
    {
        AdvanceClickTutorial();
    }
}