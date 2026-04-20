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
    public Animator winAnim;

    private void Awake()
    {
        _gameState = FindFirstObjectByType<GameState>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winAnim.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        float currentWinProgress = _gameState.winning;
        float displayedProgress = currentWinProgress;
        if (debug)
        {
            _count += Time.deltaTime;
            displayedProgress += _count;
        }

        if (winTriggered)
        {
            displayedProgress = 1;
        }

        displayedProgress = Math.Clamp(displayedProgress, 0, 1);
        slider.value = displayedProgress;

        if (!winTriggered && displayedProgress >= 1)
        {
            winTriggered = true;
            Win();
        }
    }

    [ContextMenu("Win!")]
    public void Win()
    {
        _gameState.Win();
        winAnim.enabled = true;
        winAnim.Play("winAnimation");
    }

    public void OnContinue()
    {
        winAnim.gameObject.SetActive(false);
    }

    public void OnBackToMenu()
    {
        _gameState.BackToMenu();
    }
}