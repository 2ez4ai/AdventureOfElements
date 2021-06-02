using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    // ------------------------------------------------------------------------
    // use to control levels, skills.
    // ------------------------------------------------------------------------

    [SerializeField] Board m_board;
    [SerializeField] PlayerLogic m_player;
    [SerializeField] CreatureLogic m_creature;
    [SerializeField] List<Creature> m_creatureList = new List<Creature>();

    [SerializeField] MouseOver m_creatureMouseOverAvatar;
    [SerializeField] MouseOver m_creatureMouseOverAttack;
    [SerializeField] MouseOver m_creatureMouseOverInjure;

    [SerializeField] DialogScript m_dialogScript;
    [SerializeField] List<Skill> m_skillList = new List<Skill>();
    [SerializeField] SkillSelection m_skillSelection;
    [SerializeField] SkillSlots m_playerSkillSlots;
    // [SerializeField] SkillSlots m_creatureSkillSlots;

    int m_level = 1;
    int m_creatureIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        GenerateCreature();
        LoadCreature();
    }

    // Update is called once per frame
    void Update()
    {
        BattleUpdate();
    }

    void GenerateCreature(){
        m_board.Initialization();
        // m_player.m_HP = 150;
        m_creatureIndex = Random.Range(0, m_creatureList.Count);
    }

    void LoadCreature(){
        // retrieve data, set those wont be changed info
        Sprite creatureAvatar = m_creatureList[m_creatureIndex].m_avatar;
        string creatureName = m_creatureList[m_creatureIndex].name;
        int creatureLv = m_creatureList[m_creatureIndex].m_lv;
        string creatureDescription = m_creatureList[m_creatureIndex].m_description;
        int maxHP = m_creatureList[m_creatureIndex].m_maxHP;
        int attackType = m_creatureList[m_creatureIndex].m_attackType;
        Sprite attackTypeIcon = m_creatureList[m_creatureIndex].m_attackTypeIcon;
        int attackMultiplier = m_creatureList[m_creatureIndex].m_attackMultiplier;
        int attackFreq = m_creatureList[m_creatureIndex].m_attackFreq;
        Sprite injureTypeIcon = m_creatureList[m_creatureIndex].m_injureTypeIcon;
        int injureType = m_creatureList[m_creatureIndex].m_injureType;

        // Avatar Info: UI only
        m_creatureMouseOverAvatar.ChangeIcon(creatureAvatar);
        m_creatureMouseOverAvatar.m_name = creatureName;
        m_creatureMouseOverAvatar.m_level = "Lv. " + creatureLv;
        m_creatureMouseOverAvatar.m_effect = creatureDescription;

        // HP Info: logic only
        m_creature.m_HP = maxHP;
        m_creature.m_maxHP = maxHP;

        // Attack Info: both UI and logic
        m_player.m_injureType = attackType;
        m_player.m_injureMultiplier = attackMultiplier;
        m_player.m_injureFreq = attackFreq;
        m_player.m_stepCnt = 0;
        List<string> name = new List<string>{"Metal", "Wood", "Water", "Fire", "Earth"};
        m_creatureMouseOverAttack.ChangeIcon(attackTypeIcon);
        m_creatureMouseOverAttack.m_name = name[attackType] + " Attack";
        m_creatureMouseOverAttack.m_effect = "The damage caused by the creature depends on the number of <i>" + name[attackType] + "</i> tiles on the board.";

        // Injure Info: both UI and logic
        m_creature.m_injureType = injureType;
        m_creatureMouseOverInjure.ChangeIcon(injureTypeIcon);
        m_creatureMouseOverInjure.m_name = name[injureType] + " Weakness";
        m_creatureMouseOverInjure.m_effect = "The creature will take extra damage from the remove of <i>" + name[injureType] + "</i> tiles.";

        m_player.InitUI();
        m_creature.InitUI();
    }

    void GenerateLoadSkills(){
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

    int HPChecker(){
        // check the HP of the player and the creature to determine whether the current battle is done
        // return :
        //      0 : not finished yet
        //      1 : the player win
        //      2 : the player lose
        int playerHP = m_player.m_HP;
        int creatureHP = m_creature.m_HP;
        if(playerHP == 0){
            // Lose
            m_dialogScript.TurnOn(false);
            return 2;
        }
        if(creatureHP == 0){
            m_dialogScript.TurnOn(true);
            return 1;
        }
        return 0;
    }

    void BattleUpdate(){
        if(m_board.m_stepDone && !m_skillSelection.m_activated){
            int state = HPChecker();
            switch (state){
                case 0 :
                    break;
                case 1 :
                    GenerateLoadSkills();
                    GenerateCreature();
                    LoadCreature();
                    Debug.Log("You win.");
                    break;
                case 2:
                    Debug.Log("You lose.");
                    break;
            }
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
                m_player.m_maxHP += m_skillList[id].m_keyValue[m_skillList[id].m_lv];
                m_player.InitUI();
                m_playerSkillSlots.FillSkillSlot(m_skillList[id]);
                break;
        }
    }
}