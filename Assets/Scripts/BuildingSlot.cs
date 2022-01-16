using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PanteonGames
{
    public class BuildingSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Image image;
        
        private BuildingDataSO scriptable;

        private float holdingTimeStart;
        private bool holding;

        Coroutine currentCoroutine;

        public void Initialize(BuildingDataSO _scriptable)
        {
            scriptable = _scriptable;
            image.sprite = scriptable.sprite;
            nameText.text = scriptable.BuildingName;
        }

        IEnumerator FadeWhiteCoroutine()
        {
            while (image.color.r < 0.9)
            {
                image.color = Color.Lerp(image.color, Color.white, Time.deltaTime * 2);
                yield return new WaitForEndOfFrame();
            }
            image.color = Color.white;
        }

        IEnumerator FadeGrayCoroutine()
        {
            while (image.color.r > 0.2)
            {
                image.color = Color.Lerp(image.color, Color.black, Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            SpawnBuilding();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            holdingTimeStart = Time.time;
            holding = true;

            if (currentCoroutine != null) {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(FadeGrayCoroutine());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            float holdDuration = Time.time - holdingTimeStart;

            if (currentCoroutine != null) {
                StopCoroutine(currentCoroutine);
            }

            if (holding && holdDuration > 0.25f)
            {
                SpawnBuilding();
            }
         
            currentCoroutine = StartCoroutine(FadeWhiteCoroutine());

            holding = false;
        }

        // spawns building to players hand
        private void SpawnBuilding()
        {
            if (GameManager.GetCurrentState() != GameManagerState.MovingBuilding)
            {
                // create new building
                GameObject newBuildingGO = new GameObject(scriptable.BuildingName);
                newBuildingGO.AddComponent<SpriteRenderer>();
                Building building = newBuildingGO.AddComponent<Building>();
                building.Initialize(scriptable);

                GameManager.MoveBuilding(building);
            }
            holding = false;
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            holding = false;
        }
    }
}