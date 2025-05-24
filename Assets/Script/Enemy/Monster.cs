using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour {
    public string monsterName;
    public float maxHealth = 100f;
    public float currentHealth;
    public float attackDamage = 10f;
    public float attackSpeed = 1f;
    public float moveSpeed = 3f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    
    // 몬스터 상태
    public enum MonsterState { Idle, Chase, Attack, Hit, Die }
    public MonsterState currentState;
    
    // 필요한 컴포넌트
    protected Animator animator;
    protected NavMeshAgent navMeshAgent;
    [SerializeField] protected bool canMove = true;
    protected Transform playerTransform;
    
    // 타이머 변수
    protected float lastAttackTime;
    
    // 이벤트 시스템
    public delegate void MonsterEvent(Monster monster);
    public static event MonsterEvent OnMonsterDeath;

    protected virtual void Awake() {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
    }

    protected virtual void Start() {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Player 태그를 가진 오브젝트 찾기
        navMeshAgent.speed = moveSpeed; 
        ChangeState(MonsterState.Idle);
    }

    protected virtual void Update() {
        if (!canMove) return;
        if (navMeshAgent == null) return;

        if (currentState == MonsterState.Die) return;
            
        switch (currentState) {
            case MonsterState.Idle:
                CheckPlayerInRange();
                break;
            case MonsterState.Chase:
                UpdateChaseState();
                break;
            case MonsterState.Attack:
                UpdateAttackState();
                break;
        }
    }
    
    protected virtual void CheckPlayerInRange() {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectionRange) {
            ChangeState(MonsterState.Chase);
        }
    }
    
    protected virtual void UpdateChaseState() {
        if (playerTransform == null) return;
            
        navMeshAgent.SetDestination(playerTransform.position);
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        if (distanceToPlayer > detectionRange * 1.5f) {
            // 플레이어가 탐지 범위를 벗어났을 때
            ChangeState(MonsterState.Idle);
            return;
        }
        
        if (distanceToPlayer <= attackRange) {
            ChangeState(MonsterState.Attack);
        }
    }

    protected virtual void UpdateAttackState() {
        if (playerTransform == null) return;
            
        // 플레이어를 바라보도록 회전
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        if (distanceToPlayer > attackRange) {
            ChangeState(MonsterState.Chase);
            return;
        }
        
        // 공격 쿨다운 체크
        if (Time.time >= lastAttackTime + (1f / attackSpeed)) {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    // 상태 변경 함수
    public virtual void ChangeState(MonsterState newState) {
        currentState = newState;
        
        switch (newState) {
            case MonsterState.Idle:
                // animator?.SetTrigger("Idle");
                navMeshAgent.isStopped = true;
                break;
                
            case MonsterState.Chase:
                // animator?.SetTrigger("Run");
                navMeshAgent.isStopped = false;
                break;
                
            case MonsterState.Attack:
                // animator?.SetTrigger("Attack");
                navMeshAgent.isStopped = true;
                break;
                
            case MonsterState.Hit:
                // animator?.SetTrigger("Hit");
                navMeshAgent.isStopped = true;
                break;
                
            case MonsterState.Die:
                // animator?.SetTrigger("Die");
                navMeshAgent.isStopped = true;
                // 콜라이더 비활성화
                if (GetComponent<Collider>()) {
                    GetComponent<Collider>().enabled = false;
                }
                
                // NavMeshAgent 비활성화
                navMeshAgent.enabled = false;
                
                // 몬스터 사망 이벤트 호출
                OnMonsterDeath?.Invoke(this);
                
                // 몬스터 제거 예약
                Destroy(gameObject, 3f);
                break;
        }
    }

    // 공격 수행 (자식 클래스에서 구현)
    protected abstract void PerformAttack();

    // 데미지 받기
    public virtual void TakeDamage(float damage) {
        if (currentState == MonsterState.Die) return;

        currentHealth -= damage;
        
        // 사망 체크
        if (currentHealth <= 0) {
            currentHealth = 0;
            ChangeState(MonsterState.Die);
            return;
        }
        
        // 일시적으로 히트 상태로 변경
        StartCoroutine(HitRoutine());
    }
    
    // 히트 상태 처리
    protected System.Collections.IEnumerator HitRoutine() {
        MonsterState previousState = currentState;
        ChangeState(MonsterState.Hit);
        
        yield return new WaitForSeconds(0.5f);
        
        if (currentState != MonsterState.Die) {
            ChangeState(previousState);
        }
    }
}