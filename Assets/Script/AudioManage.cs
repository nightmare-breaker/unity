using UnityEngine;

public enum BGMType
{
    A, // main + select_stage + game_over
    B, // Stage_1
    C  // Final stage
}

public class AudioManage : MonoBehaviour
{
    public static AudioManage Instance;

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip bgmA, bgmB, bgmC;
    private const string VolumeKey = "BGMVolume";

    private BGMType? currentBGM = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.5f);
        bgmSource.volume = savedVolume;

        bgmSource.Play();
    }


    public void PlayBGM(BGMType type)
    {
        if (currentBGM == type && bgmSource.isPlaying) return;

        currentBGM = type;
        bgmSource.clip = type switch
        {
            BGMType.A => bgmA,
            BGMType.B => bgmB,
            BGMType.C => bgmC,
            _ => null
        };

        if (bgmSource.clip != null)
            bgmSource.Play();
    }

    public float GetVolume()
    {
        return bgmSource.volume;
    }

    public void SetVolume(float value)
    {
        bgmSource.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }
}