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
    int stepCnt = 0;

    // ------------------------------------------------------------------------
    // for battle
    // ------------------------------------------------------------------------
    // player
    public int m_HP = 100;
    int m_maxHP = 100;
    [SerializeField] Text m_UIHP;
    [SerializeField] MouseOver m_UIHPIcon;
    // creature damage & freq
    public int m_injureType = 0;    // changed by Controller
    public int m_injureMultiplier = 1;
    public int m_injureFreq = 4;    // how often an attack will be launched
    [SerializeField] Text m_textAttack;
    [SerializeField] MouseOver m_mouseOverSwordIcon;
    [SerializeField] Text m_textFreq;
    [SerializeField] MouseOver m_mouseOverFreqIcon;
    [SerializeField] MouseOver m_mouseOverAvatar;

    // Start is called before the first frame update
    void Start()
    {
        m_numColor = m_boardScript.m_numColor;
        m_numType = m_boardScript.m_numType;
        UpdateIconTooltip(m_mouseOverAvatar, "You? Me? Whatever.", "Lv. 99", "Producer: Jingye Wang. Thanks for your play. Hope you enjoy this.");
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

    // UI
    public void InitUI(){
        SetHPUI();
        SetStepCntUI();
        List<Tile> tiles = m_boardScript.TilesToPlayer();
        int damage = CntDamage(tiles);
        SetAttackUI(damage);
    }

    void UpdateIconTooltip(MouseOver script, string name, string level, string description){
        script.m_name = name;
        script.m_level = level;
        script.m_description = description;
    }

    void SetAttackUI(int damage){
        m_textAttack.text = "x " + m_injureMultiplier + " (" + damage + ")";
        List<string> name = new List<string>{"Metal", "Wood", "Water", "Fire", "Earth"};
        string description = "Given the current number of <i>" + name[m_injureType] + "</i> tiles on the board, the next attack would cause " + damage + " or so damage.";
        UpdateIconTooltip(m_mouseOverSwordIcon, "Attack Type", "", description);
    }

    void SetStepCntUI(){
        int step = m_injureFreq - stepCnt > 0? m_injureFreq - stepCnt: m_injureFreq;
        string stepInfo = step + "";
        m_textFreq.text = ": " + stepInfo;
        string description = "The next attack will be lauched in " + stepInfo + " steps.";
        UpdateIconTooltip(m_mouseOverFreqIcon, "Attack Cooldown", "", description);
    }

    void SetHPUI(){
        string hpInfo = m_HP + " / " + m_maxHP;
        m_UIHP.text = ": " + hpInfo;
        string description = "Your current HP is " + hpInfo + ". You will lose the game when it is 0.";
        UpdateIconTooltip(m_UIHPIcon, "HP", "", description);
    }

    public void IncreStepCnt(){
        stepCnt += 1;
        SetStepCntUI();
    }

    public void TakeDamage(){
        List<Tile> tiles = m_boardScript.TilesToPlayer();
        int damage = CntDamage(tiles);
        SetAttackUI(damage);
        if(stepCnt == m_injureFreq){
            stepCnt = 0;
            m_HP = m_HP - damage < 0? 0: m_HP-damage;
            SetHPUI();
            if(m_HP < 1){
                Debug.Log("You Lose!");
            }
        }
    }

    public int CntDamage(List<Tile> tiles){
        // cnt damage over tiles
        int damage = 0;
        foreach(Tile t in tiles){
            if(t.type == m_injureType || m_injureType == -1){
                damage += 1;
            }
        }
        return damage * m_injureMultiplier;
    }
}
