using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private Slider slider;
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private Transform target;
    void Awake()
    {
        slider = GetComponent<Slider>();
    }
    
    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        slider.value = currentHealth / maxHealth;
    }

    private void Update()
    {
        transform.rotation = playerCamera.transform.rotation;
        transform.position = target.position + new Vector3(0, 2, 0);
    }
}
