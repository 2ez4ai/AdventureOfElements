using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creature : MonoBehaviour
{
    [SerializeField] int m_HP = 100;
    [SerializeField] int m_injureType = 0;
    [SerializeField] int m_injureColor = -1;
    //[SerializeField]
    //Player m_player;

    [SerializeField] Text m_UIHP;

    int damage = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(){
        m_HP -= damage;
        m_UIHP.text = "HP : " + m_HP;
        if(m_HP < 1){
            Debug.Log("You win!");
        }
        damage = 0;
    }

    public bool ValidDamage(int c, int t){
        if(c == m_injureColor || m_injureColor == -1){
            if(t == m_injureType || m_injureType == -1){
                damage += 1;
                return true;
        }
        }
        return false;
    }
}
