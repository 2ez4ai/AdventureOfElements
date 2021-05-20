using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniVariable{
    // animation variable
    public bool trigger = false;
    public bool doing = false;
    public bool drop = false;
    public Vector3 target;

    public float moveSpeed = 7.0f;
    public float dropSpeed = 2.0f;
}

public class TileLogic : MonoBehaviour
{
    [SerializeField]
    GameObject m_typer;
    [SerializeField]
    List<Material> m_colorList = new List<Material>();
    [SerializeField]
    List<Material> m_typeList = new List<Material>();

    MeshRenderer m_colorRender;
    MeshRenderer m_typeRender;
    Vector3 m_position;

    public bool m_selected = false;
    public bool m_hang = false;

    // fluctuation
    float m_bound = 0.2f;
    float m_flucStep = 0.01f;
    bool m_selectActivation = false;    // whether the obj is already protruding

    // swap animation
    public AniVariable m_moveAniV = new AniVariable();    // the swap and drop would not be done simultaneously
    /*
    public bool m_movingTrigger = false;    // make a move
    bool m_isMoving = false;    // whether the move is finished
    public Vector3 m_swapTarget;
    */

    // Start is called before the first frame update
    void Start()
    {
        m_colorRender = GetComponent<MeshRenderer>();
        m_typeRender = m_typer.GetComponent<MeshRenderer>();
        m_position = transform.position;
    }

    // Update is called once per frame
    float timer = 0.0f;
    float gap = 1.0f;
    int i = 0;

    void Update()
    {
        USelected();
        UAniMove();
    }

    public void SetColor(int index){
        m_colorRender.material = m_colorList[index];
    }

    public void SetType(int index){
        m_typeRender.material = m_typeList[index];
    }

    void USelected(){
        if(m_selected && !m_selectActivation){
            transform.position = new Vector3(1.25f, transform.position.y, transform.position.z);
            m_selectActivation = true;
        }
        if(!m_selected && m_selectActivation){
            transform.position = new Vector3(0.0f, transform.position.y, transform.position.z);
            m_selectActivation = false;
        }
        if(m_selected){
            // fluctuation
            transform.position = new Vector3(transform.position.x + m_flucStep, transform.position.y, transform.position.z);
            if (transform.position.x < 1.25f - m_bound || transform.position.x > 1.25f + m_bound)
            {
                m_flucStep *= -1.0f;
            }
        }
    }

    void UAniMove(){
        if(m_moveAniV.trigger && !m_moveAniV.doing){
            m_moveAniV.doing = true;
        }
        if(m_moveAniV.doing){
            AniTileMove();
        }
    }

    void AniTileMove(){
        // move tile a to the target
        float speed = m_moveAniV.moveSpeed;
        if(m_moveAniV.drop){
            speed = m_moveAniV.dropSpeed;
        }
        transform.position = Vector3.MoveTowards(transform.position, m_moveAniV.target, speed * Time.deltaTime);
        if(Vector3.Distance(transform.position, m_moveAniV.target) < 0.01f){
            m_moveAniV.doing = false;
            m_moveAniV.trigger = false;
            m_moveAniV.drop = false;
            transform.position = m_position;
        }
    }
}
