using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelection : MonoBehaviour
{
    Toggle[] m_toggles;
    [SerializeField] Button m_btn;

    // Start is called before the first frame update
    void Start()
    {
        m_toggles = GetComponentsInChildren<Toggle>();
    }

    void Update(){
        if(GetSelectedToggle() != null){
            m_btn.interactable = true;
        }
        else{
            m_btn.interactable = false;
        }
    }

    Toggle GetSelectedToggle()
    {
        foreach (var t in m_toggles){
            if (t.isOn){
                return t;  //returns selected toggle
            }
        }
        return null;           // if nothing is selected return null
    }
}
