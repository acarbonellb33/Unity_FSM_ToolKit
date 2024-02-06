using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseStateScript : StateScript
{
    public ChaseStateScript()
    {
        SetStateName("Chase");
    }

    public override void Execute()
    {
        agent.SetDestination(player.transform.position);
    }
}
