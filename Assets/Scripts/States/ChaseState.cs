using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NewChaseState", menuName = "Enemy States/Chase State")]
public class ChaseState : State
{
    private ChaseState()
    {
        SetStateName("Chase");
    }

    public override void Execute()
    {
        agent.SetDestination(player.transform.position);
    }
}
