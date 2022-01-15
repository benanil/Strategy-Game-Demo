

using UnityEngine;

namespace PanteonGames
{
    public class ProductionMenu : MonoBehaviour
    {
        [SerializeField] private BuildingDataSC[] allBuildings;
        [SerializeField] private ProductionSlot slotPrefab;
        [SerializeField] private Transform slotParent;

        private void Start()
        {
            // todo spawn all
            for (int i = 0; i < allBuildings.Length; i++)
            {
                Instantiate(slotPrefab, slotParent); 
            }
        }
    }
}