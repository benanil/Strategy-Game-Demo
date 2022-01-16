using System;
using System.Collections.Generic;
using UnityEngine;

namespace PanteonGames
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Soldier : MonoBehaviour
    {
        private enum SoldierState { idle = 0, walking = 1 }

        private const float CellPixelScale = 32;

        private SoldierSO soData;
        private float HealthPoint;
        private TileSystem TileSystem;
        private GameManager GameManager;

        private SoldierState State;

        public Vector2Int arrayIndex { get; private set; }

        public void Initialize(SoldierSO soldierSO, GameManager _gameManager, Vector2Int _arrayIndex, TileSystem _tileSystem)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            TileSystem = _tileSystem;
            arrayIndex = _arrayIndex;
            GameManager = _gameManager;

            // align position to grid
            transform.position = TileSystem.ArrayIndexToWorldPosition(arrayIndex);
            spriteRenderer.sprite = soldierSO.sprite;
            soData = soldierSO;
            soData.HealthPoint = HealthPoint;

            Vector2 spriteSize = soData.sprite.rect.size;
            Vector2 desiredScale = Vector2.one * CellPixelScale; // ie: desiredScale = 4x4 * 32 = 256x256
            Vector2 cellAspectRatio = desiredScale / spriteSize;

            transform.localScale = Vector3.one * cellAspectRatio;

            gameObject.AddComponent<BoxCollider2D>();
        }

        private Vector3[] worldPositions;
        private int currentIndex;

        private void Update()
        {
            if (State == SoldierState.walking) 
            {
                transform.position = Vector3.MoveTowards(transform.position, worldPositions[currentIndex], Time.deltaTime * soData.Speed);
                // todo follow path
                if (Vector3.Distance(transform.position, worldPositions[currentIndex]) < 0.05f)
                {
                    ++currentIndex;
                    if (currentIndex == worldPositions.Length)
                    {
                        Array.Clear(worldPositions, 0, worldPositions.Length);
                        worldPositions = null;
                        State = SoldierState.idle;
                    }
                }
            }
        }

        public void OnMouseDown()
        {
            GameManager.SetSellectedSoldier(this);
            GameManager.SetState(GameManagerState.movingSoldiers);
        }

        public void MoveToPoint(Vector2Int point)
        {
            State = SoldierState.walking;

            Vector2Int startGridPos = GetGridIndex();
            List<Vector2Int> pathIndexes = PathfFinding.FindPath(TileSystem.GetTiles(), startGridPos, point);

            worldPositions = new Vector3[pathIndexes.Count];
            worldPositions[0] = transform.position;

            for (int i = 1; i < pathIndexes.Count; i++)
            {
                worldPositions[i] = TileSystem.ArrayIndexToWorldPosition(pathIndexes[i]);
            }

            GameManager.instance.ShowFindedPath(worldPositions);

            currentIndex = 0;
        }

        public Vector2Int GetGridIndex()
        {
            return TileSystem.WorldPositionToGridIndex(transform.position, Vector2Int.one);
        }

        public void AddDamage(float value)
        {
            HealthPoint -= value;
            if (HealthPoint <= 0) Die();
        }

        private void Die()
        {
            Debug.Log("soldier died!");
        }

    }
}
