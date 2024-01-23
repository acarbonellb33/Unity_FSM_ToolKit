using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHearingCondition", menuName = "States Conditions/Hearing Condition")]
public class HearingCondition : ScriptableObject
{
    private float distanceToPlayer;
    public float hearingRange = 10f;
    
    void Start()
    {
        distanceToPlayer = Vector3.Distance(GameObject.FindWithTag("Player").transform.position, GameObject.FindWithTag("Enemy").transform.position);
    }

    public bool Condition()
    {
        if (distanceToPlayer <= hearingRange)
        {
            return true;
        }
        return false;
    }
}
