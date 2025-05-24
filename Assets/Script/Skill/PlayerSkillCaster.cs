using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillCaster : MonoBehaviour
{
    public GameObject[] skillPrefabs = new GameObject[6]; // 하트~나선형까지
    public Transform firePoint;
    public float skillForce = 20f;

    public PlayerStats playerStats;

    // 마나 소모 정의
    private int[] manaCosts = new int[6] { 10, 10, 10, 10, 0, 40 };

    void Update()
    {
        for (int i = 1; i <= 6; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                CastSkill(i - 1); // 0~5 인덱스
            }
        }
    }

    void CastSkill(int index)
    {
        if (playerStats == null) return;

        int manaCost = manaCosts[index];
        if (manaCost > 0 && !playerStats.UseMana(manaCost))
        {
            Debug.Log("마나 부족");
            return;
        }

        SkillType skillType = (SkillType)index;

        // 발사체 없이 즉시 발동되는 스킬
        if (skillType == SkillType.Heart || 
            skillType == SkillType.Square || 
            skillType == SkillType.ZShape)
        {
            SkillEffect tempEffect = new GameObject("TempEffect").AddComponent<SkillEffect>();
            tempEffect.skillType = skillType;
            tempEffect.playerStats = playerStats;
            tempEffect.ApplyEffect();
            Destroy(tempEffect.gameObject);
            return;
        }

        // 발사체가 필요한 스킬
        if (skillPrefabs[index] == null || firePoint == null) return;

        GameObject skill = Instantiate(skillPrefabs[index], firePoint.position, firePoint.rotation);
        Rigidbody rb = skill.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * skillForce;
        }

        SkillEffect effect = skill.GetComponent<SkillEffect>();
        if (effect != null)
        {
            effect.skillType = skillType;
            effect.playerStats = playerStats;
        }
    }
}
