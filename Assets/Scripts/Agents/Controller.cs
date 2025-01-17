using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    // ------------------------------------------------------------------------
    // use to control levels, skills.
    // ------------------------------------------------------------------------
    [SerializeField] Board m_board;
    [SerializeField] PlayerLogic m_player;
    [SerializeField] CreatureLogic m_creature;
    [SerializeField] List<Creature> m_creatureList = new List<Creature>();
    [SerializeField] GameObject m_gameObjWhite;
    [SerializeField] GameObject m_gameObjBlue;
    [SerializeField] GameObject m_gameObjRed;

    [SerializeField] MouseOver m_creatureMouseOverAvatar;
    [SerializeField] MouseOver m_creatureMouseOverAttack;
    [SerializeField] MouseOver m_creatureMouseOverInjure;
    [SerializeField] MouseOver m_creatureMouseOverInjureIcon;
    [SerializeField] SkillSlots m_creatureSkillSlots;

    [SerializeField] DialogScript m_dialogScript;
    // if win
    [SerializeField] SkillController m_skillController;
    // if lose
    [SerializeField] LoseButton m_loseBtn;
    [SerializeField] Text m_best;

    [SerializeField] Collider m_blocker;

    int m_level = 1;    // current map level
    [SerializeField] GameObject m_levelReminder;
    Text m_levelText;
    int m_creatureIndex = 0;
    bool m_creatureLoadReady = false;

    float m_firstLoadingDuration;
    bool m_loadingFinish = false;

    bool m_loaded;

    // Start is called before the first frame update
    void Start()
    {
        m_loaded = false;
        if(LocalizationManager.m_instance.loadChecker){
            m_level = PlayerPrefs.GetInt("Level");
            m_creatureIndex = PlayerPrefs.GetInt("CreatureIndex");
            GenerateCreatureIndex(m_creatureIndex);
        }
        else{
            GenerateCreatureIndex();
        }
        m_levelReminder.SetActive(true);
        m_levelText = m_levelReminder.GetComponent<Text>();
        LocalizationManager.m_instance.SetLocalisedFont(m_levelText);
        m_levelText.text = LocalizationManager.m_instance.GetLocalisedString("Level") + " " + m_level;
        m_blocker.enabled = true;
        m_firstLoadingDuration = 1.5f;
        LoadCreature();
    }

    // Update is called once per frame
    void Update()
    {
        m_firstLoadingDuration -= Time.deltaTime;
        if(!m_loadingFinish && m_firstLoadingDuration < 0){
            m_loadingFinish = true;
            m_levelReminder.SetActive(false);
            m_blocker.enabled = false;
        }
        ULoadCreature();
        BattleUpdate();
    }

    void GenerateCreatureIndex(int load=-1){
        int randomSeed = -1;
        if(LocalizationManager.m_instance.loadChecker && !m_loaded){
            randomSeed = PlayerPrefs.GetInt("RandomSeed");
            m_loaded = true;
        }
        m_board.Initialization(randomSeed);
        m_creature.m_weakPointDamage = m_level / 5 + 1;
        if(load == -1){
            if(m_level < 10){
                m_creatureIndex = m_level - 1;
            }
            else{
                m_creatureIndex = Random.Range(0, m_creatureList.Count);
            }
        }
        else{
            m_creatureIndex = load;
        }
    }

    void LoadCreature(){
        // retrieve data, set info that wont be changed
        Sprite creatureAvatar = m_creatureList[m_creatureIndex].m_avatar;
        string creatureName = m_creatureList[m_creatureIndex].m_name;
        int creatureLv = m_creatureList[m_creatureIndex].m_lv;
        int maxHP = m_creatureList[m_creatureIndex].m_maxHP;
        int attackType = m_creatureList[m_creatureIndex].m_attackType;
        Sprite attackTypeIcon = m_creatureList[m_creatureIndex].m_attackTypeIcon;
        int attackMultiplier = m_creatureList[m_creatureIndex].m_attackMultiplier;
        int attackFreq = m_creatureList[m_creatureIndex].m_attackFreq;
        Sprite injureTypeIcon = m_creatureList[m_creatureIndex].m_injureTypeIcon;
        int injureType = m_creatureList[m_creatureIndex].m_injureType;
        List<Skill> creatureSkills = m_creatureList[m_creatureIndex].m_skills;

        if(m_level > 9){
            // start randomly generating
            maxHP += (m_level - creatureLv) * 4;
            attackMultiplier += (m_level - creatureLv) / 8;
        }

        // Avatar Info: UI only
        m_creatureMouseOverAvatar.ChangeIcon(creatureAvatar);
        m_creatureMouseOverAvatar.m_name = LocalizationManager.m_instance.GetLocalisedString(creatureName);
        m_creatureMouseOverAvatar.m_level = LocalizationManager.m_instance.GetLocalisedString("LV") + " " + creatureLv;
        m_creatureMouseOverAvatar.m_effect = LocalizationManager.m_instance.GetLocalisedString(creatureName+"Description");

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
        m_creatureMouseOverAttack.m_name = LocalizationManager.m_instance.GetLocalisedString(name[attackType]) + LocalizationManager.m_instance.GetLocalisedString("Attack");
        m_creatureMouseOverAttack.m_effect = LocalizationManager.m_instance.GetLocalisedString("AtkDescriptionPart1") + "<i>" + LocalizationManager.m_instance.GetLocalisedString(name[attackType]) + "</i>" + LocalizationManager.m_instance.GetLocalisedString("AtkDescriptionPart2");

        // Injure Info: both UI and logic
        m_creature.m_injureType = injureType;
        m_creatureMouseOverInjureIcon.m_name = LocalizationManager.m_instance.GetLocalisedString("Weakness1");
        m_creatureMouseOverInjureIcon.m_effect = LocalizationManager.m_instance.GetLocalisedString("InjureDescription");
        m_creatureMouseOverInjure.ChangeIcon(injureTypeIcon);
        m_creatureMouseOverInjure.m_name = LocalizationManager.m_instance.GetLocalisedString(name[injureType]) + LocalizationManager.m_instance.GetLocalisedString("Weakness");
        m_creatureMouseOverInjure.m_effect = LocalizationManager.m_instance.GetLocalisedString("InjureDescriptionPart1") + "<i>" + LocalizationManager.m_instance.GetLocalisedString(name[injureType]) + "</i>" + LocalizationManager.m_instance.GetLocalisedString("InjureDescriptionPart2");

        // Trigger Skill
        m_creatureSkillSlots.CleanSlots();
        m_skillController.CleanCreatureSkills();
        foreach(Skill s in creatureSkills){
            m_creatureSkillSlots.FillSkillSlot(s);
            m_skillController.CreatureSkillTrigger(s);
        }

        m_player.InitUI();
        m_creature.InitUI();
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
        if(m_board.m_stepDone && !m_skillController.SkillSelectionActivation() && !m_loseBtn.m_activated){
            int state = HPChecker();
            switch (state){
                case 0 :
                    break;
                case 1 :
                    SoundManager.m_instance.PlayWinSound();
                    m_skillController.GenerateLoadSkills(m_level);
                    m_level ++;
                    BoardShrink();
                    GenerateCreatureIndex();
                    m_creature.m_HP = 1;
                    m_player.controllable = false;
                    Debug.Log("You win.");
                    break;
                case 2:
                    SoundManager.m_instance.PlayLoseSound();
                    m_loseBtn.m_activated = true;
                    m_player.m_HP = 1;
                    m_player.controllable = false;
                    int bestLv = PlayerPrefs.GetInt("BestLv");
                    if(bestLv < m_level){
                        SaveBest();;
                        bestLv = m_level;
                    }
                    m_best.text = "Your best: level " + bestLv;
                    Debug.Log("You lose.");
                    break;
            }
        }
    }

    void BoardShrink(){
        m_board.m_stompSkillLogic.DisableTrailer();
        m_board.m_isShrinking = true;
        m_blocker.enabled = true;
    }

    public void BoardExpand(){
        m_board.m_isExpanding = true;
        m_levelReminder.SetActive(true);
        LocalizationManager.m_instance.SetLocalisedFont(m_levelText);
        m_levelText.text = LocalizationManager.m_instance.GetLocalisedString("Level") + " " + m_level;
        m_creatureLoadReady = true;
        m_blocker.enabled = true;
    }

    void ULoadCreature(){
        if(m_creatureLoadReady && !m_board.m_isExpanding){
            m_creatureLoadReady = false;
            m_levelReminder.SetActive(false);
            m_blocker.enabled = false;
            LoadCreature();
        }
    }

    public void SaveData(){
        PlayerPrefs.SetInt("Level", m_level);
        PlayerPrefs.SetInt("CreatureIndex", m_creatureIndex);
        PlayerPrefs.SetInt("RandomSeed", m_board.randomSeed);
    }

    public void SaveBest(){
        PlayerPrefs.SetInt("BestLv", m_level);
    }
}
