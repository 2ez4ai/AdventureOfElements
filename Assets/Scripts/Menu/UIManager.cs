using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuPageType
{
    MainMenu,
    Language
    // Game
}

[System.Serializable]
public class MenuPage    // menu pages
{
    public MenuPageType m_menuPageType;
    public GameObject m_gameObject;    // a parental object
}

public class UIManager : MonoBehaviour
{
    public static UIManager m_instance = null;

    // Fill in Editor
    [SerializeField] List<MenuPage> m_menuPages = new List<MenuPage>();

    // Fast Lookups
    Dictionary<MenuPageType, GameObject> m_menuPageDictionary = new Dictionary<MenuPageType, GameObject>();

    [SerializeField] MenuPageType m_currentMenuPage = MenuPageType.MainMenu;

    [SerializeField] AudioClip m_uiNextSound;

    [SerializeField] AudioClip m_uiConfirmSound;

    AudioSource m_audioSource;

    // Use this for initialization
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
        //DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(MenuPage p in m_menuPages)
        {
            m_menuPageDictionary[p.m_menuPageType] = p.m_gameObject;
        }

        // m_audioSource = GetComponent<AudioSource>();
    }

    public void SetMenuState(MenuPageType newMenuPageType)
    {
        // switch menu page
        m_menuPageDictionary[m_currentMenuPage].SetActive(false);
        m_menuPageDictionary[newMenuPageType].SetActive(true);

        m_currentMenuPage = newMenuPageType;
    }

    public void PlayNextSound()
    {
        PlaySound(m_uiNextSound);
    }

    public void PlayConfirmSound()
    {
        PlaySound(m_uiConfirmSound);
    }

    void PlaySound(AudioClip sound)
    {
        if(m_audioSource && sound)
        {
            m_audioSource.PlayOneShot(sound);
        }
    }
}
