using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("FSM AI/FSM AI")]
public class FSMGraph : MonoBehaviour
{
    [SerializeField] private FSMGraphSaveData graphContainer;
}