using UnityEngine;
using UnityEngine.EventSystems;

namespace PanteonGames
{
    public class HoveredChecker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool Hovered;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Hovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Hovered = false;
        }
    }
}
