using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour {
    public GameObject[] initialEnemyPrefabs;
    public int initialEnemyCount = 8;
    public DemonDoll bossPrefab;
    public float bossActivationDelay = 3f;
    public EnemySpawner mainSpawner;
    private int remainingMobCount;
    
    // 스테이지 상태
    private bool isBossActivated = false;
    private bool isInitialWaveActive = false;
    
    private void Start() {
        // 몬스터 사망 이벤트 등록
        Monster.OnMonsterDeath += HandleMonsterDeath;
        
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

        if (initialEnemyPrefabs == null || initialEnemyPrefabs.Length == 0) {
            Debug.LogError("No initial enemy prefabs assigned!");
            return;
        }

        // 초기 웨이브 시작
        remainingMobCount = initialEnemyCount;
        StartInitialWave();
    }
    
    private void OnDisable()  => Monster.OnMonsterDeath -= HandleMonsterDeath;
    
    private void OnDestroy() => Monster.OnMonsterDeath -= HandleMonsterDeath; // 몬스터 사망 이벤트 해제

    // 초기 웨이브 시작
    private void StartInitialWave() {
        isInitialWaveActive = true;
        mainSpawner.SpawnWave(initialEnemyPrefabs, initialEnemyCount);
    }
    
    // 몬스터 사망 처리
    private void HandleMonsterDeath(Monster monster) {
        // DemonDoll 는 일반 몹 카운트에 포함하지 않음
        if (monster is not DemonDoll && isInitialWaveActive) // 보스 활성화 조건 체크
        {
            remainingMobCount--;
            if (remainingMobCount <= 0) {
                StartCoroutine(ActivateBossWithDelay());
            }
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