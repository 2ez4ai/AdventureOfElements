using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager m_instance = null;

    [SerializeField] AudioClip m_selectSound;
    [SerializeField] AudioClip m_confirmSound;
    [SerializeField] AudioClip m_backSound;
    [SerializeField] AudioClip m_quitSound;

    [SerializeField] List<AudioClip> m_bgm;
    int m_bgmIndex;

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

}
