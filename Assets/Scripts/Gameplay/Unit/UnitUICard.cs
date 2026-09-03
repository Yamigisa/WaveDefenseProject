using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UnitUICard : MonoBehaviour
{
    [SerializeField] private Button buyUnitButton;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI unitCostText;

    [Header("Affordability Visuals")] [Range(0f, 1f)][SerializeField] private float unaffordableAlpha = 0.5f;
    private CanvasGroup cardCanvasGroup;
    private UnitSO unitSO;

    private void Awake()
    {
        cardCanvasGroup = GetComponent<CanvasGroup>();

        buyUnitButton.onClick.AddListener(SelectUnit);
    }

    private void OnDestroy()
    {
        buyUnitButton.onClick.RemoveListener(SelectUnit);
    }

    private void OnEnable()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.ResourceChanged += HandleResourceChanged;

        RefreshBuyButton();
    }

    private void OnDisable()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.ResourceChanged -= HandleResourceChanged;
    }

    public void SetUnit(UnitSO _unitSO)
    {
        unitSO = _unitSO;
        image.sprite = unitSO.sprite;
        unitCostText.text = unitSO.cost.ToString();
        RefreshBuyButton();
    }

    private void HandleResourceChanged(ResourceType changedType, int newAmount)
    {
        if (changedType == ResourceType.Gold)
            RefreshBuyButton();
    }

    private void RefreshBuyButton()
    {
        bool canAfford = unitSO != null &&
            ResourceManager.Instance != null &&
            ResourceManager.Instance.GetAmount(ResourceType.Gold) >= unitSO.cost;

        buyUnitButton.interactable = canAfford;
        cardCanvasGroup.alpha = canAfford ? 1f : unaffordableAlpha;
    }

    public void SelectUnit()
    {
        if (unitSO == null || ResourceManager.Instance == null ||
            ResourceManager.Instance.GetAmount(ResourceType.Gold) < unitSO.cost)
            return;

        if (PlacementSystem.Instance == null)
        {
            Debug.LogError("A PlacementSystem is required before a unit can be selected.");
            return;
        }

        PlacementSystem.Instance.StartPlacement(unitSO);
    }
}
