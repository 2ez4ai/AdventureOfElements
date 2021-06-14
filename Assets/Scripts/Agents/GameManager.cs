using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager m_instance;
    public static GameManager Instance => m_instance;

    PlayerLogic m_playerLogic;    // HP;
    Controller m_controller;
    SkillController m_skillController;

    void Awake(){
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_playerLogic = FindObjectOfType<PlayerLogic>();
        m_controller = FindObjectOfType<Controller>();
        m_skillController = FindObjectOfType<SkillController>();
    }

    public void SaveData(){
        m_playerLogic.SaveData();
        m_controller.SaveData();
        m_skillController.SaveData();
        PlayerPrefs.SetInt("Loadable", 1);
        PlayerPrefs.Save();
    }

    // public void LoadData(){
    //     // m_controller.LoadData();    // no explict function; done by checking Localisation.m_loadChecker
    //     // m_skillController.LoadData();
    //     m_playerLogic.LoadData();
    // }
}
