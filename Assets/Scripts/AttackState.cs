using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackState", menuName = "Enemy States/Attack State")]
public class AttackState : ScriptableObject
{
    public float attackDamage = 10f;
    public float attackRange = 5f;
    public float attackCooldown = 2f;

    public void Execute(){
        Debug.Log("Attacking!");
    }
}

