using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    public int maxHp = 100;
    public int Current { get; private set; }

    private void Awake()
    {
        Current = maxHp;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(Current, maxHp);
    }
    public void TakeDamage(int amount)
    {
        if(Current <= 0)
        {
            return;
        }

        Current = Mathf.Clamp(Current - amount, 0, maxHp);

        OnHealthChanged?.Invoke(Current, maxHp);
        if(Current == 0)
        {
            OnDied?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (Current <= 0)
        {
            return;
        }

        Current = Mathf.Clamp(Current + amount, 0, maxHp);

        OnHealthChanged?.Invoke(Current, maxHp);
    }
}