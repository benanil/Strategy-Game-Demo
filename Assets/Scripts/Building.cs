
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

namespace PanteonGames
{
    // Building Base Class
    public class Building : MonoBehaviour
    {
        private const float CellPixelScale = 32;
        
        [Expandable] // only asset for visualizing scriptable object on inspector(editor only attribute)
        public BuildingDataSO data;

        public float HealthPoint = 10;

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

            gameObject.AddComponent<BoxCollider2D>();
            // var rig = gameObject.AddComponent<Rigidbody2D>();
            // rig.isKinematic = true;
        }

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
            var tileSystem = TileSystem.instance;
            tileSystem.RemoveBuilding(this, tileSystem.WorldPositionToGridIndex(transform.position, data.CellSize));
            // todo add sound and particle
            Destroy(this);
        }

        public void OnMouseUp()
        {
            GameManager.SetCurrentBuilding(this);
            InformationMenu.instance.SetCurrentBuildingInformation(data);
        }

        // gets the placeable corners of building
        public List<Vector2Int> GetPlaceableEdges()
        {
            List<Vector2Int> result = new List<Vector2Int>((data.CellSize.x * 2 + data.CellSize.y * 2) - 4);

            Vector2Int positionIndex = TileSystem.instance.WorldPositionToGridIndex(transform.position, data.CellSize);

            Vector2Int leftBottom = positionIndex - Vector2Int.one;
            Vector2Int RightUpper = positionIndex + data.CellSize;

            for (int x = 0; x <= data.CellSize.x; ++x) {
                result.Add(leftBottom + new Vector2Int(x, 0)); 
            }

            for (int y = 0; y <= data.CellSize.y; ++y) {
                result.Add(leftBottom + new Vector2Int(0, y));
            }

            for (int x = data.CellSize.x; x > 0; --x) {
                result.Add(RightUpper - new Vector2Int(x, 0));
            }

            for (int y = 0; y > data.CellSize.y; --y) {
                result.Add(RightUpper - new Vector2Int(0, y));
            }

            return result;
        }

        private void OnDrawGizmos()
        {
            for (int x = 0; x < data.CellSize.x; ++x)
            {
                for (int y = 0; y < data.CellSize.y; ++y)
                {
                    Gizmos.DrawWireSphere(transform.position + new Vector3(x * 0.32f, y * 0.32f, 0), 0.2f);           
                }
            }
        }

    }
}
