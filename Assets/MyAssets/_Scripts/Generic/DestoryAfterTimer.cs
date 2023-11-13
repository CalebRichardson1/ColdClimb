using UnityEngine;

namespace ColdClimb.Generic{

   public class DestoryAfterTimer : MonoBehaviour{
      [SerializeField] private float timeTillDestory;
      
      private void Start(){
         Destroy(gameObject, timeTillDestory);
      }
   }
}

