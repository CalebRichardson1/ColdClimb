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
        [SerializeField] private float baseStepTime = 3.75f;
        [SerializeField] private AudioSource playerAudioSource;

        private const float movementRequiredToStep = 3.5f;
        private const float sameStepTimeRange = 0.1f;

        private float previousStepTime;

        private AudioController AudioController => AudioController.instance;

        private Dictionary<TerrainType, StepAudio> stepDictionary; // relationship between terrain type (key) and step audio class (value)
        private IEnumerator stepCoroutine;

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

        private void FixedUpdate() {
            DetectGroundType();
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
            GenerateStepDictionary();
        }

        private void Dispose(){
            AttemptStopStepCoroutine(false);
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
                CreateStep(ID.terrainType);
            }
        }

        private void CreateStep(TerrainType terrain){
            float velocityMagnitude = playerRigidbody.velocity.magnitude;

            if(velocityMagnitude > movementRequiredToStep){
                float stepTime = baseStepTime / velocityMagnitude;
                if(stepCoroutine == null || Mathf.Abs(stepTime - previousStepTime) > sameStepTimeRange){
                    AttemptStopStepCoroutine(true);
                    previousStepTime = stepTime;
                    stepCoroutine = StepAudioCoroutine(terrain, stepTime);
                    StartCoroutine(stepCoroutine);
                }
            }
            else{
                AttemptStopStepCoroutine(false);
            }
        }

        private void AttemptStopStepCoroutine(bool stopSound){
            if(stopSound) playerAudioSource.Stop();
            if(stepCoroutine != null){
                StopCoroutine(stepCoroutine);
                stepCoroutine = null;
            }
        }

        private IEnumerator StepAudioCoroutine(TerrainType terrainType, float timeToStep){
            // Determine if terrainType is in the Dictionary
            if(!stepDictionary.ContainsKey(terrainType)){
                ResourceLoader.GlobalLogger.Log($"[PlayerStep]: No step entry found for [{terrainType}], did you forget to add it in the inspector?");
                yield return null;
            }

            stepDictionary.TryGetValue(terrainType, out StepAudio stepAudioClass);

            if(stepAudioClass == null){
                ResourceLoader.GlobalLogger.Log($"[PlayerStep]: No valid StepAudio found for [{terrainType}], did you forget to add it in the inspector?");
                yield return null;
            }

            AudioType[] terrainStepAudio = stepAudioClass.stepAudio;
            WaitForSeconds timeToWait = new WaitForSeconds(timeToStep);
            int previousIndex = 0;
            int index; 
            while (true){
                index = GetUniqueRandomIndex(terrainStepAudio.Length, previousIndex);
                ResourceLoader.GlobalLogger.Log($"[PlayerStep]: Playing step sound {terrainStepAudio[index]} for terrain [{terrainType}].");
                AudioController.PlayAudio(terrainStepAudio[index], false, 0, 0, playerAudioSource);
                yield return timeToWait;
            }
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
