// 임시 파일로 사용 중
// 합치는 과정에서 따로 제작하신 Player로 대체해야 함


using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float invincibilityTime = 1f;
    
    [Header("UI")]
    public Slider healthSlider;
    public Image damageImage;
    public float flashSpeed = 5f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.3f);
    
    // 컴포넌트 참조
    private Animator animator;
    private AudioSource audioSource;
    private bool isDead = false;
    private bool isInvincible = false;
    
    // 효과음
    public AudioClip hurtSound;
    public AudioClip deathSound;
    
    private void Awake() {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        currentHealth = maxHealth;
    }
    
    private void Start() {
        // UI 초기화
        UpdateHealthUI();
    }
    
    private void Update() {
        // 데미지 이미지 페이드 아웃
        if (damageImage != null)
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
    }
    
    public void TakeDamage(float damage) {
        // 사망 상태거나 무적 상태면 데미지 없음
        if (isDead || isInvincible) return;

        // 무적 시간 시작
        StartCoroutine(Invincibility());
        
        // 데미지 적용
        currentHealth -= damage;
        
        // UI 업데이트
        UpdateHealthUI();
        
        // 데미지 이미지 플래시
        if (damageImage != null) {
            damageImage.color = flashColor;
        }
        
        // 사운드 재생
        if (hurtSound != null) {
            audioSource.PlayOneShot(hurtSound);
        }
        
        // 사망 체크
        if (currentHealth <= 0) {
            Death();
        } else {
            // 피격 애니메이션
            animator?.SetTrigger("Hit");
        }
    }
    
    // 체력 회복
    public void Heal(float amount) {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }
    
    // 사망 처리
    private void Death() {
        if (isDead) return;
            
        isDead = true;
        currentHealth = 0;
        
        // UI 업데이트
        UpdateHealthUI();
        
        // 사망 애니메이션
        animator?.SetTrigger("Die");
        
        // 사망 사운드
        if (deathSound != null) {
            audioSource.PlayOneShot(deathSound);
        }
        
        // 콜라이더 비활성화
        Collider playerCollider = GetComponent<Collider>();
        if (playerCollider != null) {
            playerCollider.enabled = false;
        }
        
        // 이동 컴포넌트 비활성화
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts) {
            if (script != this && script.enabled) {
                script.enabled = false;
            }
        }
        
        // 게임 매니저에 사망 알림
        // 예: GameManager.Instance.OnPlayerDeath();
    }
    
    // 무적 시간
    private IEnumerator Invincibility() {
        isInvincible = true;
        
        // 플레이어 깜빡임 효과 (선택 사항)
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float blinkInterval = 0.1f;
        
        for (float i = 0; i < invincibilityTime; i += blinkInterval) {
            foreach (Renderer renderer in renderers) {
                renderer.enabled = !renderer.enabled;
            }
            
            yield return new WaitForSeconds(blinkInterval);
        }
        
        // 렌더러 상태 복원
        foreach (Renderer renderer in renderers) {
            renderer.enabled = true;
        }
        
        isInvincible = false;
    }
    
    // UI 업데이트
    private void UpdateHealthUI() {
        if (healthSlider != null) {
            healthSlider.value = currentHealth / maxHealth;
        }
    }
}