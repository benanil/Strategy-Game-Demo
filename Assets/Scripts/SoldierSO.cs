using UnityEngine;

namespace PanteonGames
{
    [CreateAssetMenu(fileName = "SoldierData", menuName = "ScriptableObjects/SoldierData")]
    public class SoldierSO : ScriptableObject
    {
        public float HealthPoint = 10;
        public float Damage = 10;
        public float Speed = 1.0f;
        public Sprite sprite;
    }

}
