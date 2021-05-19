using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastLogic : MonoBehaviour
{
    [SerializeField]
    GameObject m_board;

    Board m_boardScript;


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
                    Tile temp = hitObj.GetComponent<Tile>();
                    temp.m_selected = !temp.m_selected;
                }
            }
        }
    }
}
