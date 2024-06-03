namespace Demo.scripts
{
    using UnityEngine;

    public class Shooting : MonoBehaviour
    {
        [SerializeField] GameObject bulletPrefab;
        [SerializeField] Transform shootingPoint;
        [SerializeField] float shootingCooldown = 1f;
        [SerializeField] Camera playerCamera;

        private float _timeSinceLastShot;

        void Start()
        {
            _timeSinceLastShot = shootingCooldown;
        }

        void Update()
        {
            _timeSinceLastShot += Time.deltaTime;

            if (Input.GetButtonDown("Fire1") && _timeSinceLastShot >= shootingCooldown)
            {
                Shoot();
                _timeSinceLastShot = 0f;
            }
        }

        void Shoot()
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Default")))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(1000);
            }
            
            Vector3 direction = (targetPoint - shootingPoint.position).normalized;

            Instantiate(bulletPrefab, shootingPoint.position, Quaternion.LookRotation(direction));
        }
    }
}