using TMPro;
using UnityEngine;

namespace UI
{
    public class SatellitDisplayScript : MonoBehaviour
    {
        [SerializeField] private GameObject prefabUiItem;

        [SerializeField] private GameObject parentUI;
        public TMP_Text costTF;

        public void RegisterSatellite(SatelliteInstance sat)
        {
            GameObject newItem = Instantiate(prefabUiItem, parentUI.transform);
            SatellitItemDisplayScript comp = newItem.GetComponent<SatellitItemDisplayScript>();
            comp.SetSatelliteInstance(sat,costTF);
        }
    }
}