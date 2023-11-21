using UnityEngine;
using UnityEngine.EventSystems;

namespace ColdClimb.Audio{
    public class UIAudioController : MonoBehaviour, ISelectHandler, ISubmitHandler{
        // members
        [SerializeField] private AudioType hoverAudio;
        [SerializeField] private AudioType clickAudio;

        private AudioController AudioController => AudioController.instance;

        #region Unity Functions
        public void OnSelect(BaseEventData eventData){
            if(hoverAudio == AudioType.None) return;
            AudioController.PlayAudio(hoverAudio);
        }

        public void OnSubmit(BaseEventData eventData){
            if(clickAudio == AudioType.None) return;
            AudioController.PlayAudio(clickAudio);
        }
        #endregion

        #region Public Functions

        #endregion

        #region Private Functions

        #endregion
    }

}

