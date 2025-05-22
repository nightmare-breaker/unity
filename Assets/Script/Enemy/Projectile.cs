using UnityEngine;

public class Projectile : MonoBehaviour {
    public float speed = 15f; // 투사체 속도
    public float damage = 20f; // 투사체 데미지
    public float lifetime = 5f; // 투사체 생명주기
    
    private Rigidbody rb;
    
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    
    private void Start() {
        Destroy(gameObject, lifetime); // 일정 시간 후 파괴
    }
    
    public void Launch(Vector3 direction, float damageAmount) {
        damage = damageAmount;
        rb.linearVelocity = direction.normalized * speed; // 방향 설정 및 속도 적용
        transform.forward = direction.normalized; // 진행 방향으로 회전
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            // 플레이어와 충돌했을 때
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null) {
                playerHealth.TakeDamage(damage);
            }
            
            Destroy(gameObject); // 투사체 파괴
            
        } else if (other.CompareTag("Ground") || other.CompareTag("Wall")) {
            // 바닥이나 벽과 충돌했을 때
            Destroy(gameObject); // 투사체 파괴
        }
    }
}