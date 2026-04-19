using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TimeScaler : MonoBehaviour
{
    public UnityEvent onTimeFastForward;
    public UnityEvent onTimeNormal;
    public UnityEvent onTimePause;
    public UnityEvent onTimeChanged;

    public enum TimeScaleState
    {
        Normal,
        Pause,
        FastForward
    }

    public TimeScaleState currentTimeScaleState = TimeScaleState.Normal;
    private TimeScaleState _timeScaleState = TimeScaleState.Normal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (onTimeFastForward == null) onTimeFastForward = new UnityEvent();
        if (onTimeNormal == null) onTimeNormal = new UnityEvent();
        if (onTimePause == null) onTimePause = new UnityEvent();
        if (onTimeChanged == null) onTimeChanged = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        if (_timeScaleState != currentTimeScaleState)
        {
            OnTimeChanged();
            _timeScaleState = currentTimeScaleState;
        }

        // Detect player input

        Keyboard k = Keyboard.current;
        if (k.spaceKey.wasPressedThisFrame)
        {
            if (currentTimeScaleState == TimeScaleState.Pause)
            {
                TimeNormal();
            }
            else
            {
                TimePause();
            }
        }

        if (k.fKey.wasPressedThisFrame)
        {
            TimeFastForward();
        }
    }

    private void OnTimeChanged()
    {
        switch (currentTimeScaleState)
        {
            case TimeScaleState.Normal:
                onTimeNormal.Invoke();
                Time.timeScale = 1;
                break;
            case TimeScaleState.FastForward:
                onTimeFastForward.Invoke();
                Time.timeScale = 5;
                break;
            case TimeScaleState.Pause:
                onTimePause.Invoke();
                Time.timeScale = 0;
                break;
            default:
                Debug.LogError("ERROR IN TIME SCALE STATE!");
                break;
        }

        onTimeChanged.Invoke();
    }

    public void TimeFastForward()
    {
        currentTimeScaleState = TimeScaleState.FastForward;
    }

    public void TimeNormal()
    {
        currentTimeScaleState = TimeScaleState.Normal;
    }

    public void TimePause()
    {
        currentTimeScaleState = TimeScaleState.Pause;
    }
}