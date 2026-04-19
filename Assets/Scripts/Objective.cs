using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class Objective : MonoBehaviour
{
    public enum ObjectiveTypeEnum
    {
        AbandonedSite, MineralSurvey, Colony, Home
    }
    public enum ObjectiveStateEnum
    {
        Hidden, Unexplored, Explored, Completed
    }

    [Header("Params")] [Header("General")] 
    public string id;
    public string displayName;
    [TextArea(5,15)] public string description;
    
    public ObjectiveTypeEnum objectiveType;
    public float longitude, latitude;
    
    
    [SerializeField] private int payout = 100;
    [SerializeField] private float discoverCooldown = 3f;
    [SerializeField] private float exploreCooldown = 3f;
    [SerializeField] private float excavateCooldown = 3f;
    
    [Header("Colony")]
    [SerializeField] private float colonyUptimeDecay = 0.2f; //Decay of Uptime
    [SerializeField] private float colonyUptime = 2.0f; //Amount of Uptime
    [SerializeField] private float colonyProgress = 0.25f; //Amount of Progress
    [SerializeField] private float colonyCooldown = 3f; //Delay between Progress
    
    [Header("MineralSurvey")]
    [SerializeField] private float surveyProgressLeo = 0.10f;
    [SerializeField] private float surveyProgressMeo = 0.05f;
    [SerializeField] private float surveyProgressGeo = 0.01f;
    [SerializeField] private float surveyCooldown = 3f;
    
    [Header("AbandonedSite")]
    [SerializeField] private float siteProgressLeo = 0.10f;
    [SerializeField] private float siteProgressMeo = 0.05f;
    [SerializeField] private float siteProgressGeo = 0.01f;
    [SerializeField] private float siteCooldown = 3f;

    [Header("Properties")]
    [SerializeField] private ObjectiveStateEnum _objectiveState;
    [SerializeField] private float currentCooldown;
    [SerializeField] private float povProgress; //0-1 für 0-100%
    [SerializeField] private float commUpTime; //0-1 für 0-100%

    public float CommunicationUptime
    {
        get
        {
            return commUpTime;
        }
        set
        {
            Debug.LogError("Dont set CommunicationUptime!!!"); 
        }
    }

    public ObjectiveStateEnum ObjectiveState
    {
        get => _objectiveState;
        set
        {
            if (_objectiveState != value)
            {
                _objectiveState = value;
                objectiveStateChanged.Invoke(_objectiveState);
                print("POI: " + _objectiveState);
            }
        }
    }
    public UnityEvent<ObjectiveStateEnum> objectiveStateChanged;



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
        UpdateViz();
        _gameState = FindFirstObjectByType<GameState>();
        var hover = GetComponent<HoverDescription>();
        if (_objectiveState == ObjectiveStateEnum.Hidden)
        {
            hover.description = description + "\n";
        }
        else if (_objectiveState == ObjectiveStateEnum.Unexplored)
        {
            hover.description = "Our satellites have picked up something here. Send a satellite with a CAM to investigate.";
        }
        else if (_objectiveState == ObjectiveStateEnum.Explored || _objectiveState == ObjectiveStateEnum.Completed)
        {
            if(objectiveType == ObjectiveTypeEnum.Home)
                hover.description = displayName + "\n\n" + description;
            else if(objectiveType == ObjectiveTypeEnum.Colony)
                hover.description = displayName + "\n\n" + description + "\n\nThis is a colony. Establish communication using a COMM satellite.";
            else if(objectiveType == ObjectiveTypeEnum.AbandonedSite)
                hover.description = displayName + "\n\n" + description + "\n\nThis is an abandoned site. There is nothing more to do than document what happened.";
            else if(objectiveType == ObjectiveTypeEnum.MineralSurvey)
                hover.description = displayName + "\n\n" + description + "\n\nThis is a survey site. Equip a satellite with the Mineral sensor (MSE) to scan for resources.";
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateViz();
        if (Application.IsPlaying(gameObject))
        {
            // Play logic
            if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
            if (
                objectiveType == ObjectiveTypeEnum.Colony
                && commUpTime > 0
            )
            {
                commUpTime -= Time.deltaTime * colonyUptimeDecay;
            }
        }
        else
        {
            CalcPos();
        }
    }

    public void Payday(SatelliteInstance caller) 
    {
        if (currentCooldown > 0) { return; }
        if (ObjectiveState == ObjectiveStateEnum.Hidden)
        {
            DiscoverPoi(caller);
        }
        else if (ObjectiveState == ObjectiveStateEnum.Unexplored)
        {
            ExplorePoi(caller);
        } 
        else if (ObjectiveState == ObjectiveStateEnum.Explored)
        {
            ExcavatePoi(caller);
        } 
    }

    private void DiscoverPoi(SatelliteInstance caller)
    {
        if (ObjectiveState != ObjectiveStateEnum.Hidden)
        {
            return;
        }
        currentCooldown = discoverCooldown;
        ObjectiveState = ObjectiveStateEnum.Unexplored;
    }
    
    private void ExplorePoi(SatelliteInstance caller)
    {
        if (ObjectiveState != ObjectiveStateEnum.Unexplored 
            && caller.satFunction == SatelliteInstance.SatFunctions.CAM)
        {
            return;
        }
        currentCooldown = exploreCooldown;
        ObjectiveState = ObjectiveStateEnum.Explored;
    }
    
    private void ExcavatePoi(SatelliteInstance caller)
    {
        if (ObjectiveState != ObjectiveStateEnum.Explored)
        {
            return;
        }
        if (objectiveType == ObjectiveTypeEnum.Colony)
        {
            //TODO check if Sat has Connection mit andere Colonie & Base
            //Get Count als Multi
            if (caller.satFunction != SatelliteInstance.SatFunctions.COMM)
            {
                currentCooldown = colonyCooldown;
                return;
            }
            
            commUpTime += colonyUptime;
            povProgress += colonyProgress;
            currentCooldown = colonyCooldown;
            
            if (povProgress >= 1.0f)
            {
                povProgress = 0f;
                _gameState.economy.Money += payout;
            }
        }
        else if(objectiveType == ObjectiveTypeEnum.MineralSurvey)
        {
            if (caller.satFunction != SatelliteInstance.SatFunctions.SCAN)
            {
                currentCooldown = surveyCooldown;
                return;
            }
            
            float currentProgress = surveyProgressLeo;
            if (caller.orbit.orbitState == Orbit.OrbitState.MEO) currentProgress = surveyProgressMeo;
            if (caller.orbit.orbitState == Orbit.OrbitState.GEO) currentProgress = surveyProgressGeo;
            povProgress += currentProgress;
            currentCooldown = surveyCooldown;
            
            if (povProgress >= 1.0f)
            {
                povProgress = 0f;
                _gameState.economy.Money += payout;
                ObjectiveState = ObjectiveStateEnum.Completed;
            }
        }
        else if(objectiveType == ObjectiveTypeEnum.AbandonedSite)
        {
            if (caller.satFunction != SatelliteInstance.SatFunctions.CAM)
            {
                currentCooldown = siteCooldown;
                return;
            }
            
            float currentProgress = siteProgressLeo;
            if (caller.orbit.orbitState == Orbit.OrbitState.MEO) currentProgress = siteProgressMeo;
            if (caller.orbit.orbitState == Orbit.OrbitState.GEO) currentProgress = siteProgressGeo;
            povProgress += currentProgress;
            currentCooldown = siteCooldown;
            
            if (povProgress >= 1.0f)
            {
                povProgress = 0f;
                _gameState.economy.Money += payout;
                ObjectiveState = ObjectiveStateEnum.Completed;
            }
        }
        else
        {
            currentCooldown = exploreCooldown;
        }
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

            if (objectiveType == ObjectiveTypeEnum.Colony || objectiveType == ObjectiveTypeEnum.Home)
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
