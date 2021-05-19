using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    const int k_row = 8;
    [SerializeField]
    const int k_col = 8;
    [SerializeField]
    public int m_randomSeed = 64790578;
    [SerializeField]
    public int m_numColor;
    [SerializeField]
    public int m_numType;
    GameObject[] m_tiles = new GameObject[k_row * k_col];
    List<Tile> m_tilesScripts = new List<Tile>();
    List<int> m_tilesColor = new List<int>();
    List<int> m_tilesType = new List<int>();
    bool initCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        Initialization();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Initialization(){
        InitSeed();
        InitTiles();
        InitCheck();    // make sure there is no matching at the begining
        InitDraw();
    }

    void InitSeed(){
        // init random seed
        if(m_randomSeed == 0){
            // generate a new randomseed
            m_randomSeed = Random.Range(10000000, 99999999);
        }
        Random.InitState(m_randomSeed);
    }

    void InitTiles(){
        m_tiles = GameObject.FindGameObjectsWithTag("Tiles");

        for(int i = 0; i < k_row * k_col; i++){
            m_tilesScripts.Add(m_tiles[i].GetComponent<Tile>());
            m_tilesColor.Add(Random.Range(0, m_numColor));
            m_tilesType.Add(Random.Range(0, m_numType));
        }

    }

    void InitCheck(){
        bool unfinished = true;
        while(unfinished){
            unfinished = false;
            for(int i = 0; i < k_row * k_col; i++){
                List<int> temp = MatchesAt(i);
                if(temp.Count > 1){
                    unfinished = true;
                    Debug.Log("fix at "+i);
                    m_tilesColor[i] = Random.Range(0, m_numColor);
                    m_tilesType[i] = Random.Range(0, m_numType);
                }
            }
        }
    }

    void InitDraw(){
        for(int i = 0; i < k_row * k_col; i++){
            m_tilesScripts[i].SetColor(m_tilesColor[i]);
            m_tilesScripts[i].SetType(m_tilesType[i]);
        }
    }
    /*
    void SetRandomColor(int index){
        m_tilesColor[index] = Random.Range(0, m_numColor);
        m_tilesScripts[index].SetColor(m_tilesColor[index]);
    }

    void SetRandomType(int index){
        m_tilesType[index] = Random.Range(0, m_numType);
        m_tilesScripts[index].SetType(m_tilesType[index]);
    }
    */

    (int row, int col) IndexToRC(int index){
        int row = index / k_col;
        int col = index % k_col;
        return (row, col);
    }

    int RCToIndex(int row, int col){
        return row * k_col + col;
    }

    bool TwoTileCmp(int indexA, int indexB){
        // whether two tiles are matchable
        bool type = false;
        bool color = false;
        if(m_tilesColor[indexA] == m_tilesColor[indexB]){
            color = true;
        }
        if(m_tilesType[indexA] == m_tilesType[indexB]){
            type = true;
        }
        if(color && type){    // changing this allows other way to match
            return true;
        }
        return false;
    }

    List<int> CheckMap(){
        List<int> res = new List<int>();
        for(int i = 0; i < k_col * k_row; i++){
            List<int> temp = MatchesAt(i);
            if(temp.Count > 1){
                res.Add(i);
            }
        }
        return res;
    }

    List<int> MatchesAt(int index, bool ur=false){
        // return if there is a match about tiles[index], ur means up and right
        List<int> matchedTiles = new List<int>();
        matchedTiles.Add(index);

        // row-wise
        List<int> temp = CheckRowMatch(index, ur);
        foreach (int t in temp){
            matchedTiles.Add(t);
        }

        // col-wise
        temp = new List<int>();
        temp = CheckColMatch(index, ur);
        foreach (int t in temp){
            matchedTiles.Add(t);
        }

        return matchedTiles;
    }

    List<int> CheckRowMatch(int index, bool ur=false){
        (int row, int col) = IndexToRC(index);
        int cnt = 0;
        List<int> tempSeq = new List<int>();
        // to right
        for(int c = col + 1; c < k_col; c++){
            int newIndex = RCToIndex(row, c);
            if(TwoTileCmp(index, newIndex)){
                cnt += 1;
                tempSeq.Add(newIndex);
            }
            else{
                break;
            }
        }
        if(ur){    // no need to the left
            if(cnt < 2){
                return new List<int>();
            }
        }
        // to left
        for(int c = col - 1; c >= 0; c--){
            int newIndex = RCToIndex(row, c);
            if(TwoTileCmp(index, newIndex)){
                cnt += 1;
                tempSeq.Add(newIndex);
            }
            else{
                break;
            }
        }

        if(cnt < 2){
            return new List<int>();
        }
        return tempSeq;
    }

    List<int> CheckColMatch(int index, bool ur=false){
        (int row, int col) = IndexToRC(index);
        int cnt = 0;
        List<int> tempSeq = new List<int>();
        // to right
        for(int r = row + 1; r < k_row; r++){
            int newIndex = RCToIndex(r, col);
            if(TwoTileCmp(index, newIndex)){
                cnt += 1;
                tempSeq.Add(newIndex);
            }
            else{
                break;
            }
        }
        if(ur){    // no need to the left
            if(cnt < 2){
                return new List<int>();
            }
        }
        // to left
        for(int r = col - 1; r >= 0; r--){
            int newIndex = RCToIndex(r, col);
            if(TwoTileCmp(index, newIndex)){
                cnt += 1;
                tempSeq.Add(newIndex);
            }
            else{
                break;
            }
        }

        if(cnt < 2){
            return new List<int>();
        }
        return tempSeq;
    }

    public bool IsValidSwap(){
        List<int> selected = new List<int>();
        for(int i = 0; i < k_col * k_row; i++){
            if(m_tilesScripts[i].m_selected){
                selected.Add(i);
            }
        }
        foreach(int i in selected){
            m_tilesScripts[i].m_selected = false;
        }

        (int i, int j) A = IndexToRC(selected[0]);
        (int i, int j) B = IndexToRC(selected[1]);
        // we may have other direction list
        List<(int i, int j)> direction = new List<(int i, int j)>{(-1, 0), (1, 0), (0, -1), (0, 1)};
        foreach((int i, int j) d in direction){
            if((A.i + d.i == B.i) && (A.j + d.j == B.j)){
                IsValidMatch(selected[0], selected[1]);
                return true;
            }
        }

        return false;
    }

    void IsValidMatch(int a, int b){
        SwapTiles(a, b);
        List<int> AMatch = MatchesAt(a);
        List<int> BMatch = MatchesAt(b);
        bool IsMatch = false;
        if(AMatch.Count > 2){
            IsMatch = true;
            foreach(int t in AMatch){
                m_tiles[t].SetActive(false);
            }
        }
        if(BMatch.Count > 2){
            IsMatch = true;
            foreach(int t in BMatch){
                m_tiles[t].SetActive(false);
            }
        }

        if(!IsMatch){
            SwapTiles(a, b);
        }
    }

    void SwapTiles(int a, int b){
        // I should use Struct....
        int tempColor = m_tilesColor[a];
        int tempType = m_tilesType[a];

        m_tilesColor[a] = m_tilesColor[b];
        m_tilesColor[b] = tempColor;
        m_tilesType[a] = m_tilesType[b];
        m_tilesType[b] = tempType;

        m_tilesScripts[a].SetType(m_tilesType[a]);
        m_tilesScripts[a].SetColor(m_tilesColor[a]);
        m_tilesScripts[b].SetType(m_tilesType[b]);
        m_tilesScripts[b].SetColor(m_tilesColor[b]);
    }
}
