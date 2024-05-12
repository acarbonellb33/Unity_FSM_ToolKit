#if UNITY_EDITOR
namespace FSM.Nodes.States.StateScripts
{
    using UnityEngine;
    using Player;
    public class AttackStateScript : StateScript, IAction
    {
        public float attackDamage = 10f;
        public float attackFrequency = 1f;

        private float _lastAttack = 0f;

        public AttackStateScript()
        {
            SetStateName("Attack");
        }

        public void Execute()
        {
            agent.ResetPath();
            RotateEnemyToPlayer();
            if (CanAttack())
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.GetChild(0).position, transform.GetChild(0).transform.forward, out hit,
                        50f))
                {
                    HealthSystem target = hit.transform.GetComponent<HealthSystem>();
                    if (target != null)
                    {
                        target.TakeDamage(attackDamage);
                    }
                }

                _lastAttack = Time.time;
            }
        }

        private void RotateEnemyToPlayer()
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.1f);
        }

        private bool CanAttack()
        {
            return Time.time >= _lastAttack + attackFrequency;
        }
    }
}
#endif