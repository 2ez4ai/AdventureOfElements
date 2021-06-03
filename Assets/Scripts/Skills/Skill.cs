using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills")]
public class Skill : ScriptableObject
{
    public Sprite m_sprite;    // fixed
    [Range(0, 99)]
    public int m_ID;
    public int m_skillID;    // each skill has a unique skillID and each level has a unique ID...

    public string m_name;    // fixed
    public int m_lv;    // 0 if it is disposable
    public string m_effect;
    public string m_description;    // fixed

    public int m_keyValue;

    public int m_prerequisite;
    public bool m_disposable;    // fixed
    public bool m_linear;    // the effect is m_keyValue * m_level
    public int m_belonging;    // 0: both player and creatures; 1: player only; 2: creatures only
}
