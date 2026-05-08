using UnityEngine;

public class PickUp : MonoBehaviour
{
    [Header("VFX")]
    public GameObject pickupVFXPrefab;

    public int healAmount = 15;
    private void OnTriggerEnter2D(Collider2D other)
    {
        var health = other.GetComponent<Health>();
        if(!other.CompareTag("Player") || health == null || health.Current >= health.maxHp)
        {
            return;
        }

        if (pickupVFXPrefab != null)
        {
            Instantiate(
                pickupVFXPrefab,
                other.transform.position,
                Quaternion.identity);
        }
        health.Heal(healAmount);
        Destroy(gameObject);
    }
}
