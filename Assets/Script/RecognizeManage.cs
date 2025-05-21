using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RecognizeManage : MonoBehaviour
{
    public GameObject recognizefailText;
    public float duration1 = 0.5f;
    private float timer1 = 0f;
    private bool isFailed = false;
    public GameObject recognizesucessText;
    public Text recognizedText;
    public float duration2 = 0.5f;
    private float timer2 = 0f;
    private bool isRecognized = false;
    public GameObject timeoutText;
    public float duration3 = 0.5f;
    private float timer3 = 0f;
    private bool isTimeout = false;

    void Update()
    {
        if (isFailed)
        {
            timer1 -= Time.deltaTime;
            if (timer1 <= 0f)
            {
                recognizefailText.SetActive(false);
                isFailed = false;
            }
        }
        if (isRecognized)
        {
            timer2 -= Time.deltaTime;
            if (timer2 <= 0f)
            {
                recognizesucessText.SetActive(false);
                isRecognized = false;
            }
        }
        if (isTimeout)
        { 
            timer3 -= Time.deltaTime;
            if (timer3 <= 0f)
            {
                timeoutText.SetActive(false);
                isTimeout = false;
            }
        }
    }
    public void RecognizeFail()
    {
        recognizefailText.SetActive(true);
        timer1 = duration1;
        isFailed = true;
    }

    public void Recognized()
    {
        recognizesucessText.SetActive(true);
        timer2 = duration2;
        isRecognized = true;
    }

    public void Timeout()
    { 
        timeoutText.SetActive(true);
        timer3 = duration3;
        isTimeout = true;
    }
}
