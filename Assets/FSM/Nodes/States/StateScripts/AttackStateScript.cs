using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateScript : StateScript, IAction
{
    public float attackDamage = 10f;
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public bool canAttack = true;

    public AttackStateScript()
    {
        SetStateName("Attack");
    }
    
    public void Execute(){
        Debug.Log("Attacking!");
    }
}
