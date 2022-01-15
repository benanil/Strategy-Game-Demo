using System;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;

namespace PanteonGames
{
    // Building Base Class
    public class Building : MonoBehaviour
    {
        private const float CellPixelScale = 32;
        
        [Expandable] // only asset for visualizing scriptable object on inspector(editor only attribute)
        public BuildingDataSC data;

        public Vector2 cellAspectRatio;

        [ContextMenu("Test")]
        public void Test()
        {
            Initialize();
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            Vector2 spriteSize = data.sprite.rect.size;
            Vector2 desiredScale = data.CellSize * (int)CellPixelScale; // ie: desiredScale = 4x4 * 32 = 256x256
            cellAspectRatio = desiredScale / spriteSize;

            Debug.Log("cellAspectRatio" + cellAspectRatio);
            Debug.Log("desiredScale " + desiredScale);

            transform.localScale = Vector3.one * cellAspectRatio;
            cellAspectRatio /= 2;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Building))]
    public class BuildingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Initialize"))
            {
                ((Building)target).Initialize();
            }
        }
    }
#endif
}
