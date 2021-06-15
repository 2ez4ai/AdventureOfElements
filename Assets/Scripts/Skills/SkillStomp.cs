using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStomp : MonoBehaviour
{
    [SerializeField] List<GameObject> m_drawers;
    List<Vector3> m_position = new List<Vector3>();
    [SerializeField] List<Material> m_mats;
    [SerializeField] float m_speed;

    [SerializeField] int m_debugR;
    [SerializeField] int m_debugC;
    [SerializeField] float m_debugS;

    void Start(){
        foreach(GameObject t in m_drawers){
            m_position.Add(t.transform.position);
            t.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < m_drawers.Count; i++){
            Transform t = m_drawers[i].transform;
            Vector3 target = m_position[(i + 1) % m_drawers.Count];
            t.position = Vector3.MoveTowards(t.position, target, m_debugS * m_speed * Time.deltaTime);
        }
        if(Vector3.Distance(m_drawers[0].transform.position, m_position[1]) < 0.01f){
            UpdatePostion();
        }
        // timer += Time.deltaTime;
        // if(timer > 2.0){
        //     m_im = (m_im + 1) % 5;
        //     SetMats(m_im);
        //     timer = 0.0f;
        // }

    }

    void UpdatePostion(){
        for(int i = 0; i < m_position.Count; i++){
            m_position[i] = m_drawers[i].transform.position;
        }
    }

    void SetMats(int m){
        foreach(GameObject t in m_drawers){
            t.SetActive(true);
            TrailRenderer tr = t.GetComponent<TrailRenderer>();
            tr.enabled = true;
            tr.material = m_mats[m];
        }
    }

    public void DisableTrailer(bool state=false){
        foreach(GameObject t in m_drawers){
            t.SetActive(state);
            TrailRenderer tr = t.GetComponent<TrailRenderer>();
            tr.enabled = state;
        }
    }

    public void Activate(int r, int c, int size, int type){
        if(size == 0){
            return;
        }
        DisableTrailer();
        List<(int , int )> direction = new List<(int, int)>{(0, 0), (0, size), (-1 * size, 0), (0, -1 * size)};
        for(int i = 0; i < m_drawers.Count; i ++){
            (int nr, int nc) = direction[i];
            r += nr;
            c += nc;
            m_drawers[i].transform.position = new Vector3(0.5f, (r - 3) * 2.0f, (c - 3) * 2.0f);
            UpdatePostion();
        }
        SetMats(type);
    }
}
