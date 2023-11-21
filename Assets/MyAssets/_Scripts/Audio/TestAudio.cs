using ColdClimb.Global;
using UnityEngine;

namespace ColdClimb.Audio{
    public class TestAudio : MonoBehaviour{
        [SerializeField] private AudioController audioController;
        
        private InputManager InputManager => ResourceLoader.InputManager;
#region Unity Functions
#if UNITY_EDITOR
        private void Update() {
            if(InputManager.ReturnUseEquippedItemAction().triggered){
                audioController.PlayAudio(AudioType.SOUNDTRACK_01, true, 3.0f);
            }
            if(InputManager.ReturnAltUseEquippedItemAction().triggered){
                audioController.StopAudio(AudioType.SOUNDTRACK_01, true);
            }
            if(InputManager.ReturnSprintAction().triggered){
                audioController.RestartAudio(AudioType.SOUNDTRACK_01, true);
            }
            if(InputManager.ReturnStatusAction().triggered){
                audioController.PlayAudio(AudioType.UI_01);
            }
            if(InputManager.ReturnInteractAction().triggered){
                audioController.StopAudio(AudioType.UI_01);
            }
            if(InputManager.ReturnCancelAction().triggered){
                audioController.RestartAudio(AudioType.UI_01);
            }
        }
#endif
#endregion
    }
}
