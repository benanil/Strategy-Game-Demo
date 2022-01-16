using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PanteonGames
{
    // this class used as production menu and information menu

    public class InformationMenu : MonoBehaviour
    {
        public static InformationMenu instance;

        public Text BuildingName;
        public Image BuildingImage;

        public Transform ProductionParent;
        public ProductionSlot ProductionSlotPrefab;

        private ProductionSlot[] CurrentProductions;

        // we will instiantiate productionSlots only once (kind of object pooling)
        private Dictionary<BuildingDataSO, ProductionSlot[]> productionSlotMap = new Dictionary<BuildingDataSO, ProductionSlot[]>();

        private void Awake()
        {
            CurrentProductions = new ProductionSlot[0];
            instance = this;
        }


        public void SetCurrentBuildingInformation(BuildingDataSO buildingSC)
        {
            BuildingName.text = buildingSC.BuildingName;
            BuildingImage.sprite = buildingSC.sprite;

            // remove old production infos
            HideProductionInfos();

            // search if already exist in dictionary
            if (productionSlotMap.ContainsKey(buildingSC))
            {
                CurrentProductions = productionSlotMap[buildingSC];
                for (int i = 0; i < CurrentProductions.Length; i++)
                {
                    CurrentProductions[i].gameObject.SetActive(true);
                }
            }
            else
            {
                // create new dictionary for production slot object pooling
                CurrentProductions = new ProductionSlot[buildingSC.productions.Length];

                for (int i = 0; i < buildingSC.productions.Length; i++)
                {
                    ProductionSlot production = Instantiate(ProductionSlotPrefab, ProductionParent);
                    CurrentProductions[i] = production;
                    production.Initialize(buildingSC.productions[i]);
                    production.transform.SetParent(ProductionParent);
                }
                productionSlotMap.Add(buildingSC, CurrentProductions);
            }
        }

        private void HideProductionInfos()
        {
            for (int i = 0; i < CurrentProductions.Length; i++)
            {
                CurrentProductions[i].gameObject.SetActive(false);
            }
        }

        // set visuality false
        public void Hide()
        {
            BuildingName.text = string.Empty;
            BuildingImage.sprite = null;
            HideProductionInfos();
        }

    }
}

