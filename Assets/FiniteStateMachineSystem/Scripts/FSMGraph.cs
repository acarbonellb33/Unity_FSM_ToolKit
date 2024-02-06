using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("FSM AI/FSM AI")]
public class FSMGraph : MonoBehaviour
{
    /* Dialogue Scriptable Objects */
    [SerializeField] private FSMGraphSaveData graphContainer;
    [SerializeField] private FSMNodeGroupSO graphGroup;
    [SerializeField] private FSMNodeSO graph;
    
    [SerializeField] private int selectedOptionIndex = 0;
    /* Filters */
    [SerializeField] private bool groupedStates;

    /* Indexes */
    [SerializeField] private int selectedStatesGroupIndex;
    [SerializeField] private int selectedStateIndex;
}