namespace Demo.scripts
{
    using System.Collections;
    using FSM.Enemy;
    using UnityEngine;
    public class BulletProjectile : MonoBehaviour
    {
        private Rigidbody _bulletRigidbody;

        [SerializeField] private Transform vfxHitOrange;

        private void Awake()
        {
            _bulletRigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            float speed = 60f;
            _bulletRigidbody.velocity = transform.forward * speed;
            StartCoroutine(DestroyBulletAfterTime(5f));
        }

        private void OnTriggerEnter(Collider other)
        {
            Instantiate(vfxHitOrange, transform.position, transform.rotation);
            if (other.tag == "Enemy")
            {
                other.GetComponent<EnemyHealthSystem>().TakeDamage(10f);
            }

            Destroy(gameObject);
        }

        private IEnumerator DestroyBulletAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            Destroy(gameObject);
        }
    }
}