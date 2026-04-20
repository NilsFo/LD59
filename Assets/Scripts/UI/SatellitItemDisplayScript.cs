using TMPro;
using UnityEngine;

namespace UI
{
    public class SatellitItemDisplayScript : MonoBehaviour
    {
        [SerializeField] [Header("My Satellite")]
        public SatelliteInstance satelliteInstance;
        public TMP_Text costTF;

        [Header("World Hookup")] public TextMeshProUGUI nameTF;
        public TextMeshProUGUI fuelTF;

        private bool markedForDestroy = false;

        // Update is called once per frame
        void Update()
        {
            if (satelliteInstance)
            {
                nameTF.SetText(satelliteInstance.displayName);
                fuelTF.SetText("Fuel: " + satelliteInstance.fuelCurrent + "/" + satelliteInstance.fuelMax);

                nameTF.color = Color.white;
                if (satelliteInstance.IsHighLighted)
                {
                    nameTF.color = satelliteInstance.colorMuted;
                }

                if (satelliteInstance.IsSelected)
                {
                    nameTF.color = satelliteInstance.color;
                }
            }

            if (markedForDestroy) Destroy(gameObject);
        }


        public void SetSatelliteInstance(SatelliteInstance sat, TMP_Text tf)
        {
            satelliteInstance = sat;
            costTF = tf;
            sat.OnSatelliteDestroy += OnSatelliteDestroy;
        }

        public void OnSatelliteDestroy()
        {
            markedForDestroy = true;
        }
    }
}