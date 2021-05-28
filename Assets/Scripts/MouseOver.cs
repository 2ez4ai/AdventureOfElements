using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Tooltip m_script;
    [SerializeField] List<Sprite> m_icons = new List<Sprite>();

    public string m_name;
    public string m_level;
    public string m_description;

    public void ChangeIcon(int index){
        if(index > m_icons.Count){
            Debug.Log("Error!");
            return;
        }
        transform.GetComponent<Image>().sprite = m_icons[index];
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Output to console the GameObject's name and the following message
        Debug.Log("Cursor Entering " + name + " GameObject");
        m_script.UpdateInfo(m_name, m_level, m_description);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Output the following message with the GameObject's name
        Debug.Log("Cursor Exiting " + name + " GameObject");
        //The mouse is no longer hovering over the GameObject so output this message each frame
        m_script.CloseInfo();
    }
}
