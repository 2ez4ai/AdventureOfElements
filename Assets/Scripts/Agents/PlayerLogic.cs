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

    // skill
    int m_bonusHP = 0;
    // gourd
    MouseOver m_mouseOverGourd;
    string m_gourdEffect;
    int m_gourdHP = 0;
    int m_gourdProb = 0;
    int m_gourdMaxHP = -1;
    // stomp
    int m_stompLevel = 0;

    Language m_language;

    // Start is called before the first frame update
    void Start()
    {
        m_numColor = m_boardScript.m_numColor;
        m_numType = m_boardScript.m_numType;
        m_language = LocalizationManager.m_instance.GetLanguage();
        string playerName = LocalizationManager.m_instance.GetLocalisedString("PlayerName");
        string LV = LocalizationManager.m_instance.GetLocalisedString("LV") + " 99";
        string playerDescription = LocalizationManager.m_instance.GetLocalisedString("PlayerDescription");
        string playerRemark = LocalizationManager.m_instance.GetLocalisedString("PlayerRemark");
        UpdateIconTooltip(m_mouseOverAvatar, playerName, LV, playerDescription, playerRemark);
        InitUI();
    }

    // Update is called once per frame
    void Update()
    {
        ClickMouse();
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
        m_boardScript.m_specialSkill = level;
    }

    public void LearnDiagonal(int level){
        m_boardScript.m_diagonalSwapLV = level;
    }

    public void LearnGourd(int regenHP){
        int temp = m_HP + regenHP + m_bonusHP;
        m_HP = temp > m_maxHP ? m_maxHP : temp;
        if(temp > m_maxHP && m_gourdMaxHP != -1){
            int temp2 = temp - m_maxHP + m_gourdHP;
            m_gourdHP = temp2 > m_gourdMaxHP ? m_gourdMaxHP : temp2;
            string effect = m_gourdEffect + " " + LocalizationManager.m_instance.GetLocalisedString("EscapeGourdEffectPart1") + " <b>" + m_gourdHP + "</b> " + LocalizationManager.m_instance.GetLocalisedString(LocalizationManager.m_instance.GetLocalisedString("EscapeGourdEffectPart2"));
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
        // Debug.Log("Disabled!");
        m_boardScript.m_stompSkillLogic.DisableTrailer();
        m_boardScript.GenerateStompArea(m_stompLevel);
    }

    public void SetBonus(int value){
        m_bonusHP += value;
    }

    public void SetGourdMaxHP(int proportion, string effect, MouseOver mouseOverGourd){
        m_mouseOverGourd = mouseOverGourd;
        m_gourdEffect = effect;
        m_mouseOverGourd.m_effect = m_gourdEffect + " " + LocalizationManager.m_instance.GetLocalisedString("EscapeGourdEffectPart1") + " <b>" + m_gourdHP + "</b> " + LocalizationManager.m_instance.GetLocalisedString(LocalizationManager.m_instance.GetLocalisedString("EscapeGourdEffectPart2"));
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
}
