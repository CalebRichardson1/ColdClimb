using System.Collections;
using System.Collections.Generic;
using ColdClimb.Audio;
using ColdClimb.Global;
using UnityEngine;
using AudioType = ColdClimb.Audio.AudioType;
using Random = UnityEngine.Random;

namespace ColdClimb.Player{
    public class PlayerSteppingAudio : MonoBehaviour{
        // members
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private StepAudio[] stepAudio;
        [SerializeField] private Rigidbody playerRigidbody;
        [SerializeField] private float walkingStepTime = 1f;
        [SerializeField] private float runningStepTime = 0.5f;
        [SerializeField] private AudioSource playerAudioSource;

        private AudioController AudioController => AudioController.instance;

        private const float runningMinRequirment = 7;
        private const float movementRequiredToStep = 0.03f;

        private float velocityMagnitude;
        private float currentStepTime;

        private Dictionary<TerrainType, StepAudio> stepDictionary; // relationship between terrain type (key) and step audio class (value)
        private IEnumerator stepCoroutine;
        
        private TerrainType currentTerrainID = TerrainType.None;
        private AudioType[] terrainAudio;
        private WaitForSecondsRealtime timeToWait;

        [System.Serializable]
        private class StepAudio{
            public TerrainType associatedTerrain;
            public AudioType[] stepAudio;
        }
#region Unity Functions
        private void Awake() {
            Configure();
        }

        private void OnDisable(){
            Dispose();
        }

        private void Update() {
            DetectValidStep();
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, Vector3.down * 1.1f);
        }
        #endregion

        #region Public Functions

        #endregion

        #region Private Functions
        private void Configure(){
            stepDictionary = new();
            timeToWait = new(currentStepTime);
            GenerateStepDictionary();
        }

        private void Dispose(){
            StopStepCoroutine();
        }

        private void GenerateStepDictionary(){
            foreach (StepAudio audio in stepAudio){
                // Do not duplicatie keys
                if(stepDictionary.ContainsKey(audio.associatedTerrain)){
                    ResourceLoader.GlobalLogger.Log($"[PlayerStep]: You are trying to register step audio [{audio.associatedTerrain}] that has already been registered");
                }
                else{
                    stepDictionary.Add(audio.associatedTerrain, audio);
                    ResourceLoader.GlobalLogger.Log($"[PlayerStep]: Registering step audio for [{audio.associatedTerrain}]");
                }
            }
        }

        private void DetectGroundType(){
            if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f, groundLayer) && hit.collider.TryGetComponent(out TerrainID ID) && ID != null){
                if(ID.terrainType != currentTerrainID){
                    currentTerrainID = ID.terrainType;
                    // Get the TerrainAudio from the new terrain
                    terrainAudio = ReturnCurrentTerrainStepAudio();
                }
            }
        }

        private void DetectValidStep(){
            velocityMagnitude = playerRigidbody.velocity.magnitude;

            if(velocityMagnitude > movementRequiredToStep){
                DetectGroundType();
                
                currentStepTime = velocityMagnitude < runningMinRequirment ? walkingStepTime : runningStepTime;
                if(timeToWait.waitTime != currentStepTime){
                    timeToWait.waitTime = currentStepTime;
                }
                
                if(stepCoroutine == null && currentTerrainID != TerrainType.None){
                    stepCoroutine = StepAudioCoroutine();
                    StartCoroutine(stepCoroutine);
                }
                return;
            }

            if(stepCoroutine != null){
                StopStepCoroutine();
            }
        }

        private void StopStepCoroutine(){
            if(stepCoroutine != null){
                StopCoroutine(stepCoroutine);
                stepCoroutine = null;
            }
        }

        private IEnumerator StepAudioCoroutine(){
            int previousIndex = 0;
            int index;
            while (true){
                index = GetUniqueRandomIndex(terrainAudio.Length, previousIndex);
                ResourceLoader.GlobalLogger.Log($"[PlayerStep]: Playing step sound {terrainAudio[index]} for terrain [{currentTerrainID}].");
                AudioController.PlayAudio(terrainAudio[index], false, 0, 0, playerAudioSource);
                yield return timeToWait;
            }
        }

        private AudioType[] ReturnCurrentTerrainStepAudio(){
            if (!stepDictionary.ContainsKey(currentTerrainID)){
                ResourceLoader.GlobalLogger.Log($"[PlayerStep]: No step entry found for [{currentTerrainID}], did you forget to add it in the inspector?");
                return null;
            }

            stepDictionary.TryGetValue(currentTerrainID, out StepAudio stepAudioClass);

            if (stepAudioClass == null){
                ResourceLoader.GlobalLogger.Log($"[PlayerStep]: No valid StepAudio found for [{currentTerrainID}], did you forget to add it in the inspector?");
                return null;
            }

            AudioType[] terrainStepAudio = stepAudioClass.stepAudio;
            return terrainStepAudio;
        }

        private int GetUniqueRandomIndex(int length, int lastIndex){
            int index;
            do{
                index = Random.Range(0, length);
            } while (index == lastIndex);
            
            return index;
            }
#endregion
    }
}
