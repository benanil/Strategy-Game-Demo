// this class helps visualizing grids
// and we will manage tiles here

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PanteonGames
{ 
    public class TileSystem : MonoBehaviour
    {
        public static TileSystem instance;
        public const float CellPixelScale = 32;
        
        public Vector3 TileStartPos;
        public Vector2Int TileScale;

        // grid visualizer is simply 2 line renderer
        [SerializeField] private GridVisualizer MainGridVisualizer;
        // for visualizing placeable or not
        [SerializeField] private GridVisualizer PreviewGridVisualizer;

        public float cellScale = 1.0f;
        
        /// tile is struct that contains only isplacable bool
        private Tile[,] tiles;

        // about preview
        public void SetPreviewSize(Vector2Int scale) => CreateGridLines(scale, Vector3.zero, PreviewGridVisualizer);
        public void SetPreviewEnable(bool enabled) => PreviewGridVisualizer.SetVisuality(enabled);
        public void SetPreviewColor(Color color) => PreviewGridVisualizer.SetColor(color);
        public void SetPreviewPosition(Vector3 position) => PreviewGridVisualizer.SetPosition(position);

        public void SetMainGridEnable(bool enabled) => MainGridVisualizer.SetVisuality(enabled);

        private void Awake()
        {
            instance = this;
            tiles = new Tile[TileScale.x, TileScale.y];
            MainGridVisualizer.SetVisuality(false);
            // set all of the tiles placeable 
            for (int x = 0; x < tiles.GetLength(0); ++x)
                for (int y = 0; y < tiles.GetLength(1); ++y)
                    tiles[x, y].isPlaceable = true;
        }

        public Tile[,] GetTiles()
        {
            return tiles;
        }

        /// <param name="tileScale">width and height of cell ie 4x4</param>
        /// <returns></returns>
        public Vector3 WorldPositionToGridPosition(Vector3 worldPos, Vector2Int buildingArea)
        {
            float xMod = worldPos.x % cellScale;
            float yMod = worldPos.y % cellScale;

            worldPos.x -= xMod;
            worldPos.y -= yMod;

            // clamp result between minimum and maximum grid area
            worldPos.x = Mathf.Clamp(worldPos.x, 0, (TileScale.x - buildingArea.x) * cellScale);
            worldPos.y = Mathf.Clamp(worldPos.y, 0, (TileScale.y - buildingArea.y) * cellScale);

            return worldPos;
        }

        // if we have grid position (left bottom side of building) when we using mouse draging etc. sometimes we want to center pos of building
        /// <returns> direction to center of the building (notice not normalized!) </returns>
        public Vector3 GetBuildingCenterDrection(Vector2Int buildingSize)
        {
            return new Vector3(buildingSize.x * (cellScale * 0.5f), buildingSize.x * (cellScale * 0.5f), 0);
        }

        // returns integer position on the 2d grid
        public Vector2Int GridPositionToArrayIndex(Vector3 worldPos)
        {
            Vector3 worldToGrid = WorldPositionToGridPosition(worldPos, Vector2Int.zero); // we are assuming 1x1 area here
            float gridToInt = 1 / cellScale;
            worldToGrid.x *= gridToInt;
            worldToGrid.y *= gridToInt;

            Vector2Int result = new Vector2Int(Mathf.RoundToInt(worldToGrid.x), Mathf.RoundToInt(worldToGrid.y));

            return result;
        }

        public Vector3 ArrayIndexToWorldPosition(Vector2Int arrayIndex)
        {
            return new Vector3(arrayIndex.x * cellScale, arrayIndex.y * cellScale);
        }

        public Vector2Int WorldPositionToGridIndex(Vector3 worldPos, Vector2Int buildingArea)
        {
            Vector3 gridPosition = WorldPositionToGridPosition(worldPos, buildingArea - Vector2Int.one);
            return GridPositionToArrayIndex(gridPosition);
        }

        // visualize green if grid is placeable
        /// <returns> building can place or not </returns>
        public bool IsPlacable(Vector2Int arrayIndex, Vector2Int buildingSize)
        {
            // if any of the indexes are not placable return false
            for (int x = 0; x < buildingSize.x; ++x)
                for (int y = 0; y < buildingSize.y; ++y)
                    if (tiles[arrayIndex.x + x, arrayIndex.y + y].isPlaceable == false)
                        return false;
            
            return true;
        }

        // visualize green if grid is walkable
        /// <returns> soldier can run or not </returns>
        public bool IsWalkable(Vector2Int arrayIndex)
        {
            return tiles[arrayIndex.x, arrayIndex.y].IsWalkable();
        }

        public void SetWlakable(Vector2Int index, Vector2Int size, bool value)
        {
            for (int x = 0; x < size.x; ++x)
            {
                for (int y = 0; y < size.x; ++y)
                {
                    tiles[index.x + x, index.y + y].isPlaceable = value;
                }
            }
        }

        /// <summary> get 1x1 neighbors </summary>
        public IEnumerable<Vector2Int> GetNeighbors(Vector2Int index)
        {
            for (int x = -1; x < 2; ++x)
            {
                for (int y = -1; y < 2; ++y)
                {
                    if (x == 0 && y == 0) continue;
                    yield return index + new Vector2Int(x, y);    
                }
            }
        }

        // mark as not placeable in tile array
        public void PlaceBuilding(Building building, Vector2Int arrayIndex)
        {
            for (int x = 0; x < building.data.CellSize.x; ++x)
            {
                for (int y = 0; y < building.data.CellSize.y; ++y)
                {
                    tiles[arrayIndex.x + x, arrayIndex.y + y].isPlaceable = false;
                }
            }
        }

        // mark as placeable in tile array
        public void RemoveBuilding(Building building, Vector2Int arrayIndex)
        {
            for (int x = 0; x < building.data.CellSize.x; ++x)
            {
                for (int y = 0; y < building.data.CellSize.y; ++y)
                {
                    tiles[arrayIndex.x + x, arrayIndex.y + y].isPlaceable = true;
                }
            }
        }

        public void CreateMainGridLines()
        { 
            CreateGridLines(TileScale, TileStartPos, MainGridVisualizer);
        }

        // for visualizing grids we need two line renderer in order to create proper line grid
        public void CreateGridLines(Vector2Int TileScale, Vector3 TileStartPos, GridVisualizer gridVisualizer)
        {
            // this is stack allocated version but unity doesn't support
            // Span<Vector3> horizontalPositions = stackalloc Vector3[TileScale.y * 3];
            Vector3[] horizontalPositions = new Vector3[TileScale.y * 3];

            float maxX = TileStartPos.x + (TileScale.x * cellScale);
            float maxY = TileStartPos.x + (TileScale.y * cellScale);

            // horizontal lines
            for (int y = 0, i = 0; y < TileScale.y; ++y, i += 3)
            {
                horizontalPositions[i + 0] = TileStartPos + new Vector3(0   , y * cellScale            );
                horizontalPositions[i + 1] = TileStartPos + new Vector3(maxX, y * cellScale            );
                horizontalPositions[i + 2] = TileStartPos + new Vector3(maxX, y * cellScale + cellScale);
            }

            // load positions to line renderer
            gridVisualizer.horizontalLines.positionCount = horizontalPositions.Length;
            gridVisualizer.horizontalLines.SetPositions(horizontalPositions);

            Vector3[] verticalPositions = new Vector3[TileScale.x * 3];

            // vertical lines
            for (int x = 0, i = 0; x < TileScale.x; ++x, i += 3) 
            {
                verticalPositions[i + 0] = TileStartPos + new Vector3(x * cellScale            ,    0);
                verticalPositions[i + 1] = TileStartPos + new Vector3(x * cellScale            , maxY);
                verticalPositions[i + 2] = TileStartPos + new Vector3(x * cellScale + cellScale, maxY);
            }

            // load positions to line renderer
            gridVisualizer.verticalLines.positionCount = verticalPositions.Length;
            gridVisualizer.verticalLines.SetPositions(verticalPositions);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TileSystem))]
    public class TileSystemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Create Main Grid"))
            {
                ((TileSystem)target).CreateMainGridLines();
            }
        }
    }
#endif

}
