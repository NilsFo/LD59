using System;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class Objective : MonoBehaviour
{
    public enum ObjectiveTypeEnum
    {
        AbandonedSite, MineralSurvey, Colony
    }
    public enum ObjectiveStateEnum
    {
        Hidden, Unexplored, Explored, Completed
    }

    [SerializeField] ObjectiveStateEnum _objectiveState;
    
    [field: SerializeField] public ObjectiveStateEnum ObjectiveState
    {
        get => _objectiveState;
        set
        {
            if (_objectiveState != value)
            {
                _objectiveState = value;
                objectiveStateChanged.Invoke(_objectiveState);
            }
        }
    }
    public UnityEvent<ObjectiveStateEnum> objectiveStateChanged;


    public ObjectiveTypeEnum objectiveType;
    public float longitude, latitude;
    public int researchValue = 1;

    private GameState _gameState;

    [Header("Visuals")]
    public Transform unexploredViz;
    public Transform abandonedSiteViz;
    public Transform mineralSurveyViz;
    public Transform colonyViz;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CalcPos();
        _gameState = FindFirstObjectByType<GameState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.IsPlaying(gameObject))
        {
            // Play logic
        }
        else
        {
            CalcPos();
            UpdateViz();
        }
    }

    public void Payday()
    {
        // TODO PQ
    }

    public void UpdateViz()
    {
        if (ObjectiveState == ObjectiveStateEnum.Hidden && Application.IsPlaying(gameObject))
        {
            unexploredViz.gameObject.SetActive(false);
            abandonedSiteViz.gameObject.SetActive(false);
            mineralSurveyViz.gameObject.SetActive(false);
            colonyViz.gameObject.SetActive(false);
        } else if(ObjectiveState == ObjectiveStateEnum.Unexplored && Application.IsPlaying(gameObject))
        {
            unexploredViz.gameObject.SetActive(true);
            abandonedSiteViz.gameObject.SetActive(false);
            mineralSurveyViz.gameObject.SetActive(false);
            colonyViz.gameObject.SetActive(false);
        }
        else
        {
            unexploredViz.gameObject.SetActive(false);

            if (objectiveType == ObjectiveTypeEnum.Colony)
            {
                abandonedSiteViz.gameObject.SetActive(false);
                mineralSurveyViz.gameObject.SetActive(false);
                colonyViz.gameObject.SetActive(true);
            }
            if (objectiveType == ObjectiveTypeEnum.AbandonedSite)
            {
                abandonedSiteViz.gameObject.SetActive(true);
                mineralSurveyViz.gameObject.SetActive(false);
                colonyViz.gameObject.SetActive(false);
            }
            if (objectiveType == ObjectiveTypeEnum.MineralSurvey)
            {
                abandonedSiteViz.gameObject.SetActive(false);
                mineralSurveyViz.gameObject.SetActive(true);
                colonyViz.gameObject.SetActive(false);
            }
        }
    }

    public void CalcPos()
    {
        var longLatVec = LongLatToVector3(longitude, latitude);
        transform.position = longLatVec;
        transform.rotation = Quaternion.LookRotation(longLatVec, Vector3.up);
    }

    public static Vector3 LongLatToVector3(float lon, float lat)
    {
        lat = 90 - lat;
        return new Vector3(Mathf.Sin(Mathf.Deg2Rad*lat) * Mathf.Sin(Mathf.Deg2Rad*lon),
            Mathf.Cos(Mathf.Deg2Rad*lat),
            Mathf.Sin(Mathf.Deg2Rad*lat) * Mathf.Cos(Mathf.Deg2Rad*lon));
    }
}
