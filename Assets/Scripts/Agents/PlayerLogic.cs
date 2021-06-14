using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLogic : MonoBehaviour
{
    [SerializeField] Board m_boardScript;

    // CreatureLogic m_creatureScript;
    int m_numColor;
    int m_numType;

    // for selection
    int m_numSelected = 0;

    // ------------------------------------------------------------------------
    // for battle
    // ------------------------------------------------------------------------
    // player
    public int m_HP = 100;
    public int m_maxHP = 100;
    [SerializeField] Text m_UIHP;
    [SerializeField] MouseOver m_UIHPIcon;

    // creature damage & freq
    public int m_injureType = 0;    // changed by Controller
    public int m_injureMultiplier = 1;
    public int m_stepCnt = 0;
    public int m_injureFreq = 4;    // how often an attack will be launched
    [SerializeField] Text m_textAttack;
    [SerializeField] MouseOver m_mouseOverSwordIcon;
    [SerializeField] Text m_textFreq;
    [SerializeField] MouseOver m_mouseOverFreqIcon;
    [SerializeField] MouseOver m_mouseOverAvatar;

    const int k_row = 8;
    const int k_col = 8;
    List<Tile> m_tilesForController = new List<Tile>();
    int m_controllerX;
    int m_controllerY;
    const float k_maxControllerCooldown = 0.5f;
    const float k_maxControllerActivation = 3.0f;
    float m_controllerCooldownTimer;
    float m_controllerActivationTimer;
    public bool controllable;

    // skill
    int m_bonusHP = 0;
    // gourd
    MouseOver m_mouseOverGourd;
    string m_gourdEffect;
    int m_gourdHP = 0;
    int m_gourdProb = 0;
    int m_gourdMaxHP = -1;
    // special
    int m_specialLevel = -1;
    // diagonal
    int m_diagonalLevel = -1;
    // stomp
    int m_stompLevel = 0;

    Language m_language;

    // Start is called before the first frame update
    void Start()
    {
        m_numColor = m_boardScript.m_numColor;
        m_numType = m_boardScript.m_numType;
        m_tilesForController = m_boardScript.TilesToPlayer();
        m_controllerX = 0;
        m_controllerY = 0;
        m_controllerCooldownTimer = k_maxControllerCooldown;
        m_controllerActivationTimer = 0.0f;
        controllable = true;
        m_language = LocalizationManager.m_instance.GetLanguage();
        string playerName = LocalizationManager.m_instance.GetLocalisedString("PlayerName");
        string LV = LocalizationManager.m_instance.GetLocalisedString("LV") + " 99";
        string playerDescription = LocalizationManager.m_instance.GetLocalisedString("PlayerDescription");
        string playerRemark = LocalizationManager.m_instance.GetLocalisedString("PlayerRemark");
        UpdateIconTooltip(m_mouseOverAvatar, playerName, LV, playerDescription, playerRemark);
        if(LocalizationManager.m_instance.loadChecker){
            Debug.Log("Load player.");
            m_HP = 20;
            m_maxHP = 200;
        }
        InitUI();
    }

    // Update is called once per frame
    void Update()
    {
        ClickMouse();
        Controller();    // for controller
        CheckLanguage();
    }

    void CheckLanguage(){
        Language tempLanguage = LocalizationManager.m_instance.GetLanguage();
        if(tempLanguage == m_language){
            return;
        }
        else{
            m_language = tempLanguage;
            string playerName = LocalizationManager.m_instance.GetLocalisedString("PlayerName");
            string LV = LocalizationManager.m_instance.GetLocalisedString("LV") + " 99";
            string playerDescription = LocalizationManager.m_instance.GetLocalisedString("PlayerDescription");
            string playerRemark = LocalizationManager.m_instance.GetLocalisedString("PlayerRemark");
            UpdateIconTooltip(m_mouseOverAvatar, playerName, LV, playerDescription, playerRemark);
        }
    }

    void ClickMouse(){
        if(Input.GetButtonDown("Fire1")){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;

            if(Physics.Raycast(ray, out raycastHit, 50.0f)){
                GameObject hitObj = raycastHit.collider.gameObject;
                if(hitObj.tag == "Tiles"){
                    TilesSelection(hitObj);
                }
            }
        }
    }

    void Controller(){
        if(controllable && m_controllerCooldownTimer < 0.0f){
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            bool changed = false;
            int oldIndex = m_boardScript.RCToIndex(m_controllerX, m_controllerY);
            // every time we only allow one direction move
            if(horizontalInput > 0.2f){
                if(m_boardScript.RCInBoard(m_controllerX, m_controllerY+1)){
                    m_controllerY += 1;
                    changed = true;
                }
            }
            else if(horizontalInput < -0.2f){
                if(m_boardScript.RCInBoard(m_controllerX, m_controllerY-1)){
                    m_controllerY -= 1;
                    changed = true;
                }
            }
            else if(verticalInput > 0.2f){
                if(m_boardScript.RCInBoard(m_controllerX+1, m_controllerY)){
                    m_controllerX += 1;
                    changed = true;
                }
            }
            else if(verticalInput < -0.2f){
                if(m_boardScript.RCInBoard(m_controllerX-1, m_controllerY)){
                    m_controllerX -= 1;
                    changed = true;
                }
            }

            if(changed){
                ControllerOnTile(oldIndex, false);
                ControllerOnTile(m_boardScript.RCToIndex(m_controllerX, m_controllerY), true);
                m_controllerCooldownTimer = k_maxControllerCooldown;
                m_controllerActivationTimer = k_maxControllerActivation;    // there is an operation
            }
        }
        else{
            m_controllerCooldownTimer -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Fire1") && m_controllerActivationTimer > 0.0f){
            TilesSelection(m_tilesForController[m_boardScript.RCToIndex(m_controllerX, m_controllerY)].tile);
        }

        m_controllerActivationTimer -= Time.deltaTime;
    }

    void ControllerOnTile(int index, bool activate){
        Vector3 temp = m_tilesForController[index].tile.transform.position;
        if(activate){
            if(!m_tilesForController[index].logic.m_selected){
                temp.x = 0.85f;
            }
        }
        else{
            if(!m_tilesForController[index].logic.m_selected){
                temp.x = 0.0f;
            }
        }
        m_tilesForController[index].tile.transform.position = temp;
    }

    void TilesSelection(GameObject tile){
        TileLogic script = tile.GetComponent<TileLogic>();
        if(!script.m_selected){
            m_numSelected += 1;
            script.m_selected = true;
        }
        else{
            m_numSelected -= 1;
            script.m_selected = false;
        }

        if(m_numSelected == 2){
            if(m_boardScript.IsValidSwap()){
                m_boardScript.AniTileSwap();    // play animation for the swap
                m_numSelected = 0;
            }
            else{
                script.m_selected = true;
                m_numSelected = 1;
            }
        }
    }

    // ------------------------------------------------------------------------
    // battle things
    // ------------------------------------------------------------------------

    // Skill
    public void LearnSpecial(int level){
        m_specialLevel = level;
        m_boardScript.m_specialSkill = level;
    }

    public void LearnDiagonal(int level){
        m_diagonalLevel = level;
        m_boardScript.m_diagonalSwapLV = level;
    }

    public void RegenHP(int regenHP){
        int temp = m_HP + regenHP + m_bonusHP;
        m_HP = temp > m_maxHP ? m_maxHP : temp;
        if(temp > m_maxHP && m_gourdMaxHP != -1){
            int temp2 = temp - m_maxHP + m_gourdHP;
            m_gourdHP = temp2 > m_gourdMaxHP ? m_gourdMaxHP : temp2;
            string effect = m_gourdEffect + " " + LocalizationManager.m_instance.GetLocalisedString("EscapeGourdEffectPart1") + " <b>" + m_gourdHP + "</b> " + LocalizationManager.m_instance.GetLocalisedString("EscapeGourdEffectPart2");
            UpdateIconTooltip(m_mouseOverGourd, m_mouseOverGourd.m_name, m_mouseOverGourd.m_level, effect, m_mouseOverGourd.m_description);
        }
        SetHPUI();
    }

    public void LearnStomp(int level){
        // this is a skill of creatures actually
        m_stompLevel = level;
        StompActivation();
    }

    void StompActivation(){
        m_boardScript.m_stompSkillLogic.DisableTrailer();
        m_boardScript.GenerateStompArea(m_stompLevel);
    }

    public void SetBonus(int value){
        m_bonusHP += value;
    }

    public void SetGourdMaxHP(int proportion, string effect, MouseOver mouseOverGourd){
        m_mouseOverGourd = mouseOverGourd;
        m_gourdEffect = effect;
        m_mouseOverGourd.m_effect = m_gourdEffect + " " + LocalizationManager.m_instance.GetLocalisedString("EscapeGourdEffectPart1") + " <b>" + m_gourdHP + "</b> " + LocalizationManager.m_instance.GetLocalisedString("EscapeGourdEffectPart2");
        m_gourdProb = proportion;
        m_gourdMaxHP = proportion + (proportion - 30) * 2;
    }

    // UI
    public void InitUI(){
        SetHPUI();
        SetStepCntUI();
        List<Tile> tiles = m_boardScript.TilesToPlayer();
        int damage = CntDamage(tiles);
        SetAttackUI(damage);
    }

    void UpdateIconTooltip(MouseOver script, string name, string level, string effect, string description = ""){
        script.m_name = name;
        script.m_level = level;
        script.m_effect = effect;
        script.m_description = description;
    }

    void SetAttackUI(int damage){
        m_textAttack.text = "x " + m_injureMultiplier + " (" + damage + ")";
        List<string> name = new List<string>{"Metal", "Wood", "Water", "Fire", "Earth"};
        string effect = LocalizationManager.m_instance.GetLocalisedString("AtkTypePart1") + "<i>" + LocalizationManager.m_instance.GetLocalisedString(name[m_injureType]) + "</i>" + LocalizationManager.m_instance.GetLocalisedString("AtkTypePart2") + damage + LocalizationManager.m_instance.GetLocalisedString("AtkTypePart3");
        UpdateIconTooltip(m_mouseOverSwordIcon, LocalizationManager.m_instance.GetLocalisedString("AtkType"), "", effect);
    }

    void SetStepCntUI(){
        int step = m_injureFreq - m_stepCnt > 0? m_injureFreq - m_stepCnt: m_injureFreq;
        string stepInfo = step + "";
        m_textFreq.text = ": " + stepInfo;
        string effect = LocalizationManager.m_instance.GetLocalisedString("AtkCooldownPart1") + stepInfo + LocalizationManager.m_instance.GetLocalisedString("AtkCooldownPart2");
        UpdateIconTooltip(m_mouseOverFreqIcon, LocalizationManager.m_instance.GetLocalisedString("AttackCooldown"), "", effect);
    }

    void SetHPUI(){
        string hpInfo = m_HP + " / " + m_maxHP;
        m_UIHP.text = ": " + hpInfo;
        string effect = LocalizationManager.m_instance.GetLocalisedString("HPPart1") + hpInfo + LocalizationManager.m_instance.GetLocalisedString("HPPart2");
        UpdateIconTooltip(m_UIHPIcon, LocalizationManager.m_instance.GetLocalisedString("HP"), "", effect);
    }

    public void IncreStepCnt(){
        m_stepCnt += 1;
        SetStepCntUI();
    }

    public void TakeDamage(){
        List<Tile> tiles = m_boardScript.TilesToPlayer();
        int damage = CntDamage(tiles);
        SetAttackUI(damage);
        if(m_stepCnt == m_injureFreq){
            m_stepCnt = 0;
            if(m_stompLevel != 0){
                StompActivation();
            }
            m_HP = m_HP - damage < 0? 0: m_HP-damage;
            SetHPUI();
            // if(m_HP < 1){
            //     Debug.Log("You Lose!");
            // }
        }
    }

    public int CntDamage(List<Tile> tiles){
        // cnt damage over tiles
        int damage = 0;
        foreach(Tile t in tiles){
            if(t.type == m_injureType || m_injureType == -1){
                damage = damage + 1 + t.logic.m_stompDamage;
            }
        }
        return damage * m_injureMultiplier;
    }

    public void SaveData(){
        // save HP
        PlayerPrefs.SetInt("HP", m_HP);
        // save maxHP
        PlayerPrefs.SetInt("MaxHP", m_maxHP);
        // skill bonus
        PlayerPrefs.SetInt("BonusHP", m_bonusHP);
        // skill gourd
        PlayerPrefs.SetInt("AcquiredGourd", m_gourdMaxHP);
        PlayerPrefs.SetInt("GourdHP", m_gourdHP);
        PlayerPrefs.SetInt("GourdProb", m_gourdProb);
        // skill special
        PlayerPrefs.SetInt("SkillSpecialLevel", m_specialLevel);
        // skill diagonal
        PlayerPrefs.SetInt("SkillDiagonalLevel", m_diagonalLevel);
        // skill stomp
        PlayerPrefs.SetInt("SkillStompLevel", m_stompLevel);
    }

    public void LoadData(){
        m_HP = PlayerPrefs.GetInt("HP");
        m_maxHP = PlayerPrefs.GetInt("MaxHP");
        m_bonusHP = PlayerPrefs.GetInt("BonusHP");
        // load gourd
        m_gourdMaxHP = PlayerPrefs.GetInt("AcquiredGourd");
        if(m_gourdMaxHP != -1){
            m_gourdHP = PlayerPrefs.GetInt("GourdHP");
            m_gourdProb = PlayerPrefs.GetInt("GourdProb");
        }
        LearnSpecial(PlayerPrefs.GetInt("SkillSpecialLevel"));
        LearnDiagonal(PlayerPrefs.GetInt("SkillDiagonalLevel"));
        LearnStomp(PlayerPrefs.GetInt("SkillStompLevel"));
    }
}
