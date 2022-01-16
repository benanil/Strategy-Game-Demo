using UnityEngine;

namespace PanteonGames
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "ScriptableObjects/BuildingData")]
    public class BuildingDataSO : ScriptableObject 
    {
        public string BuildingName;
        [Multiline(3)]
        public string Description;

        public Sprite sprite;

        public ProdutionInfo[] productions;

        public float healthPoint = 10;

        public Vector2Int CellSize = new Vector2Int(1, 1); // eg barracks are 4x4
    
    }
    [System.Serializable]
    public struct ProdutionInfo
    {
        public Sprite sprite;
        public string name;
        public string description;
        // this can be scriptable object
        public Object initialData;
    }
}
