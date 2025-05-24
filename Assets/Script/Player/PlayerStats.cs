using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // 최대 체력 및 마나 설정
    public int maxHealth = 100;
    public int currentHealth;
    public int maxMana = 100;
    public int currentMana;
    public bool isShieldActive = false;

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    // 체력 감소
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("플레이어 체력: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 마나 사용
    public bool UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            Debug.Log("플레이어 마나: " + currentMana);
            return true;
        }
        else
        {
            Debug.Log("마나가 부족합니다.");
            return false;
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log("체력 회복됨: " + currentHealth);
    }

    public void RecoverMana(int amount)
    {
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
        Debug.Log("마나 회복됨: " + currentMana);
    }

    public void ActivateShield(float duration)
    {
        StartCoroutine(ShieldCoroutine(duration));
    }

    private IEnumerator ShieldCoroutine(float duration)
    {
        // 예시: bool isShieldActive 사용
        Debug.Log("실드 활성화됨");
        // isShieldActive = true;
        yield return new WaitForSeconds(duration);
        // isShieldActive = false;
        Debug.Log("실드 종료됨");
    }

    void Die()
    {
        // 여기에 사망 처리 로직 추가
    }
}

