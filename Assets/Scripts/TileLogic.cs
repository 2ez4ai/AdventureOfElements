using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniMoveVariable{
    // animation variable
    public bool trigger = false;
    public bool doing = false;
    public Vector3 target;

    public float moveSpeed = 7.0f;
    public float dropSpeed = 10.0f;

    public GameObject dropTarget;
}

public class TileLogic : MonoBehaviour
{
    [SerializeField]
    GameObject m_typer;
    [SerializeField]
    List<Material> m_colorList = new List<Material>();
    [SerializeField]
    List<Material> m_typeList = new List<Material>();
    //[SerializeField]
    //public int direction = 1;    // drop direction, 1:down

    MeshRenderer m_colorRender;
    MeshRenderer m_typeRender;
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
    public bool m_remove = false;
    public bool m_removeDone = false;
    float m_removeTimer = 0.0f;
    float m_removeTime = 1.0f;

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
        UAniTileGenerate();
    }

    public void SetColor(int index){
        m_colorRender.material = m_colorList[index];
    }

    public void SetType(int index){
        m_typeRender.material = m_typeList[index];
    }

    public void SetState(bool state){
        m_colorRender.enabled = state;
        m_typeRender.enabled = state;
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
        return m_colorList.IndexOf(m_colorRender.material);
    }

    public int GetTypeIndex(){
        return m_typeList.IndexOf(m_typeRender.material);
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
            m_remove = false;
            m_removeDone = false;
            m_removeTimer = m_removeTime;
        }
        m_removeTime -= Time.deltaTime;
        if(!m_removeDone && m_removeTime < 0.0f){
            m_removeDone = true;
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
