using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public bool m_selected = false;
    public bool m_hang = false;
    // fluctuation
    float m_bound = 0.2f;
    float m_flucStep = 0.01f;
    bool m_selectActivation = false;

    // Start is called before the first frame update
    void Start()
    {
        m_colorRender = GetComponent<MeshRenderer>();
        m_typeRender = m_typer.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    float timer = 0.0f;
    float gap = 1.0f;
    int i = 0;

    void Update()
    {
        Selected();
    }

    public void SetColor(int index){
        m_colorRender.material = m_colorList[index];
    }

    public void SetType(int index){
        m_typeRender.material = m_typeList[index];
    }

    void Selected(){
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
}
