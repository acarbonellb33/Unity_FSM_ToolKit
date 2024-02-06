using UnityEngine;

public class FSMGraph : MonoBehaviour
{
    /* Dialogue Scriptable Objects */
    [SerializeField] private FSMGraphSaveData graphContainer;
    [SerializeField] private FSMNodeGroupSO graphGroup;
    [SerializeField] private FSMNodeSO graph;

    /* Filters */
    [SerializeField] private bool groupedStates;

    /* Indexes */
    [SerializeField] private int selectedStatesGroupIndex;
    [SerializeField] private int selectedStateIndex;
}