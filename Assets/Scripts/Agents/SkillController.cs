using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    [SerializeField] PlayerLogic m_player;
    [SerializeField] List<Skill> m_skillList = new List<Skill>();
    List<int> m_playerObtainedSkillID = new List<int>();
    [SerializeField] SkillSelection m_skillSelection;
    [SerializeField] SkillSlots m_playerSkillSlots;

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
            m_skillSelection.SelectionItems(i, skill.m_sprite, skill.m_ID, skill.m_name, lv, skill.m_effect, skill.m_description);
        }
    }

    public void SkillUpdate(int id){
        switch(id){
            case 0:    // restore hp
            case 1:
                int temp = m_player.m_HP + m_skillList[id].m_keyValue;
                m_player.m_HP = temp > m_player.m_maxHP ? m_player.m_maxHP : temp;
                m_player.InitUI();
                break;
            case 2:    // increase maximum HP
            case 3:
            case 4:
                m_player.m_maxHP += m_skillList[id].m_keyValue;
                m_player.InitUI();
                m_playerSkillSlots.FillSkillSlot(m_skillList[id]);
                m_playerObtainedSkillID.Add(id);
                break;
            case 5:    // special tile
            case 6:
            case 7:
                m_player.LearnSpecial(m_skillList[id].m_lv);
                m_playerSkillSlots.FillSkillSlot(m_skillList[id]);
                m_playerObtainedSkillID.Add(id);
                break;
            case 8:    // diagonally swap
            case 9:
            case 10:
                m_player.LearnDiagonal(m_skillList[id].m_lv);
                m_playerSkillSlots.FillSkillSlot(m_skillList[id]);
                m_playerObtainedSkillID.Add(id);
                break;
        }
    }

    public bool SkillSelectionActivation(){
        return m_skillSelection.m_activated;
    }
}
