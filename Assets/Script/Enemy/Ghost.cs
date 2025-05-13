using UnityEngine;

public class Ghost : Monster {
    
    protected override void Awake() {
        base.Awake();
        
        // 기본 스탯 설정
        monsterName = "Ghost";
        maxHealth = 75f;
        attackDamage = 12f;
        moveSpeed = 3.5f;
        attackSpeed = 1.2f;
        attackRange = 2f;
    }
    
    protected override void Update() {
        base.Update();
    }
    
    protected override void PerformAttack() {
        animator?.SetTrigger("Attack");
        
        // 플레이어에게 데미지
        if (playerTransform != null) {
            PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}