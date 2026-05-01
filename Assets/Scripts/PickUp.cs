using UnityEngine;

public class PickUp : MonoBehaviour
{
    public int healAmount = 15;
    private void OnTriggerEnter2D(Collider2D other)
    {
        var health = other.GetComponent<Health>();
        if(health.Current >= 100)
        {
            return;
        }

        health.Heal(healAmount);
        Destroy(gameObject);
    }
}
