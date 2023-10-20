using UnityEngine;

public class HealthDamageTest : MonoBehaviour
{
    [SerializeField] private int damageDealt = 10;

    private void OnCollisionEnter(Collision other) {
        if(!other.gameObject.CompareTag("Player")) return;

        var health = other.gameObject.GetComponent<Health>();
        health.TakeDamage(damageDealt);
    }
}
