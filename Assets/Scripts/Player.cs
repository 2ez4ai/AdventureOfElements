using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    GameObject m_board;

    // [SerializeField]
    // GameObject m_creature;

    Board m_boardScript;
    Creature m_creatureScript;
    int m_numColor;
    int m_numType;

    // for selection
    int m_numSelected = 0;

    int m_hp = 100;

    List<List<int>> m_lastMoveTiles = new List<List<int>>();    // the color and type of the tiles removed

    // Start is called before the first frame update
    void Start()
    {
        m_boardScript = m_board.GetComponent<Board>();
        m_numColor = m_boardScript.m_numColor;
        m_numType = m_boardScript.m_numType;
    }

    // Update is called once per frame
    void Update()
    {
        ClickMouse();
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
                    UpdateTilesSelection(hitObj);
                }
            }
        }
    }

    void UpdateTilesSelection(GameObject tile){
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

    public void AddLastMove(int c, int t){
        m_lastMoveTiles[c][t] += 1;
    }

    public void DealDamage(){
        // called only when drop is done
        InitLastMove();
    }
}
