using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    Board m_boardScript;

    Creature m_creatureScript;
    int m_numColor;
    int m_numType;

    // for selection
    int m_numSelected = 0;
    int stepCnt = 0;

    // for battle
    int m_HP = 100;
    [SerializeField]
    Text m_UIHP;
    [SerializeField]
    public int m_injureType = 0;    // changed by Creature
    [SerializeField]
    public int m_injureColor = -1;
    public int m_injureFreq = 4;    // how often an attack will be launched

    List<List<int>> m_lastMoveTiles = new List<List<int>>();    // the color and type of the tiles removed

    // Start is called before the first frame update
    void Start()
    {
        m_numColor = m_boardScript.m_numColor;
        m_numType = m_boardScript.m_numType;
    }

    // Update is called once per frame
    void Update()
    {
        ClickMouse();
        UTakeDamage();
    }

    void InitLastMove(){
        for(int i = 0; i < m_numColor; i++){
            for(int j = 0; j < m_numType; j++){
                // for each pair of color and type
                m_lastMoveTiles[i][j] = 0;
            }
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
    // battel things
    // ------------------------------------------------------------------------

    public void IncreStepCnt(){
        stepCnt += 1;
        Debug.Log("Step: " + stepCnt);
    }

    public void UTakeDamage(){
        if(stepCnt == m_injureFreq){
            stepCnt = 0;
            List<Tile> tiles = m_boardScript.TilesToPlayer();
            int damage = CntDamage(tiles);
            m_HP -= damage;
            m_UIHP.text = "HP : " + m_HP;
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

    public void DealDamage(){
        // called only when drop is done
        InitLastMove();
    }
}
