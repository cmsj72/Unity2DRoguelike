using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource efxSource;
    public AudioSource musicSource;
    public static SoundManager instance = null;

    //  사운드 이펙트의 피치에 약간의 랜덤한 변화를 주기 위해 사용
    //  사운드가 재생될 때 약간 다르게 들리게 하되, 거슬릴 정도로 큰 폭의 변화를 일으키지는 않는다.
    //  원음의 피치에서 5% 높건 낮음을 나타낸다.
    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    //  하나의 오디오 클립을 재생할 때 사용할 함수
    //  AudioClip은 디지털 오디오 정보를 가진 애셋
    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    //  params 키워드는 , 로 구분된 같은 타입의 여러 입력들을 한방에 받게 해준다.
    public void RandomizeSfx(params AudioClip [] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }
}
