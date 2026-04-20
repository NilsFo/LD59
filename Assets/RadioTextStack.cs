using System;
using System.Collections.Generic;
using System.Linq;
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


    [Header("Initial Tutorial")] public int basicTutorialProgress = 0;

    private List<string> initialTutorialTexts = new string[]
    {
        "> ORFS Mission Control:\nWelcome to Mars. The last solar storm fried all of our satellites. We need to reestablish communication with the other colonies.\n[Click here to continue]]",
        "> ORFS Mission Control:\nFirst, launch a satellite to establish visual contact and scan the surface. Use the [LAUNCH] button on the right.\n[Click here to continue]]",
        "> ORFS Mission Control:\nThe satellite should find some points of interest on the surface. Use the [RIGHT MOUSE BUTTON] to rotate your view.\n[Click here to continue]]",
        "> ORFS Mission Control:\nWe need a closer look to see what we are dealing with. Equip the satellite with a camera [CAM] and scan the locations.\n[Click here to continue]]",
        "> ORFS Mission Control:\nThere are three types of sites of interest.\nCraters can be scanned for resources using a satellite with the mineral sensor [MSE]. The data is sold for money.\n[Click here to continue]]",
        "> ORFS Mission Control:\nAbandoned sites are no longer of value to us, but can be photographed with a camera [CAM] for a small sum of money.\n[Click here to continue]]",
        "> ORFS Mission Control:\nColonies are what we are looking for. Equip a satellite with a communication dish [COMM] and ensure it can phone home to ORFS. You may need more than one satellite to establish a connection over multiple signal relays.\n[Click here to continue]]",
        "> ORFS Mission Control:\nTo expand to other areas of Mars, use the maneuver button [MAN] to send a satellite into a new orbit. This will cost some fuel, but you can refuel the satellite using the [REF] button.\n[Click here to continue]]",
        "> ORFS Mission Control:\nFor a full communication network, we may need to boost our satellite to the Geostationary Mars Orbit [GMO]. This will cost more fuel than what we have currently, so save up some money and use [FUEL+] to increase your capacity.\n[Click here to continue]]",
        "> ORFS Mission Control:\nThis should be all the information you need. Reestablish communication with all colonies to over 90% uptime. The bar above your map shows your progress.\nThe colonies of Mars will be in your debt."
    }.ToList();

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