using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour {
    public GameObject[] initialEnemyPrefabs;
    public int initialEnemyCount = 8;
    public DemonDoll bossPrefab;
    public float bossActivationDelay = 3f;
    public EnemySpawner mainSpawner;
    
    // 스테이지 상태
    private bool isBossActivated = false;
    private bool isInitialWaveActive = false;
    
    private void Start() {
        // 메인 스포너가 없으면 찾기
        if (mainSpawner == null) {
            mainSpawner = GetComponent<EnemySpawner>();
            
            if (mainSpawner == null) {
                mainSpawner = gameObject.AddComponent<EnemySpawner>();
            }
        }

        // 보스가 할당되지 않았다면 씬에서 찾기
        if (bossPrefab == null) {
            bossPrefab = FindFirstObjectByType<DemonDoll>();
            if (bossPrefab == null) {
                Debug.LogError("Boss prefab is not assigned and no boss found in the scene!");
            }
        }

        // 보스는 비활성화 되어 시작
        if (bossPrefab != null) {
            bossPrefab.gameObject.SetActive(false);
        }
        
        // 초기 웨이브 시작
        StartInitialWave();
        
        // 몬스터 사망 이벤트 등록
        Monster.OnMonsterDeath += HandleMonsterDeath;
    }
    
    private void OnDestroy() {
        // 몬스터 사망 이벤트 해제
        Monster.OnMonsterDeath -= HandleMonsterDeath;
    }
    
    // 초기 웨이브 시작
    private void StartInitialWave() {
        isInitialWaveActive = true;
        mainSpawner.SpawnWave(initialEnemyPrefabs, initialEnemyCount);
    }
    
    // 몬스터 사망 처리
    private void HandleMonsterDeath(Monster monster) {
        CheckBossActivationCondition(); // 보스 활성화 조건 체크
    }
    
    // 보스 스폰 조건 체크
    private void CheckBossActivationCondition() {
        // 이미 보스가 소환되었으면 패스
        if (isBossActivated) return;
        
        // 필드의 모든 몬스터 카운트
        Monster[] activeMonsters = FindObjectsByType<Monster>(FindObjectsSortMode.None);
        int mobCount = 0;
        
        foreach (Monster monster in activeMonsters) {
            if (!(monster is DemonDoll)) {
                mobCount++;
            }
        }
        
        // 남은 몬스터가 없으면 보스 활성화
        if (mobCount == 0 && isInitialWaveActive) {
            isInitialWaveActive = false;
            StartCoroutine(ActivateBossWithDelay());
        }
    }
    
    // 보스 스폰 코루틴
    private IEnumerator ActivateBossWithDelay() {
        Debug.Log("All monsters defeated. Activating boss in " + bossActivationDelay + " seconds...");
        
        // 보스가 활성화 되었음을 알리는 UI 업데이트?
        
        // 딜레이 후 보스 활성화
        yield return new WaitForSeconds(bossActivationDelay);
        
        ActivateBoss();
    }
    
    // 보스 활성화
    private void ActivateBoss() {
        if (bossPrefab != null) {
            bossPrefab.gameObject.SetActive(true);
            
            // 첫 번째 웨이브 시작
            bossPrefab.StartNextWave();
            
            isBossActivated = true;
            Debug.Log("Boss activated!");
        } else {
            Debug.LogError("Boss (DemonDoll) reference is missing!");
        }
    }
    
    // 스테이지 클리어 체크
    public bool IsStageCleared() {
        // 보스 활성화 + 체력 0 이하일 때 스테이지 클리어
        return isBossActivated && (bossPrefab == null || bossPrefab.currentHealth <= 0);
    }
}