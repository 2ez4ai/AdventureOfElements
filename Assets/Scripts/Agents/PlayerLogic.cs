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

    // Start is called before the first frame update
    void Start()
    {
        m_numColor = m_boardScript.m_numColor;
        m_numType = m_boardScript.m_numType;
        UpdateIconTooltip(m_mouseOverAvatar, "You? Me? Whatever.", "Lv. 99", "Thanks for your play. Hope you enjoy this.", "Produced by Jingye Wang.");
        InitUI();
    }

    // Update is called once per frame
    void Update()
    {
        ClickMouse();
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
            string effect = m_gourdEffect + " Currently it saves <b>" + m_gourdHP + "</b> restoration.";
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
        Debug.Log("Disabled!");
        m_boardScript.m_stompSkillLogic.DisableTrailer();
        m_boardScript.GenerateStompArea(m_stompLevel);
    }

    public void SetBonus(int value){
        m_bonusHP += value;
    }

    public void SetGourdMaxHP(int prob, string effect, MouseOver mouseOverGourd){
        m_mouseOverGourd = mouseOverGourd;
        m_gourdEffect = effect;
        m_mouseOverGourd.m_effect = m_gourdEffect + " Currently it saves <b>" + m_gourdHP + "</b> restoration.";
        m_gourdProb = prob;
        m_gourdMaxHP = prob + (prob - 30) * 2;
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
        string effect = "Given the current number of <i>" + name[m_injureType] + "</i> tiles on the board, the next attack would cause " + damage + " or so damage.";
        UpdateIconTooltip(m_mouseOverSwordIcon, "Attack Type", "", effect);
    }

    void SetStepCntUI(){
        int step = m_injureFreq - m_stepCnt > 0? m_injureFreq - m_stepCnt: m_injureFreq;
        string stepInfo = step + "";
        m_textFreq.text = ": " + stepInfo;
        string effect = "The next attack will be lauched in " + stepInfo + " steps.";
        UpdateIconTooltip(m_mouseOverFreqIcon, "Attack Cooldown", "", effect);
    }

    void SetHPUI(){
        string hpInfo = m_HP + " / " + m_maxHP;
        m_UIHP.text = ": " + hpInfo;
        string effect = "Your current HP is " + hpInfo + ". You will lose the game when it is 0.";
        UpdateIconTooltip(m_UIHPIcon, "HP", "", effect);
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
