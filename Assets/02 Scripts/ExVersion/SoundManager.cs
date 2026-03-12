using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum Sound
{
    Button_Click,
    Player_Attack,
    Player_Hit,
    Enemy_Die,
    Enemy2_Warning,
    Enemy_Hit,
    Enemy_Spawn,
    Countdown, //Warning_Sound
    Title_Bgm,
    InGame_Bgm,
    Boss_Bgm,
    Ambient_Sound,
    Item_Spawn,
    Item_PickUp,
    Item_Use,
    Game_Clear,
    Game_Over


}


public class SoundManager : MonoBehaviour
{

    private static SoundManager instance;

    public static SoundManager Instance
    {
        get
        {
            if (instance == null) instance = new SoundManager();
            return instance;
        }

    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(gameObject); }

    }

    [Header("Audio Sources")]
    [Tooltip("배경음을 재생할 AudioSource 컴포넌트를 연결하세요.")]
    [SerializeField] private AudioSource BgmSource;

    [Tooltip("효과음을 재생할 AudioSource 컴포넌트를 연결하세요.")]
    [SerializeField] private AudioSource SfxSource;

    [Header("Audio Clips")]
    [Tooltip("0.타이틀 브금 1.보스 브금 2.인게임 브금 3.환경음")]
    public AudioClip[] BgmClips;

    [Tooltip("0.버튼 클릭 1.플레이어 공격 2.플레이어 피격 3.적 사망 4.적 피격 5.적 스폰 6.경고음 7.아이템 스폰 8.아이템 획득 9.게임 클리어 10.게임 오버 11.에너미2 다이,12.아이템 사용")]
    //효과 사운드를 재생한다.
    public AudioClip[] SfxClips;

    public void BgmPlay(int index)
    {

        if (index < BgmClips.Length)
        {
            BgmSource.clip = BgmClips[index];
            BgmSource.Play();
        }
    }
    public void BgmStop()
    {
        BgmSource.Stop();
    }
    public void SfxPlay(int index)
    {
        if (index < SfxClips.Length)
        {
            SfxSource.clip = SfxClips[index];
            SfxSource.Play();
        }
    }
    public void SfxStop()
    {
        SfxSource.Stop();
    }

    //BGM을 재생한다.
    public void PlayBGM(Sound sound, bool isLoop = true)
    {
        AudioClip clip = null;
        switch (sound)
        {
            case Sound.Title_Bgm:
                clip = BgmClips[0];
                break;
            case Sound.Boss_Bgm:
                clip = BgmClips[1];
                break;
            case Sound.InGame_Bgm:
                clip = BgmClips[2];
                break;
            case Sound.Ambient_Sound:
                clip = BgmClips[3];
                break;

        }

        if (clip != null)
        {
            BgmSource.clip = clip;
            BgmSource.loop = isLoop;
            BgmSource.Play();
        }
    }

    public void PlaySfx(Sound sound, float volume = 1.0f)
    {
        AudioClip clip = GetAudioClip(sound);

        if (clip != null)
        {
            SfxSource.PlayOneShot(clip, volume);
        }


    }
    private AudioClip GetAudioClip(Sound sound)
    {


        switch (sound)
        {
            case Sound.Button_Click:
                return SfxClips[0];

            case Sound.Player_Attack:
                return SfxClips[1];
             
            case Sound.Player_Hit:
                return SfxClips[2];
              
            case Sound.Enemy_Die:
                return SfxClips[3];
             
            case Sound.Enemy_Hit:
                return SfxClips[4];
            
            case Sound.Enemy_Spawn:
                return SfxClips[5];

            case Sound.Countdown: //Warning_Sound
                return SfxClips[6];
            
            case Sound.Item_Spawn:
                return SfxClips[7];
         
            case Sound.Item_PickUp:
                return SfxClips[8];
               
            case Sound.Game_Clear:
                return SfxClips[9];
         
            case Sound.Game_Over:
                return SfxClips[10];

            case Sound.Enemy2_Warning:
                return SfxClips[11];

            case Sound.Item_Use:
                return SfxClips[12];



            default: return null;
           

        }


    }
}
