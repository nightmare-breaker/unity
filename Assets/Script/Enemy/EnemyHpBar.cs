using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    public Slider hpSlider;
    public Transform enemy;
    public float maxHp = 1000f;
    public float currentHp = 1000f;

    private void Update()
    {
        transform.position = enemy.position;
        hpSlider.value = currentHp / maxHp;
    }
}
