using ColdClimb.Audio;
using ColdClimb.Global;
using ColdClimb.Interactable;
using UnityEngine;
using UnityEngine.InputSystem;
using AudioType = ColdClimb.Audio.AudioType;

namespace ColdClimb.Item.Equipped{
    public class FlashlightBehavior : EquippedItemBehavior{
        [Header("Required Components")]
        [SerializeField] private Light flashLight;

        [Header("Speed Settings")]
        [SerializeField] private float playerLookSpeed = 15f;
        [SerializeField] private float zoomSpeed;

        [Header("Raycast Settings")]
        [SerializeField] private LayerMask ignoreLayers;
        [SerializeField] private Transform flashlightRayPoint;

        [Header("Light Settings")]
        [SerializeField] private float defaultRange;
        [SerializeField] private float zoomedRange;
        [SerializeField] private float defaultIntensity;
        [SerializeField] private float zoomedIntensity;
        [SerializeField] private float defaultOuterSpotAngle;
        [SerializeField] private float zoomedOuterSpotAngle;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioType toggleAudio;
        [SerializeField] private AudioType zoomInAudio;
        [SerializeField] private AudioType zoomOutAudio;

        private InspectObject currentInspectObject;
        private AudioController AudioController => AudioController.instance;
        private bool startedZoom = false;

        private void Start(){
            SetDefaultFlashLight();
            ResourceLoader.MainPlayerData.playerStats.lookSpeed = playerLookSpeed;
        }

        public override void SetupBehavior(EquipableItem item){
            
        }

        public void Update(){
            if(!flashLight.enabled){
                ResetCurrentInspectObject();
                return;
            } 

            //Send a raycast out the size of our flashlight range
            if(Physics.Raycast(flashlightRayPoint.position, flashlightRayPoint.forward, out RaycastHit hitInfo, flashLight.range, ~ignoreLayers)){
                if(hitInfo.collider.TryGetComponent(out InspectObject inspectObject) && inspectObject.NeedsLight && inspectObject != currentInspectObject){
                    ResetCurrentInspectObject();
                    currentInspectObject = inspectObject;
                    currentInspectObject.SetInspectable(true);
                } 
            } 
            else{
                ResetCurrentInspectObject();
            } 
        }

        private void ResetCurrentInspectObject(){
            if(currentInspectObject != null){
                currentInspectObject.SetInspectable(false);
                currentInspectObject = null;
            }
        }

        private void OnDestroy(){
            ResourceLoader.MainPlayerData.playerStats.lookSpeed = 30f;
            ResetCurrentInspectObject();
        }

        private void SetDefaultFlashLight(){
            flashLight.range = defaultRange;
            flashLight.intensity = defaultIntensity;
            flashLight.spotAngle = defaultOuterSpotAngle;
        }

        public override void Use(InputAction action){
            if(onCooldown) return;
            if(action.triggered){
                flashLight.enabled = !flashLight.enabled;
                AudioController.PlayAudio(toggleAudio, false, 0, 0, audioSource);
                ActionCooldown(actionCooldownTime);
            }
        }

        public override void AltUse(InputAction action){
            if(action.IsPressed()){
                LerpLight(FlashLightType.Zoomed);
                if(!startedZoom){
                    AudioController.PlayAudio(zoomInAudio, false, 0, 0, audioSource);   
                    startedZoom = true;
                }
            } 
            else if(flashLight.range != defaultRange){
                if(startedZoom){
                    AudioController.PlayAudio(zoomOutAudio, false, 0, 0, audioSource); 
                    startedZoom = false;
                }
                LerpLight(FlashLightType.Default);
            } 
        }

        private void LerpLight(FlashLightType type){
            switch (type){
                case FlashLightType.Default: flashLight.range = Mathf.Lerp(flashLight.range, defaultRange, zoomSpeed * Time.deltaTime);
                                            flashLight.intensity = Mathf.Lerp(flashLight.intensity, defaultIntensity, zoomSpeed * Time.deltaTime);
                                            flashLight.spotAngle = Mathf.Lerp(flashLight.spotAngle, defaultOuterSpotAngle, zoomSpeed * Time.deltaTime);
                                            flashLight.innerSpotAngle = defaultOuterSpotAngle / 2;
                                            break;
                case FlashLightType.Zoomed: flashLight.range = Mathf.Lerp(flashLight.range, zoomedRange, zoomSpeed * Time.deltaTime);
                                            flashLight.intensity = Mathf.Lerp(flashLight.intensity, zoomedIntensity, zoomSpeed * Time.deltaTime);
                                            flashLight.spotAngle = Mathf.Lerp(flashLight.spotAngle, zoomedOuterSpotAngle, zoomSpeed * Time.deltaTime);
                                            flashLight.innerSpotAngle = zoomedOuterSpotAngle / 2;
                                            break;
            }
        }

        public override void UseResource(InputAction action){
            return;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(flashlightRayPoint.position, flashlightRayPoint.forward * flashLight.range);
        }
    }

    public enum FlashLightType{
        Default,
        Zoomed
    }

}