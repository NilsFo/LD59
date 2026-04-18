using UnityEngine;

public class DebugTimeScale : MonoBehaviour
{
    public float timeScale = 1;
    public bool limitFPS = false;
    public int targetFPS = 30;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus))
        {
            timeScale *= 2;
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
        {
            timeScale /= 2;
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            timeScale = 0;
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            timeScale = 1;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            timeScale = 2;
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            timeScale = 3;
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            timeScale = 4;
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            timeScale = 5;
        }

        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            timeScale = 6;
        }

        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            timeScale = 7;
        }

        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            timeScale = 8;
        }

        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            timeScale = 9;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            limitFPS = !limitFPS;
        }
#endif
        if (limitFPS)
            Application.targetFrameRate = targetFPS;
        else
            Application.targetFrameRate = -1;
        Time.timeScale = Mathf.Clamp(timeScale, 0, 100);
    }
}