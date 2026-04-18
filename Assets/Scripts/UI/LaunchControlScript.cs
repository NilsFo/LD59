using UnityEngine;

public class LaunchControlScript : MonoBehaviour
{

    private GameState _gameState;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameState = FindFirstObjectByType<GameState>();
    }

    public void AddSatellite()
    {
        _gameState.AddSatellite();
    }
}
