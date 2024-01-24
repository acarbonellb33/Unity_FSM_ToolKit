using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackState", menuName = "Enemy States/Attack State")]
public class AttackState : EnemyState
{
    public float attackDamage = 10f;
    public float attackRange = 5f;
    public float attackCooldown = 2f;

    public override void Execute(){
        Debug.Log("Attacking!");
    }
}

