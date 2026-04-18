using UnityEngine;

public class TimeScaleUI : MonoBehaviour
{

    private TimeScaler _timeScaler;

    public GameObject timeScaleButtonPlay,timeScaleButtonPause,timeScaleButtonFastForward;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _timeScaler = FindFirstObjectByType<TimeScaler>();
        _timeScaler.onTimeChanged.AddListener(OnTimeChanged);
        
        OnTimeChanged();
    }

    private void OnTimeChanged()
    {
        timeScaleButtonPlay.SetActive(false);
        timeScaleButtonPause.SetActive(false);
        timeScaleButtonFastForward.SetActive(false);

        switch (_timeScaler.currentTimeScaleState)
        {
            case TimeScaler.TimeScaleState.Normal:
                timeScaleButtonPlay.SetActive(true);
                break;
            case TimeScaler.TimeScaleState.FastForward:
                timeScaleButtonFastForward.SetActive(true);
                break;
            case TimeScaler.TimeScaleState.Pause:
                timeScaleButtonPause.SetActive(true);
                break;
            default:
                Debug.LogError("ERROR IN TIME SCALE STATE!");
                break;
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
        
    }
}
