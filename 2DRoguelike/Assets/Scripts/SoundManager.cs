using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource efxSource;
    public AudioSource musicSource;
    public static SoundManager instance = null;

    //  ���� ����Ʈ�� ��ġ�� �ణ�� ������ ��ȭ�� �ֱ� ���� ���
    //  ���尡 ����� �� �ణ �ٸ��� �鸮�� �ϵ�, �Ž��� ������ ū ���� ��ȭ�� ����Ű���� �ʴ´�.
    //  ������ ��ġ���� 5% ���� ������ ��Ÿ����.
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

    //  �ϳ��� ����� Ŭ���� ����� �� ����� �Լ�
    //  AudioClip�� ������ ����� ������ ���� �ּ�
    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    //  params Ű����� , �� ���е� ���� Ÿ���� ���� �Էµ��� �ѹ濡 �ް� ���ش�.
    public void RandomizeSfx(params AudioClip [] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }
}
