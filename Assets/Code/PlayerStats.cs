using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int Health, MaxHealth;
    public int Shield, MaxShield;
    public int Keys;

    public static event Action OnDeath;
    public static event Action<int, int> OnHealthChanged;

    private void Start()
    {
        Health = MaxHealth;
        Shield = MaxShield / 2;
        OnHealthChanged?.Invoke(Health, Shield);
    }

    public void Die()
    {
        OnDeath?.Invoke();
    }

}
