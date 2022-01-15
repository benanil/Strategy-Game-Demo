// this class will used for:
// -managing input 
// -placing buildings

using UnityEngine;

namespace PanteonGames
{
    enum GameManagerState : int
    { 
        none = 0, MovingBuilding = 1
    }
    
    public class GameManager : MonoBehaviour
    {
        static GameManager instance;
        private const float MovingBuildingDepth = 500;

        public TileSystem TileSystem;
        private GameManagerState State;

        private Building CurrentBuilding;

        // TEMP
        public Transform BuildingsParent;
        public BuildingDataSC testBuildingData;

        private void Start()
        {
            if (TileSystem == null) { Debug.LogError("please asign tile system!"); }

            instance = this;
            BuildingsParent = new GameObject("Buildings Parent").transform;
        }

        public static void SetCurrentBuilding(Building building)
        {
            instance.CurrentBuilding = building;
            instance.TileSystem.SetPreviewSize(building.data.CellSize);
            instance.TileSystem.SetPreviewEnable(true);
        }

        private void Update()
        {
            if (State == GameManagerState.MovingBuilding && CurrentBuilding != null)
            {
                Vector2Int buildingSize = CurrentBuilding.data.CellSize;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = MovingBuildingDepth;

                // we are manipulating mouse position here because we don't want to hold building at pivot position
                mousePos -= TileSystem.GetBuildingCenterDrection(buildingSize);
                // sample grid position
                Vector3 gridPosition = TileSystem.WorldPositionToGridPosition(mousePos, buildingSize);
                TileSystem.SetPreviewPosition(gridPosition);

                CurrentBuilding.transform.position = mousePos;

                Vector2Int arrayIndex = TileSystem.GridPositionToArrayIndex(gridPosition); // tile index
                bool isPlaceable = TileSystem.IsPlacable(arrayIndex, buildingSize);

                TileSystem.SetPreviewColor(isPlaceable ? Color.green : Color.red);

                if (Input.GetMouseButtonDown(0))
                {
                    if (isPlaceable)
                    {
                        TileSystem.PlaceBuilding(CurrentBuilding, arrayIndex);
                        CurrentBuilding = null;
                        State = GameManagerState.none;
                    }
                    else
                    {
                        // todo: add sound or particle place is not empty
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                State = State == GameManagerState.MovingBuilding?GameManagerState.none:GameManagerState.MovingBuilding;
                TileSystem.SetPreviewEnable(State == GameManagerState.MovingBuilding);
                TileSystem.SetMainGridEnable(State == GameManagerState.MovingBuilding);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                var newBuildingGO = new GameObject("building test");
                newBuildingGO.transform.SetParent(BuildingsParent);
                var building = newBuildingGO.AddComponent<Building>();
                building.Initialize(testBuildingData);
            }
        }
    }
}
