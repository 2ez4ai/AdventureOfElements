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
            string effect = skill.m_effectPre;
            if(skill.m_lv != 0){
                lv = "Lv. " + skill.m_lv;
                for(int l = 0; l < skill.m_keyValue.Count; l ++){
                    string temp = " ";
                    if(l != 0){
                        temp = "/";
                    }
                    if(l + 1 == skill.m_lv){
                        temp += "<b><i>" + skill.m_keyValue[l] + "</i></b>";
                    }
                    else{
                        temp += "<i>" + skill.m_keyValue[l] + "</i>";
                    }
                    if(l == skill.m_keyValue.Count - 1){
                        temp += " ";
                    }
                    effect += temp;
                }
                effect += skill.m_effectPost;
            }
            m_skillSelection.SelectionItems(i, skill.m_sprite, skill.m_ID, skill.m_name, lv, effect, skill.m_description);
        }
    }

    public void SkillUpdate(int id){
        switch(id){
            case 0:    // restore hp
            case 1:
                int temp = m_player.m_HP + m_skillList[id].m_keyValue[m_skillList[id].m_lv];
                m_player.m_HP = temp > m_player.m_maxHP ? m_player.m_maxHP : temp;
                m_player.InitUI();
                break;
            case 2:    // increase maximum HP
                m_player.m_maxHP += m_skillList[id].m_keyValue[m_skillList[id].m_lv - 1];
                m_player.InitUI();
                m_playerSkillSlots.FillSkillSlot(m_skillList[id]);
                m_playerObtainedSkillID.Add(id);
                break;
        }
    }

    public bool SkillSelectionActivation(){
        return m_skillSelection.m_activated;
    }
}
