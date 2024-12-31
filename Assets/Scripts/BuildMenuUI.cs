using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildMenuUI : MonoBehaviour
{
    [System.Serializable]
    public class TowerButton
    {
        public Button button;
        public Image icon;
        public TextMeshProUGUI costText;
    }

    [SerializeField] private TowerButton[] towerButtons;
    [SerializeField] private RectTransform menuPanel;
    [SerializeField] private Vector2 offset = new Vector2(0, 50f); // Offset from click position
    
    private Canvas canvas;
    private Vector2 screenBounds;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("BuildMenuUI must be child of a Canvas!");
        }
        
        // Get screen bounds in canvas space
        screenBounds = new Vector2(Screen.width, Screen.height);
    }

    private void Start()
    {
        // Setup button listeners
        for (int i = 0; i < towerButtons.Length; i++)
        {
            int index = i;
            towerButtons[i].button.onClick.AddListener(() => BuildManager.Instance.StartPlacement(index));
        }

        // Hide menu initially
        gameObject.SetActive(false);
    }

    public void ShowAtPosition(Vector3 worldPosition, BuildManager.TowerOption[] towers)
    {
        // Convert world position to screen position
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        
        // Add offset
        screenPosition += offset;

        // Keep menu within screen bounds
        float menuWidth = menuPanel.rect.width * canvas.scaleFactor;
        float menuHeight = menuPanel.rect.height * canvas.scaleFactor;

        screenPosition.x = Mathf.Clamp(screenPosition.x, menuWidth/2, screenBounds.x - menuWidth/2);
        screenPosition.y = Mathf.Clamp(screenPosition.y, menuHeight/2, screenBounds.y - menuHeight/2);

        // Set position
        menuPanel.position = screenPosition;

        // Update tower buttons
        for (int i = 0; i < towerButtons.Length; i++)
        {
            if (i < towers.Length)
            {
                towerButtons[i].button.gameObject.SetActive(true);
                towerButtons[i].icon.sprite = towers[i].icon;
                towerButtons[i].costText.text = $"${towers[i].cost}";
            }
            else
            {
                towerButtons[i].button.gameObject.SetActive(false);
            }
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
} 