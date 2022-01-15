using UnityEngine;

namespace PanteonGames
{
    public struct Tile
    {
        public bool isPlaceable;
        public bool IsWalkable() => !isPlaceable;
    }

    [System.Serializable] // for visualizing this struct in inspector
    public class GridVisualizer
    {
        public LineRenderer verticalLines;
        public LineRenderer horizontalLines;
        public void SetActive(bool activity)
        {
            verticalLines.enabled = activity;
            horizontalLines.enabled = activity;
        }

        public void SetColor(in Color color)
        {
            verticalLines  .startColor = color; verticalLines  .endColor = color;
            horizontalLines.startColor = color; horizontalLines.endColor = color;
        }

        internal void SetPosition(Vector3 position)
        {
            verticalLines.transform.position = position;
            horizontalLines.transform.position = position;
        }
    }
}
