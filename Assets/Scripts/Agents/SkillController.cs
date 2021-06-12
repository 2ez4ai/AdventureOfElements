using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    [SerializeField] Controller m_controller;
    [SerializeField] PlayerLogic m_player;
    [SerializeField] SkillSlots m_playerSkillSlots;
    [SerializeField] SkillSelection m_skillSelection;
    [SerializeField] List<Skill> m_skillList = new List<Skill>();

    [SerializeField] int m_numSkill = 2;
    [SerializeField] List<int> m_learnedSkillLV = new List<int>();

    int m_randomSeed;
    List<int> m_generateList;
    List<bool> m_exclusiveList;

    void Awake() {
        for(int i = 0; i < m_numSkill; i++){
            m_learnedSkillLV.Add(0);
        }
    }

    void UpdateGenerateList(int level){
        // Debug.Log("Lv. " + level);
        m_generateList = new List<int>();
        m_exclusiveList = new List<bool>();
        for(int s = 0; s < m_skillList.Count; s++){
            m_exclusiveList.Add(false);
        }
        for(int s = 0; s < m_skillList.Count; s++){
            Skill skill = m_skillList[s];
            // already learned
            if(m_learnedSkillLV[skill.m_skillID] != 0 && m_learnedSkillLV[skill.m_skillID] >= skill.m_lv && !skill.m_linear){
                continue;
            }
            // lv requirements
            if(skill.m_prerequisiteLV > level){
                continue;
            }
            // skill requirements
            List<int> prerequisite = skill.m_prerequisiteSkill;
            bool satisfied = true;
            foreach(int ps in prerequisite){
                if(m_learnedSkillLV[m_skillList[ps].m_skillID] < m_skillList[ps].m_lv){
                    // havent learned yet
                    satisfied = false;
                    break;
                }
            }
            if(satisfied){
                m_generateList.Add(s);
            }
        }
        // foreach(int s in m_generateList){
        //     Debug.Log("Skill pool: " + m_skillList[s].m_name + " Lv. " + m_skillList[s].m_lv);
        // }
    }

    public void GenerateLoadSkills(int level){
        m_skillSelection.m_activated = true;
        UpdateGenerateList(level);
        for(int i = 0; i < 3; i++){
            int skillIndex = Random.Range(0, m_generateList.Count);
            Skill skill = m_skillList[m_generateList[skillIndex]];
            if(m_exclusiveList[m_generateList[skillIndex]]){
                i--;
                break;
            }
            // m_exclusiveList[skillIndex] = true;
            foreach(int t in skill.m_exclusivePool){
                m_exclusiveList[t] = true;
            }
            string skillName = LocalizationManager.m_instance.GetLocalisedString(skill.m_name);
            string lv = "";
            if(skill.m_lv != 0){
                lv = LocalizationManager.m_instance.GetLocalisedString("LV") + " " + skill.m_lv;
            }
            if(m_learnedSkillLV[skill.m_skillID] != 0){
                lv = LocalizationManager.m_instance.GetLocalisedString("LV") + " " + m_learnedSkillLV[skill.m_skillID] + " + 1";    // upgrade
            }
            string skillEffect = LocalizationManager.m_instance.GetLocalisedString(skill.m_name+"Effect");
            string skillDescription = LocalizationManager.m_instance.GetLocalisedString(skill.m_name+"Description");
            m_skillSelection.SelectionItems(i, skill.m_sprite, skill.m_ID, skillName, lv, skillEffect, skillDescription);
        }
    }

    public void SkillUpdate(int id){
        bool learned = false;
        Skill skill = m_skillList[id];
        switch(id){
            case 0:    // restore hp
            case 1:
                m_player.LearnGourd(skill.m_keyValue);
                break;
            case 2:    // increase maximum HP
                m_player.m_maxHP += skill.m_keyValue;
                m_player.InitUI();
                learned = true;
                break;
            case 3:    // special tile
            case 4:
            case 5:
                m_player.LearnSpecial(skill.m_lv);
                learned = true;
                break;
            case 6:    // diagonally swap
            case 7:
            case 8:
                m_player.LearnDiagonal(skill.m_lv);
                learned = true;
                break;
            case 9:    // extra regen
                m_player.SetBonus(skill.m_keyValue);
                learned = true;
                break;
            case 10:    // gourd
            case 11:
            case 12:
                learned = true;
                break;

        }
        if(learned){
            m_playerSkillSlots.FillSkillSlot(skill);
            if(skill.m_linear){
                m_learnedSkillLV[skill.m_skillID] ++;
            }
            else{
                m_learnedSkillLV[skill.m_skillID] = m_skillList[id].m_lv;
            }
        }
        m_controller.BoardExpand();
    }

    public bool SkillSelectionActivation(){
        return m_skillSelection.m_activated;
    }

    public void CreatureSkillTrigger(Skill skill){
        int id = skill.m_skillID;
        switch(id){
            case 97:
                m_player.LearnStomp(skill.m_lv);
                break;
        }
    }

    public void CleanCreatureSkills(){
        m_player.LearnStomp(0);
    }
}
