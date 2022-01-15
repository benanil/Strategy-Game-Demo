using System;
using UnityEngine;

namespace PanteonGames
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "ScriptableObjects/BuildingData")]
    public class BuildingDataSC : ScriptableObject 
    {
        public string Name;
        [Multiline(3)]
        public string Description;

        public Sprite sprite;

        public Vector2Int CellSize = new Vector2Int(1, 1); // eg barracks are 4x4
    }
}
