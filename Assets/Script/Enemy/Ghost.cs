using UnityEngine;

public class Ghost : Monster {
    
    protected override void Awake() {
        base.Awake();
        
        // Ghost 스탯 설정
        monsterName = "Ghost";
        attackDamage = 12f;
        moveSpeed = 3.5f;
        attackSpeed = 1.2f;
        attackRange = 2f;
    }
    
    protected override void Update() {
        base.Update();
    }
    
    protected override void PerformAttack() {
        // animator?.SetTrigger("Attack");
        
        // 일반 공격
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) <= attackRange) {
            
            if (playerTransform.TryGetComponent<PlayerHealth>(out var playerHealth)) {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}