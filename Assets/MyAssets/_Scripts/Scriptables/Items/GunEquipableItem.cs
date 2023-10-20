using UnityEngine;

[CreateAssetMenu(menuName = "Item/New Gun Equipable Item", fileName = "NewGunEquipableItem"), System.Serializable]
public class GunEquipableItem : EquipableItem
{
    public GunStats GunStats => gunStats;

    [Header("Gun Settings")]
    [SerializeField] private GunStats gunStats;
}
