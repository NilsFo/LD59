using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameQuitter : MonoBehaviour
{
    public bool allowQuitting = true;
    public UnityEvent onQuit;

    // Start is called before the first frame update
    void Start()
    {
        if (onQuit == null)
        {
            onQuit = new UnityEvent();
        }
    }

    public void QuitGame()
    {
        if (allowQuitting)
        {
            Debug.LogWarning("Quitting Game", gameObject);
            onQuit.Invoke();
            Application.Quit();
        }
    }
}