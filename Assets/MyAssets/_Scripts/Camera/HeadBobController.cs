using ColdClimb.Player;
using UnityEngine;

namespace ColdClimb.Camera{
    [RequireComponent(typeof(Rigidbody))]
    public class HeadBobController : MonoBehaviour{
        #region Variables
        [Header("Bobbing Settings")]
        [SerializeField, Range(0, 0.1f)] private float bobbingAmplitude = 0.003f;
        [SerializeField, Range(0, 30f)] private float bobbingFrequency = 7.0f;

        [SerializeField, Range(0, 0.1f)] private float runningBobbingAmplitude = 0.0055f;
        [SerializeField, Range(0, 30f)] private float runningBobbingFrequency = 15f;

        [SerializeField, Range(1f, 5f)] private float headBobResetSpeed;

        [SerializeField] private Transform playerCamera = null;

        [SerializeField] private float requiredSpeedToBob = 0.5f;

        private Rigidbody PlayerRigidbody => GetComponent<Rigidbody>();
        private Vector3 PlayerVelocity => new(PlayerRigidbody.velocity.x, 0, PlayerRigidbody.velocity.z);

        private Vector3 startPos;
        private float defaultAmplitude;
        private float defaultFrequency;
        #endregion

        #region Setup
        private void Awake(){
            PlayerMovement.OnSprintAction += EvaluateSprintBob;
        } 

        private void OnDestroy(){
        PlayerMovement.OnSprintAction -= EvaluateSprintBob;   
        }
    
        private void Start(){
            startPos = playerCamera.localPosition;
            defaultAmplitude = bobbingAmplitude;
            defaultFrequency = bobbingFrequency;
        }
        #endregion

        #region Checks
        private void Update(){
            CheckMotion();

            ResetPostion();
        }

        private void CheckMotion(){
            var playerSpeed = PlayerVelocity.magnitude;

            if (playerSpeed < requiredSpeedToBob) return;
            PlayMotion(StepMotionCalculation());
        }
        #endregion

        #region Camera Movement
        private void PlayMotion(Vector3 motion){
            playerCamera.localPosition += motion;
        }

        private void ResetPostion(){
            if (playerCamera.localPosition == startPos) return;
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, startPos, headBobResetSpeed * Time.deltaTime);
        }

        private Vector3 StepMotionCalculation(){
            Vector3 pos = Vector3.zero;
            pos.y += Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;
            pos.x += Mathf.Cos(Time.time * bobbingFrequency / 2) * bobbingAmplitude * 2;
            return pos;
        }

        private void EvaluateSprintBob(bool isSprinting){
            if(isSprinting){
                bobbingAmplitude = runningBobbingAmplitude;
                bobbingFrequency = runningBobbingFrequency;
            }
            else{
                bobbingAmplitude = defaultAmplitude;
                bobbingFrequency = defaultFrequency;
            }
        }
        #endregion
    }
}

