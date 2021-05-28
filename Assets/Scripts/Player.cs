using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] Board m_boardScript;

    CreatureLogic m_creatureScript;
    int m_numColor;
    int m_numType;

    // for selection
    int m_numSelected = 0;
    int stepCnt = 0;

    // for battle
    int m_HP = 100;
    int m_maxHP = 100;
    [SerializeField] Text m_UIHP;
    [SerializeField] MouseOver m_UIHPIcon;
    [SerializeField] public int m_injureType = 0;    // changed by Creature
    [SerializeField] public int m_injureMultiplier = 1;
    [SerializeField] Text m_UIInjureMultiplier;
    [SerializeField] MouseOver m_UIInjureIcon;
    public int m_injureColor = -1;
    public int m_injureFreq = 4;    // how often an attack will be launched
    [SerializeField] Text m_UIFreq;
    [SerializeField] MouseOver m_UIFreqIcon;

    List<List<int>> m_lastMoveTiles = new List<List<int>>();    // the color and type of the tiles removed

    // Start is called before the first frame update
    void Start()
    {
        m_numColor = m_boardScript.m_numColor;
        m_numType = m_boardScript.m_numType;
        InitUI();
    }

    // Update is called once per frame
    void Update()
    {
        ClickMouse();
        UTakeDamage();
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
    void InitUI(){
        SetInjureUI();
        SetStepCntUI();
        SetHPUI();
    }

    void UpdateIconTooltip(MouseOver script, string name, string level, string description, int iconIndex = -1){
        script.m_name = name;
        script.m_level = level;
        script.m_description = description;
        if(iconIndex != -1){
            script.ChangeIcon(iconIndex);
        }
    }

    void SetInjureUI(){
        m_UIInjureMultiplier.text = "x " + m_injureMultiplier;
        List<string> name = new List<string>{"Metal", "Wood", "Water", "Fire", "Earth"};
        string description = "The damage caused by the creature depends on the number of <i>" + name[m_injureType] + "</i> tiles.";
        UpdateIconTooltip(m_UIInjureIcon, "Attack Type", "", description, m_injureType);
    }

    void SetStepCntUI(){
        int step = m_injureFreq - stepCnt > 0? m_injureFreq - stepCnt: m_injureFreq;
        string stepInfo = step + "";
        m_UIFreq.text = ": " + stepInfo;
        string description = "The next attack will be lauched in " + stepInfo + " steps.";
        UpdateIconTooltip(m_UIFreqIcon, "Attack Cooldown", "", description);
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

    public void UTakeDamage(){
        if(stepCnt == m_injureFreq){
            stepCnt = 0;
            List<Tile> tiles = m_boardScript.TilesToPlayer();
            int damage = CntDamage(tiles);
            m_HP -= damage;
            SetHPUI();
            if(m_HP < 1){
                Debug.Log("You Lose!");
            }
        }
    }

    public int CntDamage(List<Tile> tiles){
        int damage = 0;
        foreach(Tile t in tiles){
            if(t.color == m_injureColor || m_injureColor == -1){
                if(t.type == m_injureType || m_injureType == -1){
                    damage += 1;
                }
            }
        }
        return damage;
    }

    public void AddLastMove(int c, int t){
        m_lastMoveTiles[c][t] += 1;
    }
}
