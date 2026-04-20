using System;
using UnityEngine;
using UnityEngine.UI;

public class GlobalSignalStrength : MonoBehaviour
{
    public Slider slider;
    private GameState _gameState;

    private bool winTriggered = false;
    public bool debug;
    private float _count;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float currentWinProgress = _gameState.winning;
        float displayedProgress = currentWinProgress;

        displayedProgress = Math.Clamp(displayedProgress, 0, 1);
        slider.value = displayedProgress;

        if (!winTriggered)
        {
            winTriggered = true;
            Win();
        }
    }

    [ContextMenu("Win!")]
    public void Win()
    {
        _gameState.Win();
    }
}