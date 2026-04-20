using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class Objective : MonoBehaviour
{
    public enum ObjectiveTypeEnum
    {
        AbandonedSite,
        MineralSurvey,
        Colony,
        Home
    }

    public enum ObjectiveStateEnum
    {
        Hidden,
        Unexplored,
        Explored,
        Completed
    }

    [Header("Hookup")] public MiniMapRepresented miniMapRepresented;

    [Header("Params")] [Header("General")] public string id;
    public string displayName;
    [TextArea(5, 15)] public string description;

    public ObjectiveTypeEnum objectiveType;
    public float longitude, latitude;


    [SerializeField] private int payout = 100;
    [SerializeField] private float discoverCooldown = 3f;
    [SerializeField] private float exploreCooldown = 3f;
    [SerializeField] private float excavateCooldown = 3f;
    [SerializeField] private float completedCooldown = 15f;

    [Header("Colony")] [SerializeField] private float colonyUptimeDecay = 0.2f; //Decay of Uptime
    [SerializeField] private float colonyUptime = 2.0f; //Amount of Uptime
    [SerializeField] private float colonyProgress = 0.25f; //Amount of Progress
    [SerializeField] private float colonyCooldown = 3f; //Delay between Progress

    [Header("MineralSurvey")] [SerializeField]
    private float surveyProgressLeo = 0.10f;

    [SerializeField] private float surveyProgressMeo = 0.05f;
    [SerializeField] private float surveyProgressGeo = 0.01f;
    [SerializeField] private float surveyCooldown = 3f;

    [Header("AbandonedSite")] [SerializeField]
    private float siteProgressLeo = 0.10f;

    [SerializeField] private float siteProgressMeo = 0.05f;
    [SerializeField] private float siteProgressGeo = 0.01f;
    [SerializeField] private float siteCooldown = 3f;

    [Header("Properties")] [SerializeField]
    private ObjectiveStateEnum _objectiveState;

    [SerializeField] private float currentCooldown;
    [SerializeField] private float povProgress; //0-1 für 0-100%
    [SerializeField] private float commUpTime; //0-1 für 0-100%

    public float CommunicationUptime
    {
        get { return commUpTime; }
        set { Debug.LogError("Dont set CommunicationUptime!!!"); }
    }

    public float CommunicationUptimeProzent
    {
        get
        {
            float currentUptime = commUpTime;
            currentUptime /= _gameState.winUptime;
            if (currentUptime > 1.0f) return 1.0f;
            return currentUptime;
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
                print(displayName + " is now " + _objectiveState);
            }
        }
    }

    public UnityEvent<ObjectiveStateEnum> objectiveStateChanged;


    private GameState _gameState;

    [Header("Visuals")] public Transform unexploredViz;
    public Transform abandonedSiteViz;
    public Transform mineralSurveyViz;
    public Transform colonyViz;
    private bool _paydayAvailable;

    [field: SerializeField]
    public bool PaydayAvailable
    {
        get => _paydayAvailable;
        set
        {
            if (!_paydayAvailable && value)
            {
                _gameState.musicManager.CreateAudioClip(audioMined, Vector3.zero);
            }
            _paydayAvailable = value;
        }
    }

    [Header("Audio")] public AudioClip audioMined;
    public AudioClip audioCashout, audioDiscover;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CalcPos();
        UpdateViz();
        _gameState = FindFirstObjectByType<GameState>();
        UpdateDescription();
        objectiveStateChanged.AddListener(_ => UpdateDescription());

        if (miniMapRepresented != null)
        {
            if (ObjectiveState == ObjectiveStateEnum.Unexplored)
            {
                miniMapRepresented.Unexplored();
            }
            else if (ObjectiveState == ObjectiveStateEnum.Explored || ObjectiveState == ObjectiveStateEnum.Completed)
            {
                miniMapRepresented.Explored();
            }
        }

        if (ObjectiveState == ObjectiveStateEnum.Completed) currentCooldown = completedCooldown; //Base Income
    }


    void UpdateDescription()
    {
        var hover = GetComponent<HoverDescription>();

        if (_objectiveState == ObjectiveStateEnum.Hidden)
        {
            hover.description =
                "Our satellites have picked up something here. Send a satellite with a CAM to investigate.";
        }
        else if (_objectiveState == ObjectiveStateEnum.Unexplored)
        {
            hover.description =
                "Our satellites have picked up something here. Send a satellite with a CAM to investigate.";
        }
        else if (_objectiveState == ObjectiveStateEnum.Explored || _objectiveState == ObjectiveStateEnum.Completed)
        {
            if (objectiveType == ObjectiveTypeEnum.Home)
                hover.description = displayName + "\n\n" + description;
            else if (objectiveType == ObjectiveTypeEnum.Colony)
                hover.description = displayName + "\n\n" + description +
                                    "\n\nThis is a colony. Establish communication using a COMM satellite.";
            else if (objectiveType == ObjectiveTypeEnum.AbandonedSite)
                hover.description = displayName + "\n\n" + description +
                                    "\n\nThis is an abandoned site. There is nothing more to do than document what happened.";
            else if (objectiveType == ObjectiveTypeEnum.MineralSurvey)
                hover.description = displayName + "\n\n" + description +
                                    "\n\nThis is a survey site. Equip a satellite with the Mineral sensor (MSE) to scan for resources.";
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
            if (ObjectiveState == ObjectiveStateEnum.Completed && currentCooldown <= 0) //Base Income
            {
                PaydayAvailable = true;
                paydayAvailableViz.gameObject.SetActive(true);
                currentCooldown = completedCooldown;
            }

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
        if (currentCooldown > 0)
        {
            return;
        }

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
        miniMapRepresented.Unexplored();
        SpawnRevealed();
    }

    private void ExplorePoi(SatelliteInstance caller)
    {
        if (ObjectiveState != ObjectiveStateEnum.Unexplored
            || caller.SatFunction != SatelliteInstance.SatFunctions.CAM)
        {
            return;
        }

        //Debug.Log(displayName + " WAS FOUND BY "+caller.displayName+" WITH " + caller.satFunction );
        currentCooldown = exploreCooldown;
        ObjectiveState = ObjectiveStateEnum.Explored;
        miniMapRepresented.Explored();
        SpawnDiscoverd();
    }

    public bool ComDepthSearch(out List<SatelliteInstance> satCon)
    {
        var sats = _gameState.listOfSatellites;
        Queue<int> openSet = new Queue<int>();
        int initSat = -1, finalSat = -1;
        for (var index = 0; index < sats.Count; index++)
        {
            var sat = sats[index];
            if (sat.IsObjectiveInSight(this))
            {
                openSet.Enqueue(index);
                initSat = index;
            }
        }

        HashSet<int> closedSet = new HashSet<int>();
        NativeHashMap<int, int> satcons = new NativeHashMap<int, int>();
        bool hasCon = false;
        while (openSet.Count > 0)
        {
            var curIndex = openSet.Dequeue();
            var sat = sats[curIndex];
            closedSet.Add(curIndex);

            foreach (var othersat in sat.comSatsInSight)
            {
                if (othersat.IsObjectiveInSight(_gameState.home))
                {
                    Debug.Log(displayName + " can phone home");
                    hasCon = true;
                    openSet.Clear();
                    finalSat = curIndex;
                    break;
                }

                int othersatindex = sats.IndexOf(othersat);
                if (closedSet.Contains(othersatindex))
                    continue;
                openSet.Enqueue(othersatindex);
                satcons.Add(othersatindex, curIndex);
            }
        }
        satCon = new List<SatelliteInstance>();
        if (hasCon)
        {
            int curSat = finalSat;
            while (curSat != initSat)
            {
                satCon.Add(sats[curSat]);
                curSat = satcons[curSat];
            }
        }

        return hasCon;
    }

    private void ExcavatePoi(SatelliteInstance caller)
    {
        if (ObjectiveState != ObjectiveStateEnum.Explored)
        {
            return;
        }

        if (objectiveType == ObjectiveTypeEnum.Colony)
        {
            //Get Count als Multi
            if (caller.SatFunction != SatelliteInstance.SatFunctions.COMM)
            {
                currentCooldown = colonyCooldown;
                return;
            }

            if (!ComDepthSearch(out List<SatelliteInstance> satConPath))
            {
                currentCooldown = colonyCooldown;
                return;
            }

            commUpTime += colonyUptime;

            povProgress += colonyProgress;
            currentCooldown = colonyCooldown;

            if (povProgress >= 1.0f)
            {
                PaydayAvailable = true;
                List<GameObject> dataPath = new List<GameObject>();
                dataPath.Add(this.gameObject);
                dataPath.AddRange(satConPath.Select(t=>t.gameObject));
                dataPath.Add(_gameState.home.gameObject);
                SpawnDataPackage(dataPath);
                paydayAvailableViz.gameObject.SetActive(true);
            }
            else
            {
                SpawnProgressText(colonyProgress);
            }
        }
        else if (objectiveType == ObjectiveTypeEnum.MineralSurvey)
        {
            if (caller.SatFunction != SatelliteInstance.SatFunctions.SCAN)
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
                PaydayAvailable = true;
                paydayAvailableViz.gameObject.SetActive(true);
            }
            else
            {
                SpawnProgressText(currentProgress);
            }
        }
        else if (objectiveType == ObjectiveTypeEnum.AbandonedSite)
        {
            if (caller.SatFunction != SatelliteInstance.SatFunctions.CAM)
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
                PaydayAvailable = true;
                paydayAvailableViz.gameObject.SetActive(true);
                //ObjectiveState = ObjectiveStateEnum.Completed; Can repeat AbandonedSite
            }
            else
            {
                SpawnProgressText(currentProgress);
            }
        }
        else
        {
            currentCooldown = exploreCooldown;
        }
    }

    public DataPackage dataPackageInstance;
    private void SpawnDataPackage(List<GameObject> dataPath)
    {
        var dataPackage = Instantiate(dataPackageInstance);
        dataPackage.dataPath = dataPath;
    }

    public Transform paydayAvailableViz;

    public void Abkassieren()
    {
        _gameState.economy.Money += payout;
        povProgress = 0f;

        SpawnPaydayText(payout);
        PaydayAvailable = false;
        paydayAvailableViz.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (PaydayAvailable)
        {
            Abkassieren();
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
        }
        else if (ObjectiveState == ObjectiveStateEnum.Unexplored && Application.IsPlaying(gameObject))
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
        return new Vector3(
            x: Mathf.Sin(Mathf.Deg2Rad * lat) * Mathf.Sin(Mathf.Deg2Rad * lon),
            y: Mathf.Cos(Mathf.Deg2Rad * lat),
            z: Mathf.Sin(Mathf.Deg2Rad * lat) * Mathf.Cos(Mathf.Deg2Rad * lon));
    }

    public static Vector2 Vec3ToLongLat(Vector3 vec)
    {
        // From https://gist.github.com/nicoptere/2f2571db4b454bb18cd9
        vec.Normalize();

        //longitude = angle of the vector around the Y axis
        //-( ) : negate to flip the longitude (3d space specific )
        //- PI / 2 to face the Z axis
        var lng = -(Mathf.Atan2(-vec.z, -vec.x)) - Mathf.PI / 2;

        //to bind between 0 / PI*2
        if (lng < 0) lng += Mathf.PI * 2;

        //latitude : angle between the vector & the vector projected on the XZ plane on a unit sphere

        //project on the XZ plane
        var p = new Vector3(vec.x, 0, vec.z);
        //project on the unit sphere
        p.Normalize();

        //compute the angle ( both vectors are normalized, no division by the sum of lengths )
        var lat = Mathf.Acos(Vector3.Dot(p, vec));

        //invert if Y is negative to ensure teh latitude is comprised between -PI/2 & PI / 2
        if (vec.y < 0) lat *= -1;

        return new Vector2(lng, lat) * Mathf.Rad2Deg;
    }

    public void SpawnProgressText(float percent)
    {
        _gameState.ShowFloatingText(transform.position, percent + " %", Color.white);
    }

    public void SpawnPaydayText(float amount)
    {
        _gameState.musicManager.CreateAudioClip(audioCashout, Vector3.zero);
        _gameState.ShowFloatingText(transform.position,  "+" + amount + "$", Color.green);
    }

    public void SpawnDiscoverd() 
    {
        _gameState.musicManager.CreateAudioClip(audioDiscover, Vector3.zero);
        _gameState.ShowFloatingText(transform.position, displayName+" discoverd!", Color.red);
    }
    public void SpawnRevealed()
    {
        _gameState.musicManager.CreateAudioClip(audioDiscover, Vector3.zero);
        _gameState.ShowFloatingText(transform.position, "Site revealed!", Color.red);
    }
}