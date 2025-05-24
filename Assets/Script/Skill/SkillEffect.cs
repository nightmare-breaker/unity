using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    public SkillType skillType;
    public PlayerStats playerStats;

    void Start()
    {
        // 효과 발동 (예시: 스폰 시 즉시 발동, 충돌 시 발동 등)
        ApplyEffect();
    }  

    public void ApplyEffect()
    {
        switch (skillType)
        {
            case SkillType.Heart:
                if (playerStats.UseMana(10))
                    playerStats.Heal(20);
                break;
            case SkillType.Triangle:
                // 범위 딜: Collider 범위 안의 몬스터에게 50 데미지
                if (playerStats.UseMana(10))
                    DealAreaDamage(50f, 3f); // 데미지, 반경
                break;
            case SkillType.Square:
                if (playerStats.UseMana(10))
                    playerStats.ActivateShield(5f); // 5초간 실드
                break;
            case SkillType.Circle:
                if (playerStats.UseMana(10))
                    DealSingleTargetDamage(100);
                break;
            case SkillType.ZShape:
                playerStats.RecoverMana(40);
                break;
            case SkillType.Spiral:
                if (playerStats.UseMana(40))
                    DealAreaDamage(1000f, 5f); // 마나 8 소모, 범위 5
                break;
        }
    }

    void DealAreaDamage(float damage, float radius)
    {
        // Collider[] targets = Physics.OverlapSphere(transform.position, radius);
        // foreach (var target in targets)
        // {
        //     EnemyStats enemy = target.GetComponent<EnemyStats>();
        //     if (enemy != null)
        //     {
        //         enemy.TakeDamage((int)damage);
        //     }
        // }
    }

    void DealSingleTargetDamage(int damage)
    {
        // Ray ray = new Ray(transform.position, transform.forward);
        // if (Physics.Raycast(ray, out RaycastHit hit, 10f))
        // {
        //     EnemyStats enemy = hit.collider.GetComponent<EnemyStats>();
        //     if (enemy != null)
        //     {
        //         enemy.TakeDamage(damage);
        //     }
        // }
    }
}

