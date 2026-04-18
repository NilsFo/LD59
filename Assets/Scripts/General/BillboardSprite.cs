using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    private Camera _mainCamera;
    private GameState _gameState;

    // Start is called before the first frame update
    void Start()
    {
        _gameState = FindFirstObjectByType<GameState>();
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _mainCamera = _gameState.GetCamera();
        transform.LookAt(_mainCamera.transform);
        transform.Rotate(0, 180, 0);
    }
}