using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelection : MonoBehaviour
{
    Toggle[] m_toggles;
    [SerializeField] List<Image> m_toggleSprites = new List<Image>();
    [SerializeField] Button m_btn;
    [SerializeField] Tooltip m_tooltip;

    List<string> m_toggleName = new List<string>();
    List<string> m_toggleLV = new List<string>();
    List<string> m_toggleEffect = new List<string>();
    List<string> m_toggleDescription = new List<string>();

    public bool m_activate = false;

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
    }

    void Update(){
        if(m_activate){
            CheckSelection();
        }
    }

    void CheckSelection()
    {
        Debug.Log("Activated.");
        int selected = -1;
        for(int i = 0; i < m_toggles.Length; i++){
            if (m_toggles[i].isOn){
                selected = i;
                break;
            }
        }

        if(selected != -1){
            Debug.Log("Get a selection!");
            m_btn.interactable = true;
            m_tooltip.UpdateInfo(m_toggleName[selected], m_toggleLV[selected], m_toggleEffect[selected], m_toggleDescription[selected]);
            Debug.Log("Selected!");
        }
        else{
            m_btn.interactable = false;
            m_tooltip.CloseInfo();
        }
    }

    public void UpdateSelection(int index, Sprite icon, string name, string lv, string effect, string description){
        if(index >= m_toggles.Length){
            Debug.Log("Error!");
            return;
        }
        Debug.Log("Update " + index);
        m_toggleSprites[index].sprite = icon;
        m_toggleName[index] = name;
        m_toggleLV[index] = lv;
        m_toggleEffect[index] = effect;
        m_toggleDescription[index] = description;
    }
}
