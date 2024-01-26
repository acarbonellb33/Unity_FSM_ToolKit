using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
	[Header("Patrol")]
	[SerializeField]
	private System.Single patrolSpeed = 423423;
	[SerializeField]
	private System.Int32 patrolRadius = 234;

	[Header("Hearing")]
	[SerializeField]
	private System.Single hearingRange = 4234324;

	[Header("Attack")]
	[SerializeField]
	private System.Single attackDamage = 324234;
	[SerializeField]
	private System.Single attackRange = 4324;
	[SerializeField]
	private System.Single attackCooldown = 4243;
	[SerializeField]
	private System.Boolean canAttack = true;

}
