

using UnityEngine;

namespace PanteonGames
{
    public class ProductionMenu : MonoBehaviour
    {
        [SerializeField] private BuildingDataSO[] allBuildings;
        [SerializeField] private BuildingSlot slotPrefab;
        [SerializeField] private Transform slotParent;

        private void Start()
        {
            // todo spawn all
            for (int i = 0; i < allBuildings.Length; i++)
            {
                BuildingSlot slot = Instantiate(slotPrefab, slotParent);
                slot.Initialize(allBuildings[i]);
            }
        }
    }
}