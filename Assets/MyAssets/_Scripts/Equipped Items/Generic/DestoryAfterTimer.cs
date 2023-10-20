using UnityEngine;

public class DestoryAfterTimer : MonoBehaviour
{
   [SerializeField] private float timeTillDestory;
   
   private void Start() {
        Destroy(gameObject, timeTillDestory);
   }
}
