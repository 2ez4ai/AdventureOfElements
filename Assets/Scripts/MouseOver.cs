using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Tooltip m_tooltip;

    public string m_name;
    public string m_level;
    public string m_description;

    [SerializeField]
    int m_showTiles = -1;
    PlayerLogic m_player;
    CreatureLogic m_creature;
    GameObject[] m_tiles;
    List<TileLogic> m_tileScripts = new List<TileLogic>();

    void Start(){
        if(m_showTiles != -1){
            m_player = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerLogic>();
            m_creature = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CreatureLogic>();
            m_tiles = GameObject.FindGameObjectsWithTag("Tiles");
            foreach(GameObject t in m_tiles){
                m_tileScripts.Add(t.GetComponent<TileLogic>());
            }
        }
    }

    public void ChangeIcon(Sprite sprite){
        transform.GetComponent<Image>().sprite = sprite;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        m_tooltip.UpdateInfo(m_name, m_level, m_description);
        if(m_showTiles != -1){
            ShowTiles(true);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        m_tooltip.CloseInfo();
        if(m_showTiles != -1){
            ShowTiles(false);
        }
    }

    void ShowTiles(bool state){
        int type = 0;
        if(m_showTiles == 0){
            type = m_player.m_injureType;
        }
        if(m_showTiles == 1){
            type = m_creature.m_injureType;
        }
        foreach(TileLogic t in m_tileScripts){
            if(t.GetTypeIndex() == type){
                t.SetEffect(state);
            }
        }
    }
}
