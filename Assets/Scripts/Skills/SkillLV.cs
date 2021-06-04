using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillLV : MonoBehaviour
{
    [SerializeField] Text m_lvText;

    int m_lv = 0;

    public void SetLevel(int lv, bool linear = false){
        if(linear){
            m_lv ++;
        }
        else{
            m_lv = lv;
        }
        m_lvText.text = "" + m_lv;
    }

    public int GetLevel(){
        return m_lv;
    }
}
