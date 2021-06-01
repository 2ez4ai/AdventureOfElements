using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creature", menuName = "Creatures")]
public class Creature : ScriptableObject
{
    public Sprite m_avatar;    // fixed
    public string m_name;    // fixed
    public int m_lv;    // fixed
    public string m_description;    // fixed

    public int m_maxHP;    // fixed
    public int m_attackType;    // fixed
    public Sprite m_attackTypeIcon;    // fixed
    public int m_attackMultiplier;    // update
    public int m_injureType;    // fixed (0: metal ...
    public Sprite m_injureTypeIcon;    // fixed
    public int m_attackFreq;    // update

    public List<int> m_skills = new List<int>();    // fixed skill list
}
