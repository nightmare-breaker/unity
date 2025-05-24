using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    public float speed = 8f;

    void Start() {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void Update() {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        // 방향을 transform 기준으로 바꿔줌 (로컬 좌표 기준 이동)
        Vector3 moveDirection = transform.right * xInput + transform.forward * zInput;
        moveDirection.y = 0f; // y축은 건들지 않음

        playerRigidbody.linearVelocity = moveDirection.normalized * speed;
    }
}

