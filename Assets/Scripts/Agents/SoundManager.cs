using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager m_instance = null;

    [SerializeField] List<AudioClip> m_bgm;
    int m_bgmIndex;

    // UI
    [SerializeField] AudioClip m_selectSound;
    [SerializeField] AudioClip m_confirmSound;
    [SerializeField] AudioClip m_backSound;
    [SerializeField] AudioClip m_quitSound;
    [SerializeField] AudioClip m_win;
    [SerializeField] AudioClip m_lose;

    // Tile
    [SerializeField] AudioClip m_tileClickSound;
    [SerializeField] AudioClip m_tileMoveSound;
    [SerializeField] AudioClip m_tileRemoveSound;
    [SerializeField] List<AudioClip> m_attackSound;
    [SerializeField] AudioClip m_weakHitSound;

    // Skill
    [SerializeField] AudioClip m_regenHP;
    [SerializeField] AudioClip m_learnSkill;
    [SerializeField] AudioClip m_explode;
    [SerializeField] AudioClip m_generateExplode;
    [SerializeField] AudioClip m_gourd;

    AudioSource m_audioSource;

    void Awake()
    {
        SetupUIManagerSingleton();
    }

    void SetupUIManagerSingleton()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_bgmIndex = 0;
    }

    void Update(){
        if (!m_audioSource.isPlaying)
        {
            int temp = Random.Range(0, m_bgm.Count);
            if(temp == m_bgmIndex){
                m_bgmIndex = (temp + 1) % m_bgm.Count;
            }
            else{
                m_bgmIndex = temp;
            }
            m_audioSource.clip = m_bgm[m_bgmIndex];
            m_audioSource.Play();
        }
    }

    void PlaySound(AudioClip sound)
    {
        if(m_audioSource && sound)
        {
            m_audioSource.PlayOneShot(sound);
        }
    }

    public void PlaySelectSound(){
        PlaySound(m_selectSound);
    }

    public void PlayConfirmSound(){
        PlaySound(m_confirmSound);
    }

    public void PlayBackSound(){
        PlaySound(m_backSound);
    }

    public void PlayQuitSound(){
        PlaySound(m_quitSound);
    }

    public void PlayTileClickSound(){
        PlaySound(m_tileClickSound);
    }

    public void PlayTileMoveSound(){
        PlaySound(m_tileMoveSound);
    }

    public void PlayTileRemoveSound(){
        PlaySound(m_tileRemoveSound);
    }

    public void PlayAttackSound(int type){
        PlaySound(m_attackSound[type]);
    }

    public void PlayWeakPointSound(){
        PlaySound(m_weakHitSound);
    }

    public void PlayRegenHPSound(){
        PlaySound(m_regenHP);
    }

    public void PlayLearnSkillSound(){
        PlaySound(m_learnSkill);
    }

    public void PlayExplodeSound(){
        PlaySound(m_explode);
    }

    public void PlayGenerateExplodeSound(){
        PlaySound(m_generateExplode);
    }

    public void PlaySkillGourdSound(){
        PlaySound(m_gourd);
    }

    public void PlayWinSound(){
        PlaySound(m_win);
    }

    public void PlayLoseSound(){
        PlaySound(m_lose);
    }
}
