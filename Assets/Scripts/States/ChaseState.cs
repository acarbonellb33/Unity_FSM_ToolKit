using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NewChaseState", menuName = "Enemy States/Chase State")]
public class ChaseState : State
{
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Execute()
    {
        NavMeshAgent agent = GameObject.Find("Enemy").GetComponent<NavMeshAgent>();
        agent.SetDestination(player.transform.position);
    }
}
