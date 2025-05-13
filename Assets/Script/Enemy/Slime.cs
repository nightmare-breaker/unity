using UnityEngine;

public class Slime : Monster {
    
    protected override void Awake() {
        base.Awake();
        
        // 슬라임 기본 스탯 설정
        monsterName = "Slime";
        maxHealth = 60f;
        attackDamage = 8f;
        moveSpeed = 2.5f;
        attackSpeed = 0.8f;
        attackRange = 1.5f;
    }
    
    protected override void PerformAttack() {
        animator?.SetTrigger("Attack");
        // 일반 공격
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) <= attackRange) {
            PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}
