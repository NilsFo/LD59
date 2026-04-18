using TMPro;
using UnityEngine;

namespace UI
{
    public class SatellitItemDisplayScript : MonoBehaviour
    {
        [SerializeField] [Header("My Satellite")]
        private SatelliteInstance satelliteInstance;

        [SerializeField] [Header("World Hookup")]
        private TextMeshProUGUI nameTF;

        [SerializeField] private TextMeshProUGUI fuelTF;

        private bool markedForDestroy = false;

        // Update is called once per frame
        void Update()
        {
            if (satelliteInstance)
            {
/*                nameTF.SetText(satelliteInstance.displayName);
                fuelTF.SetText("LEVEL " + satelliteInstance.fuelCurrent);
  */          }

            if (markedForDestroy) Destroy(gameObject);
        }

        public void SetSatelliteInstance(SatelliteInstance sat)
        {
            satelliteInstance = sat;
            sat.OnSatelliteDestroy += OnSatelliteDestroy;
        }

        public void OnSatelliteDestroy()
        {
            markedForDestroy = true;
        }
    }
}