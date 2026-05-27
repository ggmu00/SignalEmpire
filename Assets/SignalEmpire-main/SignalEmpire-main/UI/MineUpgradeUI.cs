using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MineUpgradeUI : MonoBehaviour
{
    public PlanetData currentPlanet;
    public SignalManager signalManager;
    public GameObject nodePrefab;
    public Transform nodeContainer;
    public TextMeshProUGUI planetNameText;
    public TextMeshProUGUI dataText;

    private List<GameObject> nodeObjects = new List<GameObject>();

    public void ShowPlanet(PlanetData planet)
    {
        currentPlanet = planet;
        planetNameText.text = planet.planetName + " Mine Upgrades";
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (currentPlanet == null) return;

        // Clear existing nodes
        foreach (GameObject obj in nodeObjects) Destroy(obj);
        nodeObjects.Clear();

        // Create node UI elements
        foreach (MineUpgradeNode node in currentPlanet.mineUpgradeNodes)
        {
            GameObject nodeObj = Instantiate(nodePrefab, nodeContainer);
            nodeObjects.Add(nodeObj);

            TextMeshProUGUI titleText = nodeObj.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descText = nodeObj.transform.Find("Description").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI costText = nodeObj.transform.Find("Cost").GetComponent<TextMeshProUGUI>();
            Button purchaseBtn = nodeObj.transform.Find("PurchaseButton").GetComponent<Button>();

            titleText.text = node.title;
            descText.text = node.description;
            costText.text = node.cost.ToString("N0") + " Data";

            bool canPurchase = !node.isPurchased && node.IsUnlocked(currentPlanet.mineUpgradeNodes) && signalManager.data >= node.cost;
            purchaseBtn.interactable = canPurchase;
            purchaseBtn.onClick.RemoveAllListeners();
            purchaseBtn.onClick.AddListener(() => PurchaseNode(node.id));

            if (node.isPurchased)
            {
                nodeObj.GetComponent<Image>().color = Color.green;
            }
            else if (!node.IsUnlocked(currentPlanet.mineUpgradeNodes))
            {
                nodeObj.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                nodeObj.GetComponent<Image>().color = Color.white;
            }
        }

        dataText.text = "Data: " + signalManager.data.ToString("N0");
    }

    private void PurchaseNode(string id)
    {
        if (currentPlanet.PurchaseMineUpgrade(id, ref signalManager.data))
        {
            RefreshUI();
            // Notify layer unlock manager
            FindObjectOfType<LayerUnlockManager>()?.CheckLayerUnlocks();
        }
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            dataText.text = "Data: " + signalManager.data.ToString("N0");
        }
    }
}