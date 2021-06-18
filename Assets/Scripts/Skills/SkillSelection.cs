using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelection : MonoBehaviour
{
    Toggle[] m_toggles;
    [SerializeField] List<Image> m_toggleSprites = new List<Image>();

    [SerializeField] Button m_btn;
    [SerializeField] DialogScript m_dialog;
    [SerializeField] Tooltip m_tooltip;
    [SerializeField] SkillController m_skillController;

    List<int> m_toggleID = new List<int>();
    List<string> m_toggleName = new List<string>();
    List<string> m_toggleLV = new List<string>();
    List<string> m_toggleEffect = new List<string>();
    List<string> m_toggleDescription = new List<string>();

    public bool m_activated = false;
    int m_selected = -1;

    int m_controllerY;
    const float k_maxControllerCooldown = 0.5f;
    const float k_maxControllerActivation = 3.0f;
    float m_controllerCooldownTimer;
    float m_controllerActivationTimer;

    void Awake()
    {
        m_toggles = GetComponentsInChildren<Toggle>();
        foreach(Toggle t in m_toggles){
            m_toggleID.Add(0);
            m_toggleName.Add("");
            m_toggleLV.Add("");
            m_toggleEffect.Add("");
            m_toggleDescription.Add("");
        }
        m_btn.onClick.AddListener(SelectionDone);
        m_controllerCooldownTimer = k_maxControllerCooldown;
        m_controllerActivationTimer = 0.0f;
    }

    void Update(){
        CheckSelection();
        ControllerSelection();
    }

    void ControllerSelection(){
        if(m_controllerCooldownTimer < 0.0f){
            bool makeSelection = false;
            float horizontalInput = Input.GetAxis("Horizontal");
            int oldIndex = m_selected;
            if(horizontalInput > 0.1f){
                m_selected = m_selected + 1 >= m_toggles.Length? m_toggles.Length - 1 : m_selected + 1;
                makeSelection = true;
            }
            if(horizontalInput < -0.1f){
                m_selected = m_selected - 1 >= 0 ? m_selected - 1 : 0;
                makeSelection = true;
            }
            if(makeSelection){
                if(oldIndex != -1){
                    m_toggles[oldIndex].isOn = false;
                }
                SoundManager.m_instance.PlaySelectSound();
                m_toggles[m_selected].isOn = true;
                m_controllerCooldownTimer = k_maxControllerCooldown;
                m_controllerActivationTimer = k_maxControllerActivation;
            }
        }
        else{
            m_controllerCooldownTimer -= Time.deltaTime;
        }

        if(m_selected != -1 && Input.GetButtonDown("Fire1") && m_controllerActivationTimer > 0.0f){
            SelectionDone();
        }

        m_controllerActivationTimer -= Time.deltaTime;
    }

    void CheckSelection()
    {
        for(int i = 0; i < m_toggles.Length; i++){
            if (m_toggles[i].isOn){
                if(m_selected != i){
                    m_selected = i;
                    SoundManager.m_instance.PlaySelectSound();
                    break;
                }
            }
        }

        if(m_selected != -1){
            m_btn.interactable = true;
            m_tooltip.UpdateInfo(m_toggleName[m_selected], m_toggleLV[m_selected], m_toggleEffect[m_selected], m_toggleDescription[m_selected]);
        }
        else{
            m_btn.interactable = false;
            m_tooltip.CloseInfo();
        }
    }

    void SelectionDone(){
        foreach(Toggle t in m_toggles){
            t.isOn = false;
        }
        m_skillController.SkillUpdate(m_toggleID[m_selected]);
        m_selected = -1;
        m_dialog.TurnOff();
        m_activated = false;
    }

    public void SelectionItems(int index, Sprite icon, int id, string name, string lv, string effect, string description){
        // update the skills provided for the player
        if(index >= m_toggles.Length){
            Debug.Log("Error!");
            return;
        }
        m_toggleSprites[index].sprite = icon;
        m_toggleID[index] = id;
        m_toggleName[index] = name;
        m_toggleLV[index] = lv;
        m_toggleEffect[index] = effect;
        m_toggleDescription[index] = description;
    }
}
