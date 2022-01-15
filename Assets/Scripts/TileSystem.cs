using System.Collections.Generic;
using UnityEngine;

namespace PanteonGames
{ 
    
    public class TileSystem : MonoBehaviour
    {
        public const float CellPixelScale = 32;

        public Vector3 TileStartPos;
        public Vector2Int TileScale;

        public LineRenderer GridVerticalLines;
        public LineRenderer GridHorizontalLines;

        public float cellScale = 1.0f;

        private void Start()
        {
    
        }
    
        [ContextMenu("Test")]
        public void Test() 
        {
            CreateGridLines();
        }

        private void Update()
        {
            
        }
    
        // self explanatory
        public void CreateGridLines()
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
            GridHorizontalLines.positionCount = horizontalPositions.Length;
            GridHorizontalLines.SetPositions(horizontalPositions);

            Vector3[] verticalPositions = new Vector3[TileScale.x * 3];

            // vertical lines
            for (int x = 0, i = 0; x < TileScale.x; ++x, i += 3) 
            {
                verticalPositions[i + 0] = TileStartPos + new Vector3(x * cellScale            ,    0);
                verticalPositions[i + 1] = TileStartPos + new Vector3(x * cellScale            , maxY);
                verticalPositions[i + 2] = TileStartPos + new Vector3(x * cellScale + cellScale, maxY);
            }

            // load positions to line renderer
            GridVerticalLines.positionCount = verticalPositions.Length;
            GridVerticalLines.SetPositions(verticalPositions);
        }


        private void OnDrawGizmos()
        {

        }
    }
}
