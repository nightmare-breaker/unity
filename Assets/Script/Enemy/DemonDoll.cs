using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DemonDoll : Monster {
    public float throwAttackRange = 10f; // 투척 공격 범위
    public float throwCooldown = 3f; // 투척 공격 쿨타임
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint; // 투척 공격 발사 위치
    public int maxWaves = 3;
    public int monsterPerWave = 5;
    
    // 그로기 상태 관련 변수
    public float groggyDuration = 8f;
    public bool isGroggy = false;
    
    // 웨이브 관련 변수
    private EnemySpawner spawner;
    private int currentWave = 0;
    private float lastThrowTime;
    private bool finalPhaseStarted = false;   // 마지막 패턴이 이미 발동했는지
    
    // 상태 관련
    public enum BossPhase { Phase1, Phase2, Phase3 }
    public BossPhase currentPhase = BossPhase.Phase1;
    
    protected override void Awake() {
        base.Awake();
        
        //기본 스탯 설정
        monsterName = "Demon Doll";
        maxHealth = 500f;
        attackDamage = 20f;
        moveSpeed = 2.5f;
        attackSpeed = 0.7f;
        attackRange = 3f;
    }
    
    protected override void Start() {
        base.Start();
        
        // 보스 전용 스포너 초기화
        spawner = GetComponent<EnemySpawner>();
        if (spawner == null) {
            // 컴포넌트가 없으면 추가
            spawner = gameObject.AddComponent<EnemySpawner>();
        }
        
        // 첫 번째 웨이브 시작
        StartNextWave();
        
        // 보스의 체력에 따라 페이즈 변경 이벤트 등록
        InvokeRepeating("CheckPhaseChange", 1f, 1f);
    }
    
    protected override void Update() {
        base.Update();
        
        // 그로기 상태일 때는 아무것도 하지 않음
        if (isGroggy) return;
        
        // 웨이브가 살아있는 동안 투척 공격
        if (spawner.IsWaveActive() && Time.time >= lastThrowTime + throwCooldown) {
            if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) <= throwAttackRange) {
                PerformThrowAttack();
                lastThrowTime = Time.time;
            }
        }
    }
    
    // 다음 웨이브 시작
    public void StartNextWave() {
        // 모든 웨이브를 소모한 뒤면 바로 빠져나옴
        if (currentWave >= maxWaves) {
            Debug.Log("All waves cleared. Switching to final phase.");
            BeginFinalPhase();   // ← 전용 메서드 호출
            return;
            
        } else if (currentWave < maxWaves) {
            currentWave++;

            // 페이즈에 따라 웨이브의 몬스터 종류/수 조정
            List<GameObject> wavePrefabs = new List<GameObject>();
            int monstersCount = monsterPerWave;

            switch (currentPhase)
            {
                case BossPhase.Phase1:
                    // 첫 페이즈는 슬라임만
                    monstersCount = Mathf.RoundToInt(monsterPerWave * 1.0f);
                    wavePrefabs.Add(Resources.Load<GameObject>("Prefabs/Monsters/Slime"));
                    break;

                case BossPhase.Phase2:
                    // 두 번째 페이즈는 슬라임과 박쥐
                    monstersCount = Mathf.RoundToInt(monsterPerWave * 1.2f);
                    wavePrefabs.Add(Resources.Load<GameObject>("Prefabs/Monsters/Slime"));
                    wavePrefabs.Add(Resources.Load<GameObject>("Prefabs/Monsters/Bat"));
                    break;

                case BossPhase.Phase3:
                    // 세 번째 페이즈는 모든 몬스터
                    monstersCount = Mathf.RoundToInt(monsterPerWave * 1.5f);
                    wavePrefabs.Add(Resources.Load<GameObject>("Prefabs/Monsters/Slime"));
                    wavePrefabs.Add(Resources.Load<GameObject>("Prefabs/Monsters/Bat"));
                    wavePrefabs.Add(Resources.Load<GameObject>("Prefabs/Monsters/Ghost"));
                    break;
            }

            // 웨이브 생성
            spawner.SpawnWave(wavePrefabs.ToArray(), monstersCount, OnWaveDefeated);
        }
    }
    
    // 웨이브 처치 시 호출될 콜백
    private void OnWaveDefeated() {
        // 그로기 상태로 전환
        StartCoroutine(GroggyRoutine());
    }
    
    // 그로기 상태 처리 루틴
    private IEnumerator GroggyRoutine() {
        // 그로기 상태로 전환
        isGroggy = true;
        // animator?.SetBool("Groggy", true);
        navMeshAgent.isStopped = true;
        
        Debug.Log($"Boss entered groggy state for {groggyDuration} seconds");
        
        // 그로기 상태 지속
        yield return new WaitForSeconds(groggyDuration);
        
        // 그로기 상태 해제
        isGroggy = false;
        // animator?.SetBool("Groggy", false);
        navMeshAgent.isStopped = false;
        
        Debug.Log("Boss recovered from groggy state");
        // animator?.SetBool("Groggy", false);
        if (currentWave >= maxWaves) {
            BeginFinalPhase();      // 더 소환할 웨이브가 없으면 최종 패턴
        } else {
            StartNextWave();        // 아직 남았으면 다음 웨이브
        }
    }

    private void BeginFinalPhase() {
        // 이미 한 번 발동했다면 재호출 차단
        if (finalPhaseStarted) return;
        finalPhaseStarted = true;

        // 스포너 정지하고 보스랑 1:1 대결
        if (spawner != null) spawner.enabled = false;

        // 보스 능력치 조정?
        currentPhase = BossPhase.Phase3;   // 확실히 마지막 페이즈로 고정
        attackDamage *= 2f;
        attackSpeed  *= 1.5f;
        throwCooldown *= 0.5f;

    Debug.Log("Boss has entered the FINAL phase!");
}
    
    // 페이즈 체크
    private void CheckPhaseChange()
    {
        float healthPercentage = currentHealth / maxHealth;

        if (healthPercentage <= 0.3f && currentPhase != BossPhase.Phase3)
        {
            currentPhase = BossPhase.Phase3;
            OnPhaseChange();
        }
        else if (healthPercentage <= 0.6f && currentPhase != BossPhase.Phase3 && currentPhase != BossPhase.Phase2)
        {
            currentPhase = BossPhase.Phase2;
            OnPhaseChange();
        }
    }
    
    // 페이즈 변경 시 호출
    private void OnPhaseChange() {
        // 페이즈에 따른 보스 강화
        switch (currentPhase) {
            case BossPhase.Phase2:
                attackDamage *= 1.2f;
                attackSpeed *= 1.2f;
                throwCooldown *= 0.8f;
                //animator?.SetTrigger("PhaseChange");
                break;
                
            case BossPhase.Phase3:
                attackDamage *= 1.5f;
                attackSpeed *= 1.3f;
                throwCooldown *= 0.6f;
                //animator?.SetTrigger("PhaseChange");
                break;
        }
        
        Debug.Log($"Boss entered Phase {(int)currentPhase + 1}");
    }
    
    // 기본 공격 수행
    protected override void PerformAttack() {
        // animator?.SetTrigger("Attack");
        
        // 플레이어에게 데미지
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) <= attackRange) {
            PlayerHealth playerHealth = playerTransform.GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
    
    // 투척 공격 수행
    private void PerformThrowAttack() {
        if (projectilePrefab == null || projectileSpawnPoint == null)
            return;

        // animator?.SetTrigger("Throw");

        // 플레이어 방향으로 투사체 발사
        if (playerTransform != null) {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            
            if (projectileComponent != null) {
                projectileComponent.Launch(playerTransform.position - projectileSpawnPoint.position, attackDamage);
            } else {
                // Projectile 컴포넌트가 없으면 기본 발사 로직
                Rigidbody projRb = projectile.GetComponent<Rigidbody>();

                if (projRb != null) {
                    Vector3 direction = (playerTransform.position - projectileSpawnPoint.position).normalized;
                    projRb.AddForce(direction * 20f, ForceMode.Impulse);

                    // 10초 후 파괴
                    Destroy(projectile, 10f);
                }
            }
        }
    }
    
    // 데미지 처리 오버라이드
    public override void TakeDamage(float damage) {
        // 그로기 상태일 때는 받는 데미지 증가
        float finalDamage = isGroggy ? damage * 2f : damage; // 수치 조정 가능
        
        base.TakeDamage(finalDamage);
    }
}