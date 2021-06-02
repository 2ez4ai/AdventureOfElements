using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlots : MonoBehaviour
{
    [SerializeField] int m_numSlots = 12;

    List<GameObject> m_skillSlots = new List<GameObject>();
    List<MouseOver> m_mouseOverSkillSlots = new List<MouseOver>();
    List<SkillLV> m_skillLV = new List<SkillLV>();
    int m_index;

    void Awake()
    {
        for(int i = 0; i < m_numSlots; i++){
            m_skillSlots.Add(transform.Find("Slot " + i).gameObject);
            m_mouseOverSkillSlots.Add(m_skillSlots[i].GetComponent<MouseOver>());
            m_skillLV.Add(m_skillSlots[i].GetComponent<SkillLV>());
            m_skillSlots[i].SetActive(false);
        }
        m_index = 0;
    }

    public void FillSkillSlot(Skill skill){
        // tooltip ui things
        m_skillSlots[m_index].SetActive(true);
        m_mouseOverSkillSlots[m_index].ChangeIcon(skill.m_sprite);
        m_mouseOverSkillSlots[m_index].m_name = skill.m_name;
        m_mouseOverSkillSlots[m_index].m_level = "Lv. " + skill.m_lv;
        m_mouseOverSkillSlots[m_index].m_effect = skill.m_description;
        m_skillLV[m_index].SetLevel(skill.m_lv);
        m_index ++;
    }
}
