// this class will used for:
// -managing input 
// -placing buildings

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PanteonGames
{
    public enum GameManagerState : int
    {
        none = 0, MovingBuilding = 1, movingSoldiers = 2
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public float MovingBuildingDepth = 500;

        public TileSystem TileSystem;
        private GameManagerState State;

        private Building CurrentBuilding;

        public Material PrewiewLineMaterial;
        public LineRenderer FindedPathLineRenderer;
        public LineRenderer SelectAreaLines; // 

        public Transform BuildingsParent;

        private Transform SoldiersParent;

        private List<Soldier> soldiers = new List<Soldier>();

        private List<Soldier> SelectedSoldiers = new List<Soldier>();

        private List<Action> StateActions = new List<Action>();

        private float draggingTime = 0;
        private bool dragStarted;

        Vector3 dragStartPos;

        public static Building GetCurrentBuilding() => instance.CurrentBuilding;
        public static GameManagerState GetCurrentState() => instance.State;
        public static void MoveBuilding(Building building) => instance.MoveBuildingInternal(building);
        public static void SetCurrentBuilding(Building building) => instance.CurrentBuilding = building;
        public static void ProduceSoldier(SoldierSO soldierSO) => instance.ProduceSoldierInternal(soldierSO);
        public void SetSellectedSoldiers(List<Soldier> soldiers) { SelectedSoldiers = soldiers; }

        private void Awake()
        {
            if (TileSystem == null) { Debug.LogError("please asign tile system!"); }

            Application.targetFrameRate = 60;

            // order is important
            StateActions.Add(NoneState);
            StateActions.Add(MovingBuildingState);
            StateActions.Add(MovingSoldierState);

            instance = this;
            BuildingsParent = new GameObject("Buildings Parent").transform;
            SoldiersParent = new GameObject("Soldiers Parent").transform;

            SelectAreaLines.positionCount = 5;
        }

        private void Update()
        {
            StateActions[(int)State].Invoke();
        }

        // STATE MACHINE STATES
        private void NoneState()
        {
            if (Input.GetMouseButton(0) && !CameraMovement.IsUIHovering())
            {
                draggingTime += Time.deltaTime;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            if (draggingTime > 0.25f)
            {
                // begin dragging
                if (!dragStarted)
                {
                    SelectAreaLines.gameObject.SetActive(true);
                    dragStartPos = mousePos;
                    dragStarted = true;
                }

                // draw selection line 
                SelectAreaLines.SetPosition(0, dragStartPos);
                SelectAreaLines.SetPosition(1, new Vector3(mousePos.x, dragStartPos.y, 0));
                SelectAreaLines.SetPosition(2, new Vector3(mousePos.x, mousePos.y, 0));
                SelectAreaLines.SetPosition(3, new Vector3(dragStartPos.x, mousePos.y, 0));
                SelectAreaLines.SetPosition(4, dragStartPos);
            }

            // if mouse up end dragging
            if (dragStarted && Input.GetMouseButtonUp(0))
            {
                if (SoldiersSelected(new MinMaxSelectArea(dragStartPos, mousePos)))
                {
                    State = GameManagerState.movingSoldiers;
                }
                SelectAreaLines.gameObject.SetActive(false);
                dragStarted = false;
                draggingTime = 0;
            }
        }

        private void MovingBuildingState()
        {
            if (CurrentBuilding == null) return;
            Vector2Int buildingSize = CurrentBuilding.data.CellSize;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = -MovingBuildingDepth;

            // we are manipulating mouse position here because we don't want to hold building at pivot position
            mousePos -= TileSystem.GetBuildingCenterDrection(buildingSize);
            // sample grid position
            Vector3 gridPosition = TileSystem.WorldPositionToGridPosition(mousePos, buildingSize - Vector2Int.one);
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
                // else todo: add sound and particle place is not empty
            }
        }

        private void MovingSoldierState()
        {
            if (SelectedSoldiers.Count == 0 || CameraMovement.IsUIHovering()) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = -MovingBuildingDepth / 2;

            // right click move soldier
            if (Input.GetMouseButtonDown(1))
            {
                Vector2Int mouseIndex = TileSystem.WorldPositionToGridIndex(mousePos, Vector2Int.one);
                if (SelectedSoldiers.Count == 1)
                {
                    SelectedSoldiers[0].MoveToPoint(mouseIndex);
                }
                else
                {
                    Vector3 averagePosition = default;
                    // find center position of all soldiers (average pos)
                    for (int i = 0; i < SelectedSoldiers.Count; ++i)
                    {
                        averagePosition += SelectedSoldiers[i].transform.position;
                    }
                    averagePosition /= SelectedSoldiers.Count;
                    Vector2Int averageGridIndex = TileSystem.WorldPositionToGridIndex(averagePosition, Vector2Int.one);

                    for (int i = 0; i < SelectedSoldiers.Count; i++)
                    {
                        Vector2Int direction = mouseIndex - averageGridIndex;
                        Vector2Int targetIndex = averageGridIndex + direction + (averageGridIndex - SelectedSoldiers[i].GetGridIndex());
                        SelectedSoldiers[i].MoveToPoint(targetIndex);
                    }
                }
                State = GameManagerState.none;
            }
        }

        // ABOUT SOLIER
        public void SetSellectedSoldier(Soldier soldier)
        {
            SelectedSoldiers.Clear();
            SelectedSoldiers.Add(soldier);
        }

        public void ProduceSoldierInternal(SoldierSO soldierSO)
        {
            if (CurrentBuilding == null) return;
            List<Vector2Int> placeableEdges = CurrentBuilding.GetPlaceableEdges();

            bool placed = false;

            for (int i = 0; i < placeableEdges.Count; i++)
            {
                Vector2Int arrayIndex = placeableEdges[i];

                bool AlreadyHasSoldier = soldiers.Any(soldier =>
                {
                    return soldier.arrayIndex == arrayIndex;
                });

                if (!AlreadyHasSoldier && TileSystem.instance.IsWalkable(placeableEdges[i]))
                {
                    // place soldier
                    var soldierGO = new GameObject(soldierSO.name);
                    soldierGO.AddComponent<SpriteRenderer>();
                    soldierGO.transform.SetParent(SoldiersParent);
                    Soldier soldier = soldierGO.AddComponent<Soldier>();
                    soldier.Initialize(soldierSO, this, arrayIndex, TileSystem);
                    placed = true;
                    soldiers.Add(soldier);
                    break;
                }
            }

            if (!placed)
            {
                Debug.Log("soldier maximum capacity!");
            }
        }

        private void MoveBuildingInternal(Building building)
        {
            CurrentBuilding = building;
            TileSystem.SetPreviewSize(building.data.CellSize);
            TileSystem.SetPreviewEnable(true);
            building.transform.SetParent(BuildingsParent);
            SetState(GameManagerState.MovingBuilding);
        }

        private bool SoldiersSelected(in MinMaxSelectArea area)
        {
            SelectedSoldiers.Clear();
            foreach (var soldier in soldiers)
            {
                if (area.IsInside(soldier.transform.position))
                {
                    SelectedSoldiers.Add(soldier);
                }
            }
            return SelectedSoldiers.Count > 0;
        }

        public void ShowFindedPath(Vector3[] points)
        {
            FindedPathLineRenderer.positionCount = points.Length;
            FindedPathLineRenderer.SetPositions(points);
        }

        public void SetState(GameManagerState state)
        {
            this.State = state;

            TileSystem.SetPreviewEnable(State == GameManagerState.MovingBuilding);
            if (CurrentBuilding)
            TileSystem.SetPreviewSize(CurrentBuilding.data.CellSize);
            TileSystem.SetMainGridEnable(State == GameManagerState.MovingBuilding);
        }
        
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            var tiles = TileSystem.GetTiles();

            Gizmos.color = Color.red;
                
            for (int x = 0; x < tiles.GetLength(0); ++x)
            {
                for (int y = 0; y < tiles.GetLength(1); ++y)
                {
                    if (!tiles[x,y].IsWalkable())
                    {
                        Gizmos.DrawWireSphere(new Vector3(x * 0.32f, y * 0.32f, 0), 0.05f);
                    }
                }
            }
        }
    }
}
