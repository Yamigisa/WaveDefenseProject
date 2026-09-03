using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UnitUICard : MonoBehaviour
{
    [SerializeField] private Button buyUnitButton;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI unitCostText;

    private UnitSO unitSO;

    private void OnEnable()
    {
        ResourceManager.Instance.ResourceChanged += HandleResourceChanged;
        RefreshBuyButton();
    }

    private void OnDisable()
    {
        ResourceManager.Instance.ResourceChanged -= HandleResourceChanged;
    }

    public void SetUnit(UnitSO _unitSO)
    {
        unitSO = _unitSO;
        image.sprite = unitSO.sprite;
        unitCostText.text = unitSO.cost.ToString();
    }

    private void HandleResourceChanged(ResourceType changedType, int newAmount)
    {
        if (changedType == ResourceType.Gold)
            RefreshBuyButton();
    }

    private void RefreshBuyButton()
    {
        buyUnitButton.interactable = ResourceManager.Instance.GetAmount(ResourceType.Gold) >= unitSO.cost;
    }
}
