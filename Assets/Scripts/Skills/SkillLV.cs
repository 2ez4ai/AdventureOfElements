using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillLV : MonoBehaviour
{
    [SerializeField] Text m_lvText;

    public void SetLevel(int lv){
        m_lvText.text = "" + lv;
    }
}
