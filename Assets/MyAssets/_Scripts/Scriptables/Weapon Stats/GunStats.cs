using UnityEngine;

namespace ColdClimb.Item{
    [CreateAssetMenu(menuName = "Weapon Stats/New Gun Stats", fileName = "NewGunStats"), System.Serializable]
    public class GunStats : ScriptableObject{
        [Header("Base Gun Settings")]
        public GunType gunType;
        public float fireRate;
        public int attackDamage;
        public float fireRange;
        public float aimSpeed;

        [Header("Gun Ammo Settings")]
        public int maxAmmo;
        public int currentAmmo;
        public float reloadSpeed;
        
        [Header("Accuracy Settings")]
        public float baseBloom;
        public float visualRecoil;
        public float basekickback;

        [Header("Gun Type Settings")]
        public bool isAuto;
        public bool burstFire;
        public int burstAmount;
    }

    public enum GunType{
        Pistol,
        Shotgun
    }
}
