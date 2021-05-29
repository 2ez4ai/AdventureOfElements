using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    // ------------------------------------------------------------------------
    // use to control levels, skills.
    // ------------------------------------------------------------------------

    [SerializeField] PlayerLogic m_player;
    [SerializeField] CreatureLogic m_creature;
    [SerializeField] List<Creature> m_creatureList = new List<Creature>();

    [SerializeField] MouseOver m_creatureMouseOverAvatar;
    [SerializeField] MouseOver m_creatureMouseOverAttack;
    [SerializeField] MouseOver m_creatureMouseOverInjure;

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
        // LoadCreature();
    }

    void GenerateCreature(){
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
        m_creatureMouseOverAvatar.m_description = creatureDescription;

        // HP Info: logic only
        m_creature.m_HP = maxHP;
        m_creature.m_maxHP = maxHP;

        // Attack Info: both UI and logic
        m_player.m_injureType = attackType;
        m_player.m_injureMultiplier = attackMultiplier;
        m_player.m_injureFreq = attackFreq;
        List<string> name = new List<string>{"Metal", "Wood", "Water", "Fire", "Earth"};
        m_creatureMouseOverAttack.ChangeIcon(attackTypeIcon);
        m_creatureMouseOverAttack.m_name = name[attackType] + " Attack";
        m_creatureMouseOverAttack.m_description = "The damage caused by the creature depends on the number of <i>" + name[attackType] + "</i> tiles on the board.";

        // Injure Info: both UI and logic
        m_creature.m_injureType = injureType;
        m_creatureMouseOverInjure.ChangeIcon(injureTypeIcon);
        m_creatureMouseOverInjure.m_name = name[injureType] + " Weakness";
        m_creatureMouseOverInjure.m_description = "The creature will take extra damage from the remove of <i>" + name[injureType] + "</i> tiles.";

        m_player.InitUI();
        m_creature.InitUI();
    }
}
