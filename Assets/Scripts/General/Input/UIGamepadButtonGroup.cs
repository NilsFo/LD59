using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIGamepadButtonGroup : MonoBehaviour
{
    [Header("Configuration")] public int selectedIndex;
    private int _selectedIndex;
    public bool reverseSelection = false;

    [Header("Button Group")] public List<UIGamepadButtonRelay> buttonGroup;
    public List<GameObject> selectionIndicators;

    [Header("Gamepad Button Pairs to listen for:")]
    public bool triggers = false;

    public bool shoulderButtons = false;
    public bool dPadHorizontal = false;
    public bool dPadVertical = false;
    private Vector2Int _dpadLastFrame;

    private GamepadInputDetector _gamepadInputDetector;
    private bool _isGamePad;

    private void Awake()
    {
        _gamepadInputDetector = FindObjectOfType<GamepadInputDetector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _dpadLastFrame = Vector2Int.zero;
        _selectedIndex = -1;
        _isGamePad = _gamepadInputDetector.isGamePad;
    }

    // Update is called once per frame
    void Update()
    {
        // Updating selection done via inspector
        SetSelectedIndex(selectedIndex);

        // Updating Button group if selection changed
        if (selectedIndex != _selectedIndex)
        {
            UpdateButtonGroup();
            _selectedIndex = selectedIndex;
        }

        // Detecting user input
        if (_gamepadInputDetector.isGamePad)
        {
            Gamepad gamepad = Gamepad.current;

            if (gamepad != null)
            {
                Vector2 dpadRaw = gamepad.dpad.ReadValue();
                Vector2Int dpad = new Vector2Int((int)dpadRaw.x, (int)dpadRaw.y);

                // Checking if controlled by triggers
                if (triggers)
                {
                    if (gamepad.leftTrigger.wasPressedThisFrame)
                    {
                        Previous();
                    }

                    if (gamepad.rightTrigger.wasPressedThisFrame)
                    {
                        Next();
                    }
                }

                // Checking if controlled by shoulder buttons
                if (shoulderButtons)
                {
                    if (gamepad.leftShoulder.wasPressedThisFrame)
                    {
                        Previous();
                    }

                    if (gamepad.rightShoulder.wasPressedThisFrame)
                    {
                        Next();
                    }
                }

                // Checking if controlled by dpad horizontal
                if (dPadHorizontal)
                {
                    if (DpadLeftWasPressedThisFrame(dpad))
                    {
                        Previous();
                    }

                    if (DpadRightWasPressedThisFrame(dpad))
                    {
                        Next();
                    }
                }

                // Checking if controlled by dpad horizontal
                if (dPadVertical)
                {
                    if (DpadDownWasPressedThisFrame(dpad))
                    {
                        Previous();
                    }

                    if (DpadUpWasPressedThisFrame(dpad))
                    {
                        Next();
                    }
                }

                // Updating the last known dpad input
                _dpadLastFrame = dpad;
            }
        }

        // Updating selection group if gamepad / kbm mode changes
        if (_isGamePad != _gamepadInputDetector.isGamePad)
        {
            UpdateButtonGroup();
            _isGamePad = _gamepadInputDetector.isGamePad;
        }
    }

    private void UpdateButtonGroup()
    {
        for (int i = 0; i < buttonGroup.Count; i++)
        {
            UIGamepadButtonRelay button = buttonGroup[i];
            if (button != null && _gamepadInputDetector.isGamePad)
            {
                button.enabled = i == selectedIndex;
            }
        }

        for (int i = 0; i < selectionIndicators.Count; i++)
        {
            GameObject selectionIndicator = selectionIndicators[i];
            if (selectionIndicator != null && _gamepadInputDetector.isGamePad)
            {
                selectionIndicator.SetActive(i == selectedIndex);
            }
        }

        if (!_gamepadInputDetector.isGamePad)
        {
            foreach (GameObject indicator in selectionIndicators)
            {
                indicator.SetActive(false);
            }
        }
    }

    public void Next()
    {
        if (reverseSelection)
            SetSelectedIndex(selectedIndex - 1);
        else
            SetSelectedIndex(selectedIndex + 1);
    }

    public void Previous()
    {
        if (reverseSelection)
            SetSelectedIndex(selectedIndex + 1);
        else
            SetSelectedIndex(selectedIndex - 1);
    }

    private void SetSelectedIndex(int newIndex)
    {
        int max = 0;
        if (buttonGroup.Count > 0)
        {
            max = buttonGroup.Count - 1;
        }

        selectedIndex = Mathf.Clamp(newIndex, 0, max);
    }

    #region dpadDecoding

    //gamepad.leftShoulder.wasPressedThisFrame
    private bool DpadLeftWasPressedThisFrame(Vector2Int dpad)
    {
        return DpadLeftIsPressed(dpad) && _dpadLastFrame.x == 0;
    }

    private bool DpadLeftIsPressed(Vector2Int dpad)
    {
        return dpad.x == -1;
    }

    private bool DpadRightWasPressedThisFrame(Vector2Int dpad)
    {
        return DpadRightIsPressed(dpad) && _dpadLastFrame.x == 0;
    }

    private bool DpadRightIsPressed(Vector2Int dpad)
    {
        return dpad.x == 1;
    }

    private bool DpadUpWasPressedThisFrame(Vector2Int dpad)
    {
        return DpadUpIsPressed(dpad) && _dpadLastFrame.y == 0;
    }

    private bool DpadUpIsPressed(Vector2Int dpad)
    {
        return dpad.y == 1;
    }

    private bool DpadDownWasPressedThisFrame(Vector2Int dpad)
    {
        return DpadDownIsPressed(dpad) && _dpadLastFrame.y == 0;
    }

    private bool DpadDownIsPressed(Vector2Int dpad)
    {
        return dpad.y == -1;
    }

    #endregion
}