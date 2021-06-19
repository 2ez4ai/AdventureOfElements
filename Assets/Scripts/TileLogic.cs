using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniMoveVariable{
    // animation variable
    public bool trigger = false;
    public bool doing = false;
    public Vector3 target;

    public float moveSpeed = 8.0f;
    public float dropSpeed = 16.0f;    // 16

    public bool targetSpecial = false;

    public GameObject dropTarget;
}

public class TileLogic : MonoBehaviour
{
    [SerializeField] GameObject m_gameObjWhite;
    [SerializeField] GameObject m_gameObjBlue;
    [SerializeField] GameObject m_gameObjRed;
    [SerializeField] GameObject m_typer;
    [SerializeField] GameObject m_particle;
    [SerializeField] List<Material> m_colorMat;
    [SerializeField] List<Material> m_typeMat;
    [SerializeField] List<Material> m_effectMat;    // effect
    [SerializeField] bool m_onBoard;

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

    // swap animation
    public AniMoveVariable m_moveAniV = new AniMoveVariable();    // the swap and drop would not be done simultaneously

    // drop animation
    public bool m_drop = false;
    public bool m_newTile = false;    // generate new Tile here

    // remove animation
    bool m_remove = false;
    public bool m_removeDone = true;
    float m_removeTimer = 0.0f;
    float m_removeTime = 0.4f;

    // special tile
    public bool m_isSpecial = false;

    // stomp damage
    public int m_stompDamage = 0;

    // attack
    Vector3 m_creaturePos = new Vector3(10.15f, 2.7f, 5.75f);
    Vector3 m_playerPos = new Vector3(10.15f, -2.7f, -5.75f);
    Vector3 m_targetPosition;

    // Start is called before the first frame update
    void Awake()
    {
        m_colorRender = GetComponent<MeshRenderer>();
        m_typeRender = m_typer.GetComponent<MeshRenderer>();
        m_position = transform.position;
    }

    void Update()
    {
        USelected();
        UAniMove();
        USwing();
        USpecial();
        UAniRemove();
        //UAniTileGenerate();
    }

    public void SetColor(int index){
        m_colorIndex = index;
        // m_colorRender = GetComponent<MeshRenderer>();
        m_colorRender.material = m_colorMat[m_colorIndex];
    }

    public void SetType(int index){
        m_typeIndex = index;
        // m_typeRender = m_typer.GetComponent<MeshRenderer>();
        m_typeRender.material = m_typeMat[m_typeIndex];
    }

    public void SetEffect(bool state){
        if(state){
            m_colorRender.material = m_effectMat[m_typeIndex];
        }
        else{
            m_colorRender.material = m_colorMat[m_colorIndex];
        }
    }

    public void SetRemoveState(bool state, bool creatureIsAttacking=false){
        // if true, means it is removed
        // actually, no parameter is needed
        if(state){
            m_remove = true;
            m_removeDone = false;
            m_targetPosition = m_creaturePos;
            if(creatureIsAttacking){
                m_targetPosition = m_playerPos;
            }
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
            SetEffect(true);
            m_selectActivation = true;
        }
        if(!m_selected && m_selectActivation){
            transform.position = new Vector3(0.0f, transform.position.y, transform.position.z);
            SetEffect(false);
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

    public void SetSpecial(bool state){
        if(state){
            m_isSpecial = true;
        }
        else{
            m_isSpecial = false;
            // transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void SetStompDamage(int d){
        m_stompDamage = d;
    }

    void USpecial(){
        if(m_isSpecial){
            m_colorRender.enabled = true;
            m_typeRender.enabled = true;
            m_colorRender.material = m_colorMat[3];
            //transform.Rotate(new Vector3(m_specialRotSpeed, 0, 0));
        }
    }

    void UAniRemove(){
        if(m_remove){
            // this tile is going to be removed
            m_colorRender.material = m_effectMat[m_typeIndex];
            m_removeTimer = m_removeTime;
            m_remove = false;
        }

        // change size
        m_removeTimer -= Time.deltaTime;
        if(m_removeTimer > 0.0f && m_removeTimer < 0.5f * m_removeTime){
            float size = m_removeTimer / (0.5f * m_removeTime);
            transform.localScale = Vector3.one * size;
        }
        // flying particle; not desired so far... may can be used as an attack
        if(m_removeTimer > 0.0f && m_removeTimer < 0.6f * m_removeTime){
            m_particle.SetActive(true);
            // m_particle.GetComponent<TrailRenderer>().material.SetColor("_EmissionColor", m_effectMat[m_typeIndex].GetColor("_EmissionColor") * 2.0f);
            m_particle.GetComponent<TrailRenderer>().material = m_effectMat[m_typeIndex];
            m_particle.transform.position = Vector3.Lerp(m_position, m_targetPosition, 1.0f - m_removeTimer / (0.6f * m_removeTime));
        }

        // reset
        if(!m_removeDone && m_removeTimer < 0){
            transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            m_particle.SetActive(false);
            m_colorRender.material = m_colorMat[m_colorIndex];
            m_colorRender.enabled = false;
            m_typeRender.enabled = false;
            m_removeDone = true;    // start drop
        }
    }

    void AniTileSwap(){
        // move tile a to the target
        float speed = m_moveAniV.moveSpeed;
        transform.position = Vector3.MoveTowards(transform.position, m_moveAniV.target, speed * Time.deltaTime);
        if(Vector3.Distance(transform.position, m_moveAniV.target) < 0.01f){
            if(m_moveAniV.targetSpecial){
                m_colorRender.material = m_colorMat[3];
                m_isSpecial = true;
            }
            else{
                m_isSpecial = false;
            }
            transform.position = m_position;    // when this is done, the material should be changed
            m_moveAniV.doing = false;
            m_moveAniV.trigger = false;
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
