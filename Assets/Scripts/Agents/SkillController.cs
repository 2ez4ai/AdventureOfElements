using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    [SerializeField] PlayerLogic m_player;
    [SerializeField] List<Skill> m_skillList = new List<Skill>();
    [SerializeField] SkillSelection m_skillSelection;
    [SerializeField] SkillSlots m_playerSkillSlots;

    [SerializeField] int m_numSkill = 5;
    [SerializeField] List<int> m_learnedSkillLV = new List<int>();

    void Awake() {
        for(int i = 0; i < m_numSkill; i++){
            m_learnedSkillLV.Add(0);
        }
    }

    public void GenerateLoadSkills(){
        int n = m_skillList.Count;
        m_skillSelection.m_activated = true;
        for(int i = 0; i < 3; i++){
            int skillIndex = Random.Range(0, n);
            Skill skill = m_skillList[skillIndex];
            string lv = "";
            if(skill.m_lv != 0){
                lv = "Lv. " + skill.m_lv;
            }
            if(m_learnedSkillLV[skill.m_skillID] != 0){
                lv = "Lv. " + m_learnedSkillLV[skill.m_skillID] + " + 1";    // upgrade
            }
            m_skillSelection.SelectionItems(i, skill.m_sprite, skill.m_ID, skill.m_name, lv, skill.m_effect, skill.m_description);
        }
    }

    public void SkillUpdate(int id){
        bool learned = false;
        switch(id){
            case 0:    // restore hp
            case 1:
                int temp = m_player.m_HP + m_skillList[id].m_keyValue;
                m_player.m_HP = temp > m_player.m_maxHP ? m_player.m_maxHP : temp;
                m_player.InitUI();
                break;
            case 2:    // increase maximum HP
                m_player.m_maxHP += m_skillList[id].m_keyValue;
                m_player.InitUI();
                learned = true;
                break;
            case 3:    // special tile
            case 4:
            case 5:
                m_player.LearnSpecial(m_skillList[id].m_lv);
                learned = true;
                break;
            case 6:    // diagonally swap
            case 7:
            case 8:
                m_player.LearnDiagonal(m_skillList[id].m_lv);
                learned = true;
                break;
        }
        if(learned){
            m_playerSkillSlots.FillSkillSlot(m_skillList[id]);
            m_learnedSkillLV[m_skillList[id].m_skillID] = m_skillList[id].m_lv;
        }
    }

    public bool SkillSelectionActivation(){
        return m_skillSelection.m_activated;
    }
}
