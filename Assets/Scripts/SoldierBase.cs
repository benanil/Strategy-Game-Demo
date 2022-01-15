using UnityEngine;

namespace PanteonGames
{
    // I would like to create these with scriptable objects
    // because its more data eficient (single data multiple objects)
    // but for senior developer asignment I'm using classes
    public class SoldierBase
    {
        public float HealthPoint;
        public float AttackDamage = 2.5f;
        public virtual void TakeDamage(float damage)
        {
            HealthPoint -= damage;
        }
    }

    public class Soldier0 : SoldierBase
    {
        public Soldier0() {
            AttackDamage = 10;
        }
    }

    public class Soldier1 : SoldierBase
    {
        public Soldier1() {
            AttackDamage = 5;        
        }
    }

    public class Soldier2 : SoldierBase
    {
        public Soldier2() {
            AttackDamage = 2;
        }
    }

    public class SoldierManager : MonoBehaviour
    {
        
    }
}
