using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    [System.Serializable]
    public class TowerOption
    {
        public string name;
        public GameObject towerPrefab;
        public int cost;
        public Sprite icon;
    }

    public static BuildManager Instance { get; private set; }

    [Header("Building Settings")]
    [SerializeField] private Tilemap groundTilemap; // Reference to your ground tilemap
    [SerializeField] private TileBase[] buildableTiles; // Array of tiles that we can build on
    [SerializeField] private Color validPlacementColor = new Color(0, 1, 0, 0.3f);
    [SerializeField] private Color invalidPlacementColor = new Color(1, 0, 0, 0.3f);
    [SerializeField] private float minDistanceFromPath = 1.0f; // Minimum distance from path tiles

    [Header("Tower Options")]
    [SerializeField] private List<TowerOption> availableTowers;
    [SerializeField] private GameObject placementIndicator;

    private bool isPlacing = false;
    private GameObject currentTowerPreview;
    private int selectedTowerIndex = -1;

    [SerializeField] private BuildMenuUI buildMenuUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (groundTilemap == null)
        {
            groundTilemap = FindObjectOfType<Tilemap>();
            Debug.LogWarning("Tilemap reference not set, attempting to find in scene.");
        }
    }

    private void Update()
    {
        if (isPlacing)
        {
            // Get mouse position in world space
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            if (currentTowerPreview != null)
            {
                currentTowerPreview.transform.position = mousePos;
                
                bool isValidPosition = IsValidPlacement(mousePos);
                UpdatePreviewColor(isValidPosition);

                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    if (isValidPosition)
                    {
                        PlaceTower(mousePos);
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    CancelPlacement();
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;

                if (IsValidPlacement(mousePos))
                {
                    buildMenuUI.ShowAtPosition(mousePos, availableTowers.ToArray());
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                buildMenuUI.Hide();
            }
        }
    }

    private bool IsValidPlacement(Vector2 worldPosition)
    {
        // Convert world position to cell position
        Vector3Int cellPosition = groundTilemap.WorldToCell(worldPosition);
        
        // Check if the clicked position is on a buildable tile
        TileBase tile = groundTilemap.GetTile(cellPosition);
        bool isBuildable = false;
        
        foreach(TileBase buildableTile in buildableTiles)
        {
            if(tile == buildableTile)
            {
                isBuildable = true;
                break;
            }
        }
        
        if(!isBuildable) return false;

        // Check for overlapping towers
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(worldPosition, 0.4f);
        foreach (var collider in hitColliders)
        {
            if (collider.GetComponent<Tower>() != null)
            {
                return false;
            }
        }

        return true;
    }

    private void UpdatePreviewColor(bool isValid)
    {
        SpriteRenderer[] renderers = currentTowerPreview.GetComponentsInChildren<SpriteRenderer>();
        Color targetColor = isValid ? validPlacementColor : invalidPlacementColor;
        
        foreach (var renderer in renderers)
        {
            // Don't change the range indicator's alpha
            if (renderer.gameObject.CompareTag("RangeIndicator"))
            {
                Color rangeColor = targetColor;
                rangeColor.a = 0.2f;
                renderer.color = rangeColor;
            }
            else
            {
                renderer.color = targetColor;
            }
        }
    }

    public void StartPlacement(int towerIndex)
    {
        if (towerIndex >= availableTowers.Count) return;

        buildMenuUI.Hide();
        
        selectedTowerIndex = towerIndex;
        TowerOption selectedTower = availableTowers[towerIndex];

        // Check if player has enough gold
        if (!GameManager.Instance.SpendGold(selectedTower.cost))
        {
            Debug.Log("Not enough gold!");
            return;
        }

        // Create preview
        currentTowerPreview = Instantiate(selectedTower.towerPrefab);
        // Disable the Tower component during preview but still show range
        Tower towerComponent = currentTowerPreview.GetComponent<Tower>();
        if (towerComponent != null)
        {
            towerComponent.enabled = false;
            towerComponent.ShowRange(true, new Color(0.5f, 0.5f, 1f, 0.2f));
        }
        SetPreviewTransparency(0.5f);
        isPlacing = true;
    }

    private void SetPreviewTransparency(float alpha)
    {
        SpriteRenderer[] renderers = currentTowerPreview.GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }

    private void PlaceTower(Vector3 position)
    {
        TowerOption selectedTower = availableTowers[selectedTowerIndex];
        GameObject tower = Instantiate(selectedTower.towerPrefab, position, Quaternion.identity);
        
        // Hide range indicator on placed tower
        Tower towerComponent = tower.GetComponent<Tower>();
        if (towerComponent != null)
        {
            towerComponent.ShowRange(false, Color.white);
        }
        
        // Reset placement state
        Destroy(currentTowerPreview);
        currentTowerPreview = null;
        isPlacing = false;
        selectedTowerIndex = -1;
    }

    public void CancelPlacement()
    {
        if (currentTowerPreview != null)
        {
            // Refund the cost
            if (selectedTowerIndex >= 0)
            {
                GameManager.Instance.AddGold(availableTowers[selectedTowerIndex].cost);
            }
            
            Destroy(currentTowerPreview);
            currentTowerPreview = null;
            isPlacing = false;
            selectedTowerIndex = -1;
        }
    }
} 