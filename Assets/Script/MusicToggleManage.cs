using UnityEngine;
using UnityEngine.UI;

public class MusicToggleManage : MonoBehaviour
{
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Slider volumeSlider;

    private float previousVolume = 0.5f; // �ʱⰪ 50

    private void Awake()
    {
        // �����̴��� ������ ���� �� ���� �ʱ�ȭ
        if (AudioManage.Instance != null)
        {
            float currentVolume = AudioManage.Instance.GetVolume();
            volumeSlider.value = currentVolume;

            musicToggle.isOn = currentVolume > 0f;

            if (currentVolume > 0f)
                previousVolume = currentVolume;
        }
    }

    private void Start()
    {
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        musicToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnVolumeChanged(float value)
    {
        if (value == 0f)
        {
            musicToggle.isOn = false;
        }
        else
        {
            if (!musicToggle.isOn)
                musicToggle.isOn = true;

            previousVolume = value;
        }

        AudioManage.Instance.SetVolume(value);
    }

    private void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            float restoreVolume = Mathf.Max(previousVolume, 0.05f);
            volumeSlider.value = restoreVolume;
            AudioManage.Instance.SetVolume(restoreVolume);
        }
        else
        {
            volumeSlider.value = 0f;
            AudioManage.Instance.SetVolume(0f);
        }
    }
}