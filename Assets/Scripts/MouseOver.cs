using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Tooltip m_script;

    public string m_name;
    public string m_level;
    public string m_description;

    public void ChangeIcon(Sprite sprite){
        transform.GetComponent<Image>().sprite = sprite;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        m_script.UpdateInfo(m_name, m_level, m_description);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        m_script.CloseInfo();
    }
}
