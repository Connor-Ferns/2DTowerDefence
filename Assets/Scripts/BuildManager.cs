using UnityEngine;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    [System.Serializable]
    public class TowerOption
    {
        public string name;
        public GameObject towerPrefab;
        public int cost;
        public Sprite icon;
    }

    [Header("Tower Options")]
    [SerializeField] private List<TowerOption> availableTowers;
    
    [Header("UI References")]
    [SerializeField] private BuildMenuUI buildMenuUI;
    
    private BuildingSpot selectedSpot;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            // Validate required references
            if (buildMenuUI == null)
            {
                Debug.LogError("BuildMenuUI reference is missing in BuildManager!");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSelectedBuildingSpot(BuildingSpot spot)
    {
        if (buildMenuUI == null)
        {
            Debug.LogError("BuildMenuUI reference is missing! Please assign it in the Unity Inspector.");
            return;
        }

        // If clicking the same spot, deselect it
        if (selectedSpot == spot)
        {
            CancelBuild();
            return;
        }

        selectedSpot = spot;
        buildMenuUI.ShowAtPosition(spot.transform.position, availableTowers.ToArray());
    }

    public void AttemptToBuild(int towerIndex)
    {
        if (selectedSpot == null || towerIndex >= availableTowers.Count) return;

        TowerOption selectedTower = availableTowers[towerIndex];
        
        if (GameManager.Instance.SpendGold(selectedTower.cost))
        {
            selectedSpot.BuildTower(selectedTower.towerPrefab);
            CancelBuild();
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }

    public void CancelBuild()
    {
        selectedSpot = null;
        buildMenuUI.Hide();
    }
} 