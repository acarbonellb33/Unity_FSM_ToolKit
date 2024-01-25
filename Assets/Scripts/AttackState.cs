using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackState", menuName = "Enemy States/Attack State")]
public class AttackState : State
{
    public float attackDamage = 10f;
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public bool canAttack = true;

    private AttackState()
    {
         SetStateName("Attack");
    }
    
    public override void Execute(){
        Debug.Log("Attacking!");
    }
}

