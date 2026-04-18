using TMPro;
using UnityEngine;

namespace UI
{
    public class SatellitItemDisplayScript : MonoBehaviour
    {

        [SerializeField]
        private SatelliteInstance satelliteInstance;
        [SerializeField]
        private TextMeshProUGUI displayName;
        [SerializeField]
        private TextMeshProUGUI fule;
        
        private bool markedForDestroy = false;

        // Update is called once per frame
        void Update()
        {
            if(satelliteInstance) fule.SetText("LEVEL "+ satelliteInstance.fuelCurrent);
            if(markedForDestroy) Destroy(gameObject);
        }

        public void SetSatelliteInstance(SatelliteInstance sat)
        {
            satelliteInstance = sat;
            displayName.SetText(satelliteInstance.displayName);
            sat.OnSatelliteDestroy += OnSatelliteDestroy;
        }

        public void OnSatelliteDestroy()
        {
            markedForDestroy = true;
        }
    }
}
