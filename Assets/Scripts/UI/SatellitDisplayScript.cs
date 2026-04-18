using UnityEngine;

namespace UI
{
    public class SatellitDisplayScript : MonoBehaviour
    {
        [SerializeField]
        private GameObject prefabUiItem;
    
        [SerializeField]
        private GameObject parentUI;

        public void AddSatellit(SatelliteInstance sat)
        {
            GameObject newItem = Instantiate(prefabUiItem, parentUI.transform);
            SatellitItemDisplayScript comp = newItem.GetComponent<SatellitItemDisplayScript>();
            comp.SetSatelliteInstance(sat);
        }
    }
}
