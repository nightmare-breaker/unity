using UnityEngine;

public class Slime : Monster {
    
    protected override void Awake() {
        base.Awake();
        
        // Slime 스탯 설정
        monsterName = "Slime";
        attackDamage = 8f;
        moveSpeed = 2.5f;
        attackSpeed = 0.8f;
        attackRange = 1.5f;
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
