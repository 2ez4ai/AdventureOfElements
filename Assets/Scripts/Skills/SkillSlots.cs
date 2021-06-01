using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlots : MonoBehaviour
{
    [SerializeField] int m_numSlots = 12;

    List<GameObject> m_skillSlots = new List<GameObject>();
    List<MouseOver> m_mouseOverSkillSlots = new List<MouseOver>();
    int m_index;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < m_numSlots; i++){
            m_skillSlots.Add(transform.Find("Slot " + i).gameObject);
            m_mouseOverSkillSlots.Add(m_skillSlots[i].GetComponent<MouseOver>());
        }
        m_index = 0;
    }

    public void FillSkillSlot(Skill skill){
        // Avatar Info: UI only
        m_skillSlots[m_index].SetActive(true);
        m_mouseOverSkillSlots[m_index].ChangeIcon(skill.m_sprite);
        m_mouseOverSkillSlots[m_index].m_name = skill.m_name;
        m_mouseOverSkillSlots[m_index].m_level = "Lv. " + skill.m_lv;
        m_mouseOverSkillSlots[m_index].m_effect = skill.m_description;
        m_index ++;
    }
}
