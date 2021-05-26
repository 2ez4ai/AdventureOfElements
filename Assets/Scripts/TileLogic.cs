using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniMoveVariable{
    // animation variable
    public bool trigger = false;
    public bool doing = false;
    public Vector3 target;

    public float moveSpeed = 5.0f;
    public float dropSpeed = 10.0f;

    public GameObject dropTarget;
}

public class TileLogic : MonoBehaviour
{
    [SerializeField]
    GameObject m_typer;
    [SerializeField]
    List<Material> m_colorMat = new List<Material>();
    [SerializeField]
    List<Material> m_typeMat = new List<Material>();
    [SerializeField]
    List<Material> m_effectMat = new List<Material>();    // effect

    MeshRenderer m_colorRender;
    MeshRenderer m_typeRender;
    int m_colorIndex;
    int m_typeIndex;
    Vector3 m_position;

    public bool m_selected = false;
    bool m_swing = false;
    public bool m_hang = false;

    // fluctuation
    float m_flucBound = 0.4f;
    float m_flucStep = 0.02f;
    bool m_selectActivation = false;    // whether the obj is protruding

    // swing
    float m_swingBound = 0.1f;
    float m_swingStep = 0.5f;
    bool m_swingActivation = false;

    // swap animation
    public AniMoveVariable m_moveAniV = new AniMoveVariable();    // the swap and drop would not be done simultaneously

    // drop animation
    public bool m_drop = false;
    public bool m_newTile = false;    // generate new Tile here

    // remove animation
    bool m_remove = false;
    public bool m_removeDone = true;
    float m_removeTimer = 0.0f;
    float m_removeTime = 0.3f;

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
        USwing();
        UAniRemove();
        UAniTileGenerate();
    }

    public void SetColor(int index){
        m_colorIndex = index;
        m_colorRender.material = m_colorMat[m_colorIndex];
    }

    public void SetType(int index){
        m_typeIndex = index;
        m_typeRender.material = m_typeMat[m_typeIndex];
    }

    public void SetRemoveState(bool state){
        // if true, means it is removed
        // actually, no parameter is needed
        // m_colorRender.enabled = !state;
        if(state){
            m_remove = true;
            m_removeDone = false;
        }
        else{
            m_colorRender.enabled = true;
            m_typeRender.enabled = true;
        }
    }

    public void SetSwing(bool state){
        m_swing = state;
        if(state){
            m_swingStep = 0.5f;
        }
        else{
            m_swing = false;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public int GetColorIndex(){
        return m_colorIndex;
    }

    public int GetTypeIndex(){
        return m_typeIndex;
    }

    void USelected(){
        if(m_selected && !m_selectActivation){
            // protrude at 1.25
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
            if (transform.position.x < 1.25f - m_flucBound || transform.position.x > 1.25f + m_flucBound)
            {
                m_flucStep *= -1.0f;
            }
        }
    }

    void USwing(){
        if(m_swing){
            if(m_selected){
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if(m_moveAniV.trigger || m_drop){
                m_swing = false;    // stop
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else{
                transform.Rotate(new Vector3(m_swingStep, 0, 0));
                if (transform.rotation.x < - m_swingBound || transform.rotation.x > m_swingBound)
                {
                    m_swingStep *= -1.0f;
                }
            }
        }
    }

    void UAniMove(){
        if(m_moveAniV.trigger && !m_moveAniV.doing){
            m_moveAniV.doing = true;
        }
        if(m_moveAniV.doing){
            if(m_drop){
                AniTileDrop();
            }
            else{
                AniTileSwap();
            }
        }
    }

    void UAniRemove(){
        if(m_remove){
            // this tile is going to be removed
            m_colorRender.material = m_effectMat[m_typeIndex];
            m_removeTimer = m_removeTime;
            m_remove = false;
        }
        m_removeTimer -= Time.deltaTime;

        if(!m_removeDone && m_removeTimer < 0){
            Debug.Log("Remove done!");
            m_colorRender.material = m_colorMat[m_colorIndex];
            m_colorRender.enabled = false;
            m_typeRender.enabled = false;
            m_removeDone = true;    // start drop

        }
    }

    void UAniTileGenerate(){
        if(m_newTile){
            AniTileGenerate();
        }
    }

    void AniTileSwap(){
        // move tile a to the target
        float speed = m_moveAniV.moveSpeed;
        transform.position = Vector3.MoveTowards(transform.position, m_moveAniV.target, speed * Time.deltaTime);
        if(Vector3.Distance(transform.position, m_moveAniV.target) < 0.01f){
            m_moveAniV.doing = false;
            m_moveAniV.trigger = false;
            transform.position = m_position;    // when this is done, the material should be changed
        }
    }

    void AniTileDrop(){
        float speed = m_moveAniV.dropSpeed;
        transform.position = Vector3.MoveTowards(transform.position, m_moveAniV.target, speed * Time.deltaTime);
        if(Vector3.Distance(transform.position, m_moveAniV.target) < 0.01f){
            m_moveAniV.doing = false;
            m_moveAniV.trigger = false;
            m_drop = false;
        }
    }

    void AniTileGenerate(){
        m_moveAniV.doing = false;
        m_moveAniV.trigger = false;
        m_drop = false;
        m_newTile = false;
    }
}
