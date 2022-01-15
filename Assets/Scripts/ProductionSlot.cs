using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PanteonGames
{
    public class ProductionSlot : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public BuildingDataSC scriptable;
        public Text nameText;
        public Image image;
        float holdStartTime;

        public void Initialize()
        {
            image.sprite = scriptable.sprite;
            nameText.text = scriptable.BuildingName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // todo show information on information menu
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            holdStartTime = Time.time;

            Debug.Log("hold startTime");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("hold endTime:" + Time.time);

            if (Time.time - holdStartTime > 1)
            {
                // todo spawn item on players hand
            }
        }
    }
}