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
        public float MovingBuildingDepth = 500;

        public TileSystem TileSystem;
        private GameManagerState State;

        private Building CurrentBuilding;

        public Material PrewiewLineMaterial;

        // TEMP
        public Transform BuildingsParent;
        public BuildingDataSO testBuildingData;

        public static Building GetCurrentBuilding() { return instance.CurrentBuilding; }

        private void Awake()
        {
            if (TileSystem == null) { Debug.LogError("please asign tile system!"); }
            
            Application.targetFrameRate = 60;

            instance = this;
            BuildingsParent = new GameObject("Buildings Parent").transform;
        }

        public static void SetCurrentBuilding(Building building)
        {
            instance.SetBuildingInternal(building);
        }

        private void SetBuildingInternal(Building building)
        { 
            CurrentBuilding = building;
            TileSystem.SetPreviewSize(building.data.CellSize);
            TileSystem.SetPreviewEnable(true);
            building.transform.SetParent(BuildingsParent);
            SetState(GameManagerState.MovingBuilding);
        }

        private void Update()
        {
            if (State == GameManagerState.MovingBuilding && CurrentBuilding != null)
            {
                Vector2Int buildingSize = CurrentBuilding.data.CellSize;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = -MovingBuildingDepth;

                // we are manipulating mouse position here because we don't want to hold building at pivot position
                mousePos -= TileSystem.GetBuildingCenterDrection(buildingSize);
                // sample grid position
                Vector3 gridPosition = TileSystem.WorldPositionToGridPosition(mousePos, buildingSize);
                TileSystem.SetPreviewPosition(gridPosition);

                CurrentBuilding.transform.position = gridPosition;

                Vector2Int arrayIndex = TileSystem.GridPositionToArrayIndex(gridPosition); // tile index
                bool isPlaceable = TileSystem.IsPlacable(arrayIndex, buildingSize);

                TileSystem.SetPreviewColor(isPlaceable ? Color.green : Color.red);

                if (Input.GetMouseButtonDown(0))
                {
                    if (isPlaceable)
                    {
                        TileSystem.PlaceBuilding(CurrentBuilding, arrayIndex);
                        State = GameManagerState.none;
                        // todo: sound and particle
                        SetState(GameManagerState.none);
                        CurrentBuilding = null; // this line must came after set state otherwise null referance exception
                    }
                    else
                    {
                        // todo: add sound and particle place is not empty
                    }
                }
            }
        }

        private void SetState(GameManagerState state)
        {
            this.State = state;

            TileSystem.SetPreviewEnable(State == GameManagerState.MovingBuilding);
            TileSystem.SetPreviewSize(CurrentBuilding.data.CellSize);
            TileSystem.SetMainGridEnable(State == GameManagerState.MovingBuilding);
            
            switch (state)
            {
                case GameManagerState.MovingBuilding: break;
                case GameManagerState.none:           break;
                default: throw new System.Exception("invaild gamemanager state!");
            }
        }

    }
}
