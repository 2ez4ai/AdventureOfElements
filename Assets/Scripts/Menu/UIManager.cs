using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIStateType
{
    MainMenu,
    Options,
    VideoOptions,
    AudioOptions,
    Game
}

[System.Serializable]
public class UIState
{
    public UIStateType m_UIStateType;
    public GameObject m_GameObject;
}

public class UIManager : MonoBehaviour
{
    public static UIManager m_Instance = null;

    // Fill in Editor
    [SerializeField]
    List<UIState> m_UIStates = new List<UIState>();

    // Fast Lookups
    Dictionary<UIStateType, GameObject> m_UIStateDictionary = new Dictionary<UIStateType, GameObject>();

    [SerializeField]
    UIStateType m_CurrentUIStateType = UIStateType.MainMenu;

    [SerializeField]
    AudioClip m_UINextSound;

    [SerializeField]
    AudioClip m_UIConfirmSound;

    AudioSource m_AudioSource;

    // Use this for initialization
    void Awake()
    {
        SetupUIManagerSingleton();
    }

    void SetupUIManagerSingleton()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else if (m_Instance != this)
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(UIState state in m_UIStates)
        {
            m_UIStateDictionary[state.m_UIStateType] = state.m_GameObject;
        }

        m_AudioSource = GetComponent<AudioSource>();
    }

    public void SetUIState(UIStateType newUIStateType)
    {
        m_UIStateDictionary[m_CurrentUIStateType].SetActive(false);
        m_UIStateDictionary[newUIStateType].SetActive(true);

        m_CurrentUIStateType = newUIStateType;
    }

    public void PlayNextSound()
    {
        PlaySound(m_UINextSound);
    }

    public void PlayConfirmSound()
    {
        PlaySound(m_UIConfirmSound);
    }

    void PlaySound(AudioClip sound)
    {
        if(m_AudioSource && sound)
        {
            m_AudioSource.PlayOneShot(sound);
        }
    }
}
