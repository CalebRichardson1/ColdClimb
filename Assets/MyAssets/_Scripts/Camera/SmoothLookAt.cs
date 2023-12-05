using UnityEngine;
using ColdClimb.Global.SaveSystem;
using ColdClimb.Global;

namespace ColdClimb.Camera{

    // Used for our camera to smoothly look at what our hand is pointing to
    public class SmoothLookAt : MonoBehaviour{
        private Vector3 vectOffset;
        [SerializeField] private Transform followPos;
        private float FollowSpeed => PlayerData.playerStats.lookSpeed;

        private PlayerData PlayerData => ResourceLoader.MainPlayerData;

        #region Setup
        private void Start() => vectOffset = transform.position - followPos.position;
        #endregion

        private void Update(){
            transform.SetPositionAndRotation(followPos.position + vectOffset, 
                                            Quaternion.Slerp(transform.rotation, followPos.rotation, FollowSpeed * Time.deltaTime));
        }
    }
}

