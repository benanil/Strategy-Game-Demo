using System.Runtime.CompilerServices;
using UnityEngine;

namespace PanteonGames
{
    public struct Tile
    {
        public bool isPlaceable;
        public bool IsWalkable() => isPlaceable;
    }

    public struct MinMaxSelectArea
    {
        public Vector2 minPoint, maxPoint;
        public MinMaxSelectArea(Vector2 _minPoint, Vector2 _maxPoint)
        {
            minPoint = new Vector2();
            minPoint.x = _minPoint.x < _maxPoint.x ? _minPoint.x : _maxPoint.x;
            minPoint.y = _minPoint.y < _maxPoint.y ? _minPoint.y : _maxPoint.y;

            maxPoint = new Vector2();
            maxPoint.x = _minPoint.x > _maxPoint.x ? _minPoint.x : _maxPoint.x;
            maxPoint.y = _minPoint.y > _maxPoint.y ? _minPoint.y : _maxPoint.y;
        }

        public bool IsInside(Vector3 point)
        {
            return point.x <= maxPoint.x && point.y <= maxPoint.y &&
                point.x >= minPoint.x && point.y >= minPoint.y ;
        }
    }

    public static class Helper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }

    [System.Serializable] // for visualizing this struct in inspector
    public class GridVisualizer
    {
        public LineRenderer verticalLines;
        public LineRenderer horizontalLines;
        public Material material;
        public void SetVisuality(bool activity)
        {
            verticalLines.enabled = activity;
            horizontalLines.enabled = activity;
        }

        public void SetColor(in Color color)
        {
            material.color = color;
        }

        internal void SetPosition(Vector3 position)
        {
            verticalLines.transform.position = position;
            horizontalLines.transform.position = position;
        }
    }
}
