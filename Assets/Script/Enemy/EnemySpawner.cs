using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public float spawnRadius = 10f; // 스폰 반경
    public int maxEnemiesAtOnce = 5; // 최대 동시 출현 몬스터 수
    public float spawnInterval = 1f; // 스폰 간격
    
    // 웨이브 관련 변수
    private List<GameObject> currentWaveEnemies = new List<GameObject>();
    private bool isWaveActive = false; // 현재 웨이브 활성화 상태
    private System.Action onWaveDefeated; // 웨이브 처치 콜백
    
    private void Awake() {
        // 스폰 포인트가 설정되지 않았으면 자신의 위치 사용
        if (spawnPoints == null || spawnPoints.Length == 0) {
            spawnPoints = new Transform[1] { transform };
        }
    }
    
    private void OnEnable() {
        // 몬스터 사망 이벤트 등록
        Monster.OnMonsterDeath += HandleMonsterDeath;
    }
    
    private void OnDisable() {
        // 몬스터 사망 이벤트 해제
        Monster.OnMonsterDeath -= HandleMonsterDeath;
    }
    
    // 몬스터 사망 처리
    private void HandleMonsterDeath(Monster monster) { 
        // 현재 웨이브에서 사망한 몬스터 제거
        if (currentWaveEnemies.Contains(monster.gameObject)) {
            currentWaveEnemies.Remove(monster.gameObject);
            CheckWaveStatus(); // 웨이브의 모든 몬스터가 처치되었는지 확인
        }
    }
    
    // 웨이브 상태 확인
    private void CheckWaveStatus() {
        // 리스트에서 null 참조 제거
        currentWaveEnemies.RemoveAll(enemy => enemy == null);
        
        // 웨이브의 모든 몬스터가 처치되었다면
        if (currentWaveEnemies.Count == 0 && isWaveActive) {
            isWaveActive = false;
            
            // 웨이브 처치 콜백 호출
            if (onWaveDefeated != null) {
                onWaveDefeated.Invoke();
            }
        }
    }
    
    // 웨이브 스폰
    public void SpawnWave(GameObject[] prefabs, int count, System.Action onDefeated = null) {
        // 이전 웨이브가 활성화 상태면 중단
        if (isWaveActive) return;

        // 웨이브 처치 콜백 저장
        onWaveDefeated = onDefeated;
        
        // 새 웨이브 시작
        isWaveActive = true;
        currentWaveEnemies.Clear();
        
        // 스폰할 프리팹 설정
        GameObject[] spawnPrefabs = prefabs;
        if (spawnPrefabs == null || spawnPrefabs.Length == 0) {
            spawnPrefabs = enemyPrefabs;
        }
        
        // 적 스폰 시작
        StartCoroutine(SpawnEnemiesRoutine(spawnPrefabs, count));
    }
    
    // 웨이브 활성화 상태 반환
    public bool IsWaveActive() {
        return isWaveActive;
    }
    
    // 적 스폰 코루틴
    private IEnumerator SpawnEnemiesRoutine(GameObject[] prefabs, int count) {
        int spawned = 0;
        
        while (spawned < count) {
            // 최대 동시 출현 몬스터 수 제한
            if (currentWaveEnemies.Count < maxEnemiesAtOnce) {
                // 랜덤 적 프리팹 선택
                GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                
                // 랜덤 스폰 위치 선택
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Vector3 spawnPosition = GetSpawnPosition(spawnPoint.position);
                
                // 적 생성
                GameObject enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);
                currentWaveEnemies.Add(enemy);
                
                spawned++;
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    // 유효한 스폰 위치 찾기
    private Vector3 GetSpawnPosition(Vector3 center) {
        Vector3 spawnPos;
        int attempts = 0;
        const int maxAttempts = 30;
        
        do {
            // 랜덤 위치 계산
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            spawnPos = center + new Vector3(randomCircle.x, 0, randomCircle.y);
            
            // NavMesh에 유효한 위치인지 확인
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(spawnPos, out hit, 5f, UnityEngine.AI.NavMesh.AllAreas)) {
                spawnPos = hit.position;
                break;
            }
            
            attempts++;
        } while (attempts < maxAttempts);
        
        return spawnPos;
    }
    
    // 디버그용 범위 표시
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        
        if (spawnPoints != null && spawnPoints.Length > 0) {
            foreach (Transform point in spawnPoints) {
                if (point != null) {
                    Gizmos.DrawWireSphere(point.position, spawnRadius);
                }
            }
        } else {
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}