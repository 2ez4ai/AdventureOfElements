using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Tile{
    public GameObject tile;
    public TileLogic script;
    public int color;
    public int type;
    public bool empty;

    public void SetColor(int i=-1){
        if(i != -1){
            color = i;
        }
        script.SetColor(color);
    }

    public void SetType(int i=-1){
        if(i != -1){
            type = i;
        }
        script.SetType(type);
    }
}

public class Board : MonoBehaviour
{
    [SerializeField]
    const int k_row = 8;
    [SerializeField]
    const int k_col = 8;
    int m_numTiles;

    [SerializeField]
    public int m_randomSeed = 64790578;
    [SerializeField]
    public int m_numColor;
    [SerializeField]
    public int m_numType;
    GameObject[] m_tilesInit = new GameObject[k_row * k_col];
    List<Tile> m_tiles = new List<Tile>();
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
        m_tilesInit = GameObject.FindGameObjectsWithTag("Tiles");
        m_numTiles = k_row * k_col;
        for(int i = 0; i < m_numTiles; i++){
            Tile temp = new Tile();
            temp.tile = m_tilesInit[i];
            temp.script = m_tilesInit[i].GetComponent<TileLogic>();
            temp.color = Random.Range(0, m_numColor);
            temp.type = Random.Range(0, m_numType);
            temp.empty = false;
            m_tiles.Add(temp);
        }

    }

    void InitCheck(){
        bool unfinished = true;
        while(unfinished){
            unfinished = false;
            for(int i = 0; i < m_numTiles; i++){
                List<int> temp = MatchesAt(i);
                if(temp.Count > 1){
                    unfinished = true;
                    m_tiles[i].color = Random.Range(0, m_numColor);
                    m_tiles[i].type = Random.Range(0, m_numType);
                }
            }
        }
    }

    void InitDraw(){
        foreach(Tile t in m_tiles){
            t.SetColor();
            t.SetType();
        }
    }

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
        if(m_tiles[indexA].color == m_tiles[indexB].color){
            color = true;
        }
        if(m_tiles[indexA].type == m_tiles[indexB].type){
            type = true;
        }
        if(color && type){    // changing this allows other way to match
            return true;
        }
        return false;
    }

    List<int> CheckMap(){
        List<int> res = new List<int>();
        for(int i = 0; i < m_numTiles; i++){
            List<int> temp = MatchesAt(i);
            if(temp.Count > 1){
                res.Add(i);
            }
        }
        return res;
    }

    List<int> MatchesAt(int index, bool ur=false){
        // if there is no match involves tiles[index], return a list of size 1
        // ur means up and right search
        List<int> matchedTiles = new List<int>();
        matchedTiles.Add(index);

        // row-wise
        List<int> temp = CheckRowMatch(index, ur);
        foreach (int t in temp){
            matchedTiles.Add(t);
        }

        // col-wise
        //temp = new List<int>();
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
            return tempSeq;
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
            return tempSeq;
        }
        // to left
        for(int r = row - 1; r >= 0; r--){
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
        // if two tiles are selected at one time
        List<int> selected = new List<int>();
        for(int i = 0; i < k_col * k_row; i++){
            if(m_tiles[i].script.m_selected){
                selected.Add(i);
            }
        }
        foreach(int i in selected){
            m_tiles[i].script.m_selected = false;
        }

        (int i, int j) A = IndexToRC(selected[0]);
        (int i, int j) B = IndexToRC(selected[1]);
        // we may have other direction list
        List<(int i, int j)> direction = new List<(int i, int j)>{(1, 0), (-1, 0), (0, -1), (0, 1)};
        foreach((int i, int j) d in direction){
            if((A.i + d.i == B.i) && (A.j + d.j == B.j)){
                IsValidMatch(selected[0], selected[1]);
                return true;
            }
        }

        return false;
    }

    void IsValidMatch(int a, int b){
        // remove tiles if valid
        SwapTiles(a, b);
        List<int> AMatch = MatchesAt(a);
        List<int> BMatch = MatchesAt(b);
        bool IsMatch = false;
        if(AMatch.Count > 2){
            IsMatch = true;
            foreach(int t in AMatch){
                m_tiles[t].tile.SetActive(false);
                m_tiles[t].empty = true;
            }
        }
        if(BMatch.Count > 2){
            IsMatch = true;
            foreach(int t in BMatch){
                m_tiles[t].tile.SetActive(false);
                m_tiles[t].empty = true;
            }
        }

        if(!IsMatch){
            SwapTiles(a, b);
        }
        else{
            DropTiles();
            RemoveAfterDrop();
        }
    }

    void SwapTiles(int a, int b){
        // I should use Struct....
        int tempColor = m_tiles[a].color;
        int tempType = m_tiles[a].type;

        m_tiles[a].SetType(m_tiles[b].type);
        m_tiles[a].SetColor(m_tiles[b].color);
        m_tiles[b].SetType(tempType);
        m_tiles[b].SetColor(tempColor);
    }

    void DropTiles(int d=0){
        // drop tiles from direction d; d = 0123/UDLR
        bool changed = true;
        while(changed){
            changed = false;
            for(int i = 0; i < m_numTiles; i++){
                if(m_tiles[i].empty){
                    changed = true;
                    m_tiles[i].tile.SetActive(true);
                    int index = FetchTile(i);
                    if(index == -1){
                        m_tiles[i].SetColor(Random.Range(0, m_numColor));
                        m_tiles[i].SetType(Random.Range(0, m_numType));
                    }
                    else{
                        m_tiles[i].SetColor(m_tiles[index].color);
                        m_tiles[i].SetType(m_tiles[index].type);
                    }
                    m_tiles[i].empty = false;
                }
            }
        }
    }

    int FetchTile(int index, int d=0){
        // get a tile from direction d for position at index
        // return -1 if there is no tiles available
        List<(int i, int j)> direction = new List<(int i, int j)>{(1, 0), (-1, 0), (0, -1), (0, 1)};
        List<int> border = new List<int>{k_row, 0, 0, k_col};

        (int row, int col) = IndexToRC(index);
        int res = -1;
        row += direction[d].i;
        col += direction[d].j;
        while(row>-1 && row<k_row && col>-1 && col<k_col){
            int newIndex = RCToIndex(row, col);
            if(!m_tiles[newIndex].empty){
                m_tiles[newIndex].empty = true;
                return newIndex;
            }
            row += direction[d].i;
            col += direction[d].j;
        }
        return res;
    }

    void RemoveAfterDrop(){
        bool unfinished = true;
        while(unfinished){
            unfinished = false;
            for(int i = 0; i < m_numTiles; i++){
                List<int> temp = MatchesAt(i);
                if(temp.Count > 1){
                    unfinished = true;
                    IsValidMatch(temp[0], temp[1]);
                }
            }
        }
    }
}
