using UnityEngine;

public class ExempleJudith : MonoBehaviour
{
    public int MaxHealth = 100;
    int health;
    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            health = Mathf.Clamp(value, 0, MaxHealth);
            if (health <= 0)
            {
                Die();
            }
        }
    }

    private void Awake()
    {
        Debug.Log(Health);
        Health = 1000;
    }

    void Daamage(int dmg)
    {
        Health -= dmg;
    }

    void Die()
    {
         
    }
}
