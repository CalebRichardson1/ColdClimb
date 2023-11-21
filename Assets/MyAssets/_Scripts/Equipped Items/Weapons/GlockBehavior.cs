using ColdClimb.Audio;
using ColdClimb.Generic;
using ColdClimb.Global;
using ColdClimb.Inventory;
using ColdClimb.Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using AudioType = ColdClimb.Audio.AudioType;

namespace ColdClimb.Item.Equipped{
    public class GlockBehavior : EquippedItemBehavior{
        [Header("Required Components")]
        [SerializeField] private Transform anchor;
        [SerializeField] private Transform shootSpawnPos;
        [SerializeField] private LayerMask canBeShot;
        [SerializeField] private TMP_Text currentAmmoText;

        [Header("Weapon Positions")]
        [SerializeField] private Transform hipPos;
        [SerializeField] private Transform adsPos;
        [SerializeField] private Transform runningPos;
        [SerializeField] private Transform checkingAmmoPos;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioType fireGlockAudio;
        [SerializeField] private AudioType reloadGlockAudio;
        [SerializeField] private AudioType emptyGlockAudio;


        private AudioController AudioController => AudioController.instance;
        private PlayerInventory PlayerInventory => ResourceLoader.PlayerInventory;

        private GunEquipableItem gunItem;
        private GunStats gunStats;

        private float aimedBloomDivider = 2f;

        private float rangeToSnap = 0.005f;
        private bool isRunning;
        private bool fullyAimedIn;
        private bool checkingAmmo;

        private void OnEnable(){
            PlayerMovement.OnSprintAction += (state) => isRunning = state;
            ResourceLoader.InputManager.ReturnReloadAction().performed += GetPerformedReloadInteraction;
            ResourceLoader.InputManager.ReturnReloadAction().canceled += GetCanceledReloadInteraction;
        }
        private void OnDisable(){
            PlayerMovement.OnSprintAction -= (state) => isRunning = state;
            ResourceLoader.InputManager.ReturnReloadAction().performed -= GetPerformedReloadInteraction;
            ResourceLoader.InputManager.ReturnReloadAction().canceled -= GetCanceledReloadInteraction;
        }

        public override void SetupBehavior(EquipableItem item){
            gunItem = (GunEquipableItem)item;
            gunStats = gunItem.GunStats;
            currentAmmoText.enabled = false;
        }

        //glock firing
        public override void Use(InputAction action){
            if(onCooldown || isRunning || checkingAmmo) return;
            if(gunStats.currentAmmo == 0 && action.triggered){
                AudioController.PlayAudio(emptyGlockAudio, false, 0, 0, audioSource);
                return;
            }

            Debug.DrawRay(shootSpawnPos.position, shootSpawnPos.forward * gunStats.fireRange, Color.green);

            if(gunStats.isAuto){
                    if(action.IsPressed()){
                    Shoot();
                }
                return;
            }

            if(action.triggered){
                Shoot();
            }  
        }

        private void Shoot(){
            AudioController.PlayAudio(fireGlockAudio, false, 0, 0, audioSource);
            var bloom = Bloom();
            Physics.Raycast(shootSpawnPos.position, bloom, out RaycastHit hit, gunStats.fireRange, canBeShot);

            if (hit.transform != null){
                Debug.DrawRay(shootSpawnPos.position, bloom * gunStats.fireRange, Color.white);
                if(hit.collider.TryGetComponent(out Health health) && health != null){
                    health.TakeDamage(gunStats.attackDamage);
                }
            }

            GunRecoil();
            AmmoCalculation();
            ActionCooldown(gunStats.fireRate);
        }

        private void AmmoCalculation(){
            gunStats.currentAmmo--;
        }

        public override void UseResource(InputAction action){
            if(isRunning){
                checkingAmmo = false;
                currentAmmoText.enabled = false;
            } 
            if(checkingAmmo){
                Aim(checkingAmmoPos);
            }
        }

        private void GetPerformedReloadInteraction(InputAction.CallbackContext context){
            if(context.interaction is HoldInteraction && !isRunning){
                checkingAmmo = true;
                currentAmmoText.enabled = true;
                currentAmmoText.text = gunStats.currentAmmo + "/" + gunStats.maxAmmo;
                currentAmmoText.color = gunStats.currentAmmo == gunStats.maxAmmo ? Color.green : Color.white;
                if(gunStats.currentAmmo == 0){
                    currentAmmoText.color = Color.red;
                }
            } 
            if(context.interaction is MultiTapInteraction) ReloadGun();
        }

        private void GetCanceledReloadInteraction(InputAction.CallbackContext context){
            if(context.interaction is HoldInteraction && checkingAmmo == true) {
                checkingAmmo = false;
                currentAmmoText.enabled = false;
            }
        }

        private void ReloadGun(){
            //quick reload
            if(!checkingAmmo && gunStats.currentAmmo < gunStats.maxAmmo){
                //reload timer
                var amount = PlayerInventory.GetAmmoAmount(gunStats.gunType);
                if(amount == 0) return;
                AudioController.PlayAudio(reloadGlockAudio, false, 0, 0, audioSource);
                //remove all current ammo if any remains
                gunStats.currentAmmo = 0;
                if(amount >= gunStats.maxAmmo){
                    gunStats.currentAmmo = gunStats.maxAmmo;
                    PlayerInventory.RemoveGunAmmo(gunStats.gunType, gunStats.maxAmmo);
                }
                else{
                    gunStats.currentAmmo = amount;
                    PlayerInventory.RemoveGunAmmo(gunStats.gunType, amount);
                }
            }
        }

        private void GunRecoil(){
            anchor.Rotate(-gunStats.visualRecoil, 0, 0);
            anchor.position -= anchor.forward * gunStats.basekickback;
        }

        private Vector3 Bloom(){
            // base bloom
            var baseBloom = gunStats.baseBloom;
            if(fullyAimedIn){
                baseBloom /= aimedBloomDivider;
            }
            Vector3 bloom = shootSpawnPos.position + shootSpawnPos.forward * gunStats.fireRange;
            bloom += Random.Range(-baseBloom, baseBloom) * shootSpawnPos.up;
            bloom += Random.Range(-baseBloom, baseBloom) * shootSpawnPos.right;
            bloom -= shootSpawnPos.position;
            bloom.Normalize();
            return bloom;
        }

        //glock ads
        public override void AltUse(InputAction action){ 
            //if running set aim pos to running
            if(isRunning){
                Aim(runningPos);
                fullyAimedIn = false;
                return;
            } 

            if(checkingAmmo) return;

            //aiming in action
            if(action.IsPressed()){
                Aim(adsPos);
                fullyAimedIn = true;
            }
            else if(anchor.position != hipPos.position){
                Aim(hipPos);
                fullyAimedIn = false;
            }
        }

        private void Aim(Transform aimPos){
            var currentLerp = Vector3.Slerp(anchor.position, aimPos.position, Time.deltaTime * gunStats.aimSpeed);
            anchor.position = currentLerp;
            if(Vector3.Distance(anchor.position, aimPos.position) <= rangeToSnap){
                anchor.position = aimPos.position;
            }

            var currentLerpRotation = Quaternion.Slerp(anchor.localRotation, aimPos.localRotation, Time.deltaTime * gunStats.aimSpeed);
            anchor.localRotation = currentLerpRotation;
        }
    }
}  
