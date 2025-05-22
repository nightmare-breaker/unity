using UnityEngine;

public class Bat : Monster {
    public float flyHeight = 2f; // 비행 높이
    public float hoverRadius = 3f; // 호버링 반경
    private Vector3 hoverPoint; // 호버링 포인트
    private float hoverTime; // 호버링 시간
    
    protected override void Awake() {
        base.Awake();

        // Bat 스탯 설정
        monsterName = "Bat";
        attackDamage = 10f;
        moveSpeed = 4f;
        attackSpeed = 1.5f;
        attackRange = 1.8f;
    }
    
    protected override void Start() {
        base.Start();
        navMeshAgent.baseOffset = flyHeight; // NavMeshAgent가 y축 높이를 무시하도록 설정
    }
    
    protected override void Update() {
        base.Update();
        
        // 박쥐는 항상 일정 높이를 유지
        if (currentState != MonsterState.Die) {
            Vector3 position = transform.position;
            position.y = Mathf.Max(position.y, flyHeight);
            transform.position = position;
        }
    }
    
    protected override void UpdateChaseState() {
        if (playerTransform == null) return;
        
        // 호버링 패턴
        hoverTime += Time.deltaTime;
        
        // 새로운 호버 포인트 계산
        if (hoverTime >= 2f) {
            CalculateNewHoverPoint();
            hoverTime = 0f;
        }
        
        // 호버 포인트로 이동
        Vector3 targetPosition = hoverPoint;
        targetPosition.y = flyHeight;
        navMeshAgent.SetDestination(targetPosition);
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        if (distanceToPlayer > detectionRange * 1.5f) {
            ChangeState(MonsterState.Idle);
            return;
        }
        
        if (distanceToPlayer <= attackRange) {
            ChangeState(MonsterState.Attack);
        }
    }
    
    private void CalculateNewHoverPoint() {
        if (playerTransform != null)
        {
            Vector2 randomCircle = Random.insideUnitCircle * hoverRadius;
            hoverPoint = playerTransform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        }
    }
    
    protected override void PerformAttack() {
        // animator?.SetTrigger("Attack");
        
        // 플레이어에게 데미지
        if (playerTransform != null) {
            if (playerTransform.TryGetComponent<PlayerHealth>(out var playerHealth)) {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}