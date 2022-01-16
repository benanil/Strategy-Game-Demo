using UnityEngine;
using UnityEngine.UI;

namespace PanteonGames
{
    public class ProductionSlot : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI nameText;
        [SerializeField] private Image image;

        public void Initialize(ProdutionInfo produtionInfo)
        {
            image.sprite = produtionInfo.sprite;
            nameText.text = produtionInfo.name;
        }
    }
}