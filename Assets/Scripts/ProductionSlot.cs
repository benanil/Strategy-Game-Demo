using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace PanteonGames
{
    public class ProductionSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMPro.TextMeshProUGUI nameText;
        [SerializeField] private Image image;
        ProdutionInfo produtionInfo;

        public void Initialize(ProdutionInfo _produtionInfo)
        {
            produtionInfo = _produtionInfo;
            image.sprite = produtionInfo.sprite;
            nameText.text = produtionInfo.name;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (produtionInfo.initialData is SoldierSO soldier)
            {
                GameManager.ProduceSoldier(soldier);
            }
            else
            {
                throw new System.Exception("production slot: Production Info initial data must be SoldierSO!");
            }
        }
    }
}