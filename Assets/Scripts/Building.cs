
using UnityEngine;
using NaughtyAttributes;

namespace PanteonGames
{
    // Building Base Class
    public class Building : MonoBehaviour
    {
        private const float CellPixelScale = 32;
        
        [Expandable] // only asset for visualizing scriptable object on inspector(editor only attribute)
        public BuildingDataSO data;

        public float HealthPoint = 10;

        public void AddDamage(float damage)
        {
            HealthPoint -= damage;

            if (HealthPoint <= 0)
            {
                Explode();
            }
        }

        public void Explode()
        {
            // todo add sound and particle
            Destroy(this);
        }

        // we are rescaling the gameobject here because our sprites are not exact size that we want
        // eg. I have 236x236 image I want to round it to 256x256(because every cell is 32pixel)
        // so I scale it up here
        public void Initialize(BuildingDataSO _data)
        {
            data = _data;
            HealthPoint = data.healthPoint;
            GetComponent<SpriteRenderer>().sprite = data.sprite;
            Vector2 spriteSize = data.sprite.rect.size;
            Vector2 desiredScale = data.CellSize * (int)CellPixelScale; // ie: desiredScale = 4x4 * 32 = 256x256
            Vector2 cellAspectRatio = desiredScale / spriteSize;

            transform.localScale = Vector3.one * cellAspectRatio;
        }
    }
}
