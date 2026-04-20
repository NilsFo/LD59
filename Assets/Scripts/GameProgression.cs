using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameProgression : MonoBehaviour
{
    private GameState _gameState;
    public Slider slider;

    private bool completed = false;
    public UnityEvent onAllColoniesDiscovered;

    public Image progressionIcon;
    public Sprite newProgressSprite;

    private void Awake()
    {
        _gameState = FindFirstObjectByType<GameState>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        int objectiveCounts = _gameState.colonies.Length;
        Objective[] foundObjectives = _gameState.colonies.Where(objective =>
            objective.ObjectiveState == Objective.ObjectiveStateEnum.Completed ||
            objective.ObjectiveState == Objective.ObjectiveStateEnum.Explored).ToArray();
        int foundObjectivesCount = foundObjectives.Length;

        print(foundObjectivesCount + "/" + objectiveCounts);

        float progress = (float)(foundObjectivesCount) / (float)objectiveCounts;
        slider.value = progress;

        if (objectiveCounts == foundObjectivesCount)
        {
            if (!completed)
            {
                completed = true;
                OnAllDiscovered();
            }
        }
    }

    public void OnAllDiscovered()
    {
        onAllColoniesDiscovered.Invoke();
        _gameState.OnAllColoniesDiscovered();
    }
}