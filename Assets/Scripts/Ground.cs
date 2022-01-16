using UnityEngine;

namespace PanteonGames
{
    public class Ground : MonoBehaviour
    {
        [SerializeField] private HoveredChecker production, information;
        
        public void OnMouseDown()
        {
            if (production.Hovered == false && information.Hovered == false && GameManager.GetCurrentState() != GameManagerState.MovingBuilding)
            {
                InformationMenu.instance.Hide();
                GameManager.SetCurrentBuilding(null);
            }
        }
    }
}
