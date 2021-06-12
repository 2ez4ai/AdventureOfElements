using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlots : MonoBehaviour
{
    [SerializeField] PlayerLogic m_player;
    [SerializeField] bool m_creature;
    [SerializeField] int m_numSlots = 12;
    [SerializeField] int m_numSkill = 7;

    List<GameObject> m_skillSlots = new List<GameObject>();
    List<int> m_skillOccupy = new List<int>();
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
        for(int i = 0; i < m_numSkill; i++){
            m_skillOccupy.Add(-1);
        }
        m_index = 0;
    }

    public void CleanSlots(){
        for(int i = 0; i < m_numSlots; i++){
            m_skillSlots[i].SetActive(false);
        }
        for(int i = 0; i < m_numSkill; i++){
            m_skillOccupy[i] = -1;
        }
        m_index = 0;
    }

    public void FillSkillSlot(Skill skill){
        int temp = m_index;
        if(m_creature){
            m_skillSlots[temp].SetActive(true);
            m_mouseOverSkillSlots[temp].ChangeIcon(skill.m_sprite);
            m_mouseOverSkillSlots[temp].m_name = skill.m_name;
            m_mouseOverSkillSlots[temp].m_level = "Lv. " + skill.m_lv;
            m_mouseOverSkillSlots[temp].m_effect = skill.m_effect;
            m_mouseOverSkillSlots[temp].m_description = skill.m_description;
            m_skillLV[temp].SetLevel(skill.m_lv, skill.m_linear);
            return;
        }

        if(m_skillOccupy[skill.m_skillID] != -1){
            // already occupy one slot
            temp = m_skillOccupy[skill.m_skillID];
        }
        else{
            m_index ++;
        }
        // tooltip ui things
        m_skillSlots[temp].SetActive(true);
        m_mouseOverSkillSlots[temp].ChangeIcon(skill.m_sprite);
        m_mouseOverSkillSlots[temp].m_name = LocalizationManager.m_instance.GetLocalisedString(skill.m_name);
        m_mouseOverSkillSlots[temp].m_level = LocalizationManager.m_instance.GetLocalisedString("LV") + " " + skill.m_lv;
        m_mouseOverSkillSlots[temp].m_effect = LocalizationManager.m_instance.GetLocalisedString(skill.m_name+"Effect");

        switch(skill.m_skillID){
            case 6:
                m_player.SetGourdMaxHP(skill.m_keyValue, LocalizationManager.m_instance.GetLocalisedString(skill.m_name+"Effect"), m_mouseOverSkillSlots[temp]);
                break;
        }
        m_mouseOverSkillSlots[temp].m_description = LocalizationManager.m_instance.GetLocalisedString(skill.m_name+"Description");
        m_skillLV[temp].SetLevel(skill.m_lv, skill.m_linear);
        if(skill.m_linear){
            m_mouseOverSkillSlots[temp].m_level = LocalizationManager.m_instance.GetLocalisedString("LV") + " " + m_skillLV[temp].GetLevel();
        }
        m_skillOccupy[skill.m_skillID] = temp;
    }
}
