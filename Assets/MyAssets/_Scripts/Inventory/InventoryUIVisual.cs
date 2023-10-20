using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Handles the visual aspect of the InventoryUI
public class InventoryUIVisual : MonoBehaviour
{
    #region Variables
    public Transform InventorySlotContent => inventorySlotParent;
    public Transform ContextMenuSpawnPoint => contextMenuSpawnPoint;
    public Button EquippedItemButton => equippedItemButton;

    [Header("Selected Item Information")]
    [SerializeField] private TMP_Text selectedItemNameText;
    [SerializeField] private TMP_Text selectedItemDescriptionText;
    [SerializeField] private Transform selectedItemImageSpawnPos;

    [Header("Inventory Controller UI Dependencies")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform contextMenuSpawnPoint;
    [SerializeField] private Button equippedItemButton;

    private InventoryUIController controller;
    private GameObject selectedItemImageObj;
    #endregion

    #region Setup
    public void SetInventoryUIController(InventoryUIController uIController) => controller = uIController;
    private void Start() => controller.OnSlotSelected += UpdateSelectedInfo;
    private void OnDestroy() => controller.OnSlotSelected -= UpdateSelectedInfo;
    #endregion

    #region Updating Visuals
    public void UpdateSelectedInfo(InventorySlot selectedSlot){
        if(selectedItemImageObj != null){
            Destroy(selectedItemImageObj);
        }
        if(selectedSlot.ItemInSlot.ItemData == null){
            selectedItemNameText.text = "";
            selectedItemDescriptionText.text = "";
            return;
        }

        selectedItemNameText.SetText(selectedSlot.ItemInSlot.ItemData.Name);
        selectedItemDescriptionText.SetText(selectedSlot.ItemInSlot.ItemData.Description[0]);
        selectedItemImageObj = Instantiate(selectedSlot.ItemImage, selectedItemImageSpawnPos.position, Quaternion.identity, selectedItemImageSpawnPos).gameObject;
    }
    #endregion
}
