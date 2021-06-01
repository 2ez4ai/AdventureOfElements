using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelection : MonoBehaviour
{
    Toggle[] m_toggles;
    [SerializeField] List<Image> m_toggleSprites = new List<Image>();

    [SerializeField] Button m_btn;
    [SerializeField] int m_btnType = 0;    // 0: confirm; 1: lose game
    [SerializeField] DialogScript m_dialog;
    [SerializeField] Tooltip m_tooltip;
    [SerializeField] Controller m_controller;

    List<string> m_toggleName = new List<string>();
    List<string> m_toggleLV = new List<string>();
    List<string> m_toggleEffect = new List<string>();
    List<string> m_toggleDescription = new List<string>();

    public bool m_activated = false;
    public int m_selected = -1;

    // Start is called before the first frame update
    void Start()
    {
        m_toggles = GetComponentsInChildren<Toggle>();
        foreach(Toggle t in m_toggles){
            m_toggleName.Add("");
            m_toggleLV.Add("");
            m_toggleEffect.Add("");
            m_toggleDescription.Add("");
        }
        m_btn.onClick.AddListener(SelectionDone);
    }

    void Update(){
        CheckSelection();
    }

    void CheckSelection()
    {
        for(int i = 0; i < m_toggles.Length; i++){
            if (m_toggles[i].isOn){
                m_selected = i;
                break;
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
        m_controller.SkillUpdate(m_selected);
        m_selected = -1;
        m_dialog.TurnOff();
        m_activated = false;
    }

    public void UpdateSelection(int index, Sprite icon, string name, string lv, string effect, string description){
        if(index >= m_toggles.Length){
            Debug.Log("Error!");
            return;
        }
        m_toggleSprites[index].sprite = icon;
        m_toggleName[index] = name;
        m_toggleLV[index] = lv;
        m_toggleEffect[index] = effect;
        m_toggleDescription[index] = description;
    }
}
