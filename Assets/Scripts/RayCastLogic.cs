using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastLogic : MonoBehaviour
{
    [SerializeField]
    GameObject m_board;

    Board m_boardScript;
    // at most two tiles can be selected at one time
    int m_numSelected = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_boardScript = m_board.GetComponent<Board>();
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
                m_numSelected = 0;
            }
            else{
                script.m_selected = true;
                m_numSelected = 1;
            }
        }
    }
}
