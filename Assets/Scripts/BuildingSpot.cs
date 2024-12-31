using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingSpot : MonoBehaviour
{
    [SerializeField] private Color hoverColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color unavailableColor = new Color(0.8f, 0.2f, 0.2f, 1f);
    
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private bool isOccupied = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; // Ignore if over UI
        spriteRenderer.color = isOccupied ? unavailableColor : hoverColor;
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = originalColor;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; // Ignore if over UI
        if (isOccupied) return;

        BuildManager.Instance.SetSelectedBuildingSpot(this);
    }

    public void BuildTower(GameObject towerPrefab)
    {
        if (isOccupied) return;

        Vector3 position = transform.position;
        Instantiate(towerPrefab, position, Quaternion.identity);
        isOccupied = true;
        spriteRenderer.color = originalColor;
    }
} 