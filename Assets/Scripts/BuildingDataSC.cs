﻿using UnityEngine;

namespace PanteonGames
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "ScriptableObjects/BuildingData")]
    public class BuildingDataSC : ScriptableObject 
    {
        public string BuildingName;
        [Multiline(3)]
        public string Description;

        public Sprite sprite;
        public float healthPoint = 10;

        public Vector2Int CellSize = new Vector2Int(1, 1); // eg barracks are 4x4
    }
}
