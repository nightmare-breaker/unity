using UnityEngine;

public class BGMManage : MonoBehaviour
{
    [SerializeField] private BGMType bgmToPlay;

    void Start()
    {
        AudioManage.Instance?.PlayBGM(bgmToPlay);
    }
}