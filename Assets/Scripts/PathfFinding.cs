
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PanteonGames
{
    public static class PathfFinding
    {
        private static readonly int invaildParentNode = 1;
        
        public static List<Vector2Int> FindPath(Vector2Int startIndex, Vector2Int endIndex)
        {
            return FindPath(TileSystem.instance.GetTiles(), startIndex, endIndex);   
        }

        public static List<Vector2Int> FindPath(Tile[,] tile, Vector2Int startIndex, Vector2Int endIndex)
        {
            Vector2Int   TileSize   = new Vector2Int(tile.GetLength(0), tile.GetLength(1));
            TilePoint[,] tilePoints = new TilePoint[TileSize.x, TileSize.y];

            for (int x = 0; x < TileSize.x; ++x)
            {
                for (int y = 0; y < TileSize.y; ++y)
                {
                    tilePoints[x, y].hCost = Vector2Int.Distance(new Vector2Int(x, y), endIndex);
                    tilePoints[x, y].parentTileIndex.x = invaildParentNode;
                    tilePoints[x, y].index.x = x;
                    tilePoints[x, y].index.y = y;
                    tilePoints[x, y].gCost = int.MaxValue;
                }
            }

            List<Vector2Int> openIndexes  = new List<Vector2Int>(TileSize.x * TileSize.y);
            List<Vector2Int> closeIndexes = new List<Vector2Int>(TileSize.x * TileSize.y);

            tilePoints[startIndex.x, startIndex.y].gCost = 0;

            openIndexes.Add(startIndex);

            while (openIndexes.Count > 0)
            {
                Vector2Int currentNodeIndex = GetLowestFCostIndex(openIndexes, tilePoints);
                TilePoint currTilePoint = tilePoints[currentNodeIndex.x, currentNodeIndex.y];

                if (currentNodeIndex == endIndex) {
                    // we found the end position
                    break;
                }

                openIndexes.Remove(currentNodeIndex);
                closeIndexes.Add(currentNodeIndex);

                foreach (var neighborIndex in TileSystem.instance.GetNeighbors(currTilePoint.index))
                {
                    if (!IsPositionInsideGrid(TileSize, neighborIndex)) continue;

                    TilePoint neighborTile = tilePoints[neighborIndex.x, neighborIndex.y];

                    if (closeIndexes.Contains(neighborIndex)) continue;

                    if (!tile[neighborIndex.x, neighborIndex.y].IsWalkable()) continue;

                    float tentativeGCost = currTilePoint.gCost + Vector2Int.Distance(currentNodeIndex, neighborIndex);

                    if (tentativeGCost < neighborTile.gCost)
                    {
                        tilePoints[neighborIndex.x, neighborIndex.y].parentTileIndex = currentNodeIndex;
                        tilePoints[neighborIndex.x, neighborIndex.y].gCost = tentativeGCost;

                        if (!openIndexes.Contains(neighborIndex))
                        {
                            openIndexes.Add(neighborIndex);
                        }
                    }
                } 
            }
            
            TilePoint endTilePoint = tilePoints[endIndex.x, endIndex.y];

            return CalculateResultPath(tilePoints, endTilePoint);    
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FastDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x + a.y - b.y);
        }

        private static List<Vector2Int> CalculateResultPath(TilePoint[,] tilePoints, TilePoint endTilePoint)
        {
            if (endTilePoint.parentTileIndex.x == invaildParentNode) {
                Debug.LogError("path is not finded");
            }

            List<Vector2Int> result = new List<Vector2Int>();

            TilePoint currentTilePoint = endTilePoint;
            while (currentTilePoint.parentTileIndex.x != invaildParentNode)
            {
                TilePoint parentTile = tilePoints[currentTilePoint.parentTileIndex.x, currentTilePoint.parentTileIndex.y];
                result.Add(parentTile.index);
                currentTilePoint = parentTile;
            }

            result.Reverse();

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPositionInsideGrid(Vector2Int tileSize, Vector2Int index)
        {
            if (index.x < 0 || index.y < 0) return false;
            return index.x <= tileSize.x && index.y <= tileSize.y;
        }

        private static Vector2Int GetLowestFCostIndex(List<Vector2Int> openIndexes, TilePoint[,] tilePoints)
        {
            Vector2Int lowestIndex = openIndexes[0];

            for (int i = 1; i < openIndexes.Count; ++i)
            {
                if (tilePoints[openIndexes[i].x, openIndexes[i].y].GetFCost() 
                    < tilePoints[lowestIndex.x, lowestIndex.y].GetFCost())
                {
                    lowestIndex = openIndexes[i];
                }
            }
            return lowestIndex;
        }

        private struct TilePoint
        {
            public float gCost; // distance to end position
            public float hCost; // heuristic cost to end Position

            public Vector2Int index;

            public Vector2Int parentTileIndex;

            public float GetFCost()
            {
                return gCost + hCost;
            }
        }
    }
}
