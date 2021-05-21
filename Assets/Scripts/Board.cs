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

    public int dropDest = -1;
    int dropColor = -1;
    int dropType = -1;

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

    public void SetState(bool state){
        script.SetState(state);
    }

    public void PreDrop(int c, int t){
        // record the color and type that will be rendered after a drop
        dropColor = c;
        dropType = t;
    }

    public void PostDrop(){
        if(dropColor == -1){    // not changed after a drop
            return;
        }
        SetColor(dropColor);
        SetType(dropType);
    }
}

public class Board : MonoBehaviour
{
    [SerializeField]
    const int k_row = 8;
    [SerializeField]
    const int k_col = 8;
    [SerializeField]
    public int m_dirIndex = 1;
    List<(int r, int c)> m_direction = new List<(int r, int c)>{(1, 0), (-1, 0), (0, -1), (0, 1)};    // move to

    int m_numTiles;
    [SerializeField]
    public int m_numColor;
    [SerializeField]
    public int m_numType;
    GameObject[] m_tilesInit = new GameObject[k_row * k_col];
    List<Tile> m_tiles = new List<Tile>();

    [SerializeField]
    public int m_randomSeed = 0;

    int m_aTile = -1;    // selected tiles
    int m_bTile = -1;

    // animation related variables
    [SerializeField]
    Collider m_rayBlocker;
    // for swap
    bool m_isSwapping = false;
    bool m_isReversing = false;    // to reverse a swap if there is no match
    // for drop
    bool m_isDropping = false;
    bool m_dropStepDown = true;    // we drop tiles step by step

    // Start is called before the first frame update
    void Start()
    {
        Initialization();
    }

    // Update is called once per frame
    void Update()
    {
        UAniTileSwap();
        UAniTileDrop();
        // if(Input.GetButtonDown("Fire1")){
        //     Debug.Log("type of 61: " + m_tiles[61].type);
        //     Debug.Log("type of 53: " + m_tiles[53].type);
        //     Debug.Log("type of 45: " + m_tiles[45].type);
        // }
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
        // called when two tiles are selected at one time
        // returns true if it is a valid swap (the two selected tiles are contiguous or something like that)
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
        // we may have other directions
        List<(int i, int j)> direction = new List<(int i, int j)>{(1, 0), (-1, 0), (0, -1), (0, 1)};
        foreach((int i, int j) d in direction){
            if((A.i + d.i == B.i) && (A.j + d.j == B.j)){
                m_aTile = selected[0];
                m_bTile = selected[1];
                // HasMatch(selected[0], selected[1]);
                return true;
            }
        }
        return false;
    }

    bool RemoveMatch(int a, int b){
        // called when the board is changed like a swap is conducted
        // remove tiles if valid

        List<int> AMatch = MatchesAt(a);
        List<int> BMatch = MatchesAt(b);
        bool IsMatch = false;

        if(AMatch.Count > 2){
            IsMatch = true;
            foreach(int t in AMatch){
                m_tiles[t].SetState(false);
                m_tiles[t].empty = true;
            }
        }
        if(BMatch.Count > 2){
            IsMatch = true;
            foreach(int t in BMatch){
                m_tiles[t].SetState(false);
                m_tiles[t].empty = true;
            }
        }

        if(!IsMatch){
            return false;
        }
        else{
            SetDropDest();
            AniTileDrop();
        }
        return true;
    }

    void SwapTiles(){
        // swap materials of two selected tiles
        int tempColor = m_tiles[m_aTile].color;
        int tempType = m_tiles[m_aTile].type;

        m_tiles[m_aTile].SetType(m_tiles[m_bTile].type);
        m_tiles[m_aTile].SetColor(m_tiles[m_bTile].color);
        m_tiles[m_bTile].SetType(tempType);
        m_tiles[m_bTile].SetColor(tempColor);
    }

    void DropTiles(){
        // change materials after a drop
        foreach(Tile t in m_tiles){
            if(t.empty){
                t.SetState(true);
                t.empty = false;
            }
            t.PostDrop();
            t.PreDrop(-1, -1);
        }
        //RemoveAfterDrop();
    }

    int GetDropBorder(int index){
        // return where the new tile generated
        (int r, int c) = IndexToRC(index);
        int iteration = k_row > k_col ? k_row : k_col;
        for(int i = 0; i < iteration; i++){
            r -= m_direction[m_dirIndex].r;
            c -= m_direction[m_dirIndex].c;
            if(r < 0 || r >= k_row || c < 0 || c >= k_col){
                r += m_direction[m_dirIndex].r;
                c += m_direction[m_dirIndex].c;
                return RCToIndex(r, c);
            }
        }
        return -1;
    }

    void SetDropDest(){
        for(int i = 0; i < m_numTiles; i++){
            if(m_tiles[i].empty){
                (int destR, int destC) = IndexToRC(i);
                int r = destR - m_direction[m_dirIndex].r;
                int c = destC -  m_direction[m_dirIndex].c;
                while(r > -1 && r < k_row && c > -1 && c < k_col){
                    int temp = RCToIndex(r, c);    // tiles that need to drop
                    if(!m_tiles[temp].empty && m_tiles[temp].dropDest == -1){    // hasn't been set a destination
                        m_tiles[temp].dropDest = RCToIndex(destR, destC);
                        destR -= m_direction[m_dirIndex].r;
                        destC -= m_direction[m_dirIndex].c;
                    }
                    r -= m_direction[m_dirIndex].r;
                    c -= m_direction[m_dirIndex].c;
                }
                int borderTile = GetDropBorder(i);
                m_tiles[borderTile].PreDrop(Random.Range(0, m_numColor), Random.Range(0, m_numType));
            }
        }
        m_isDropping = true;
        m_rayBlocker.enabled = true;
    }

    void RemoveAfterDrop(){
        bool unfinished = true;
        while(unfinished){
            unfinished = false;
            for(int i = 0; i < m_numTiles; i++){
                List<int> temp = MatchesAt(i);
                if(temp.Count > 1){
                    unfinished = true;
                    RemoveMatch(temp[0], temp[1]);
                }
            }
        }
    }

    void CheckValiableMatch(){
        // 2n * O(MatchesAtIndex)
        for(int i = 0; i < m_numTiles; i++){
            (int r, int c) temp = IndexToRC(i);
            if(temp.r + 1 < k_row){    // upside check
                if(RemoveMatch(i, RCToIndex(temp.r+1, temp.c))){
                    return;
                }
            }
            if(temp.c + 1 < k_col){    // right side
                if(RemoveMatch(i, RCToIndex(temp.r, temp.c+1))){
                    return;
                }
            }
        }
        Shuffle();
    }

    void Shuffle(){
        Debug.Log("We need a shuffle!");
    }

    // animation things
    public void AniTileSwap(){
        // animation for swapping tile a and tile b
        // turn on the triggers
        m_isSwapping = true;
        m_rayBlocker.enabled = true;
        // two tiles start moving
        Vector3 aPos = m_tiles[m_aTile].tile.transform.position;
        Vector3 bPos = m_tiles[m_bTile].tile.transform.position;
        aPos.x = 0.0f;
        bPos.x = 0.0f;
        m_tiles[m_aTile].script.m_moveAniV.trigger = true;
        m_tiles[m_aTile].script.m_moveAniV.target = bPos;
        m_tiles[m_bTile].script.m_moveAniV.trigger = true;
        m_tiles[m_bTile].script.m_moveAniV.target = aPos;
    }

    void AniTileDrop(){
        // turn on triggers
        for(int i = 0; i < m_numTiles; i++){
            if(m_tiles[i].dropDest != -1){
                m_tiles[m_tiles[i].dropDest].PreDrop(m_tiles[i].color, m_tiles[i].type);
                // trigger
                Vector3 tPos = m_tiles[m_tiles[i].dropDest].tile.transform.position;
                m_tiles[i].script.m_moveAniV.trigger = true;
                m_tiles[i].script.m_moveAniV.target = tPos;
                m_tiles[i].script.m_drop = true;
                m_tiles[i].dropDest = -1;
            }
        }
    }

    void UAniTileSwap(){
        if(m_isSwapping && !m_tiles[m_aTile].script.m_moveAniV.trigger){
            // the swap is finished visually, but we still need to change the appearance...
            m_isSwapping = false;
            m_rayBlocker.enabled = false;
            SwapTiles();    // when the animation is done, change logically
            m_isReversing = !m_isReversing;    // check whether the swap needs to be reversed
        }

        if(m_isReversing){
            if(!RemoveMatch(m_aTile, m_bTile)){
                AniTileSwap();
            }
            else{
                m_isReversing = false;
            }
        }
    }

    void UAniTileDrop(){
        // animation for dropping a to d
        if(m_isDropping){
            // check whether a step is down and the drop is done
            int noCnt = 0;
            bool noDrop = true;
            foreach(Tile t in m_tiles){
                noDrop = noDrop & !t.script.m_drop;    // drop animation done
            }
            if(noDrop){    // the dropping animation is done
                DropTiles();    // change the material
                if(noCnt == 0){
                    m_isDropping = false;
                    m_rayBlocker.enabled = false;
                }
                else{
                    AniTileDrop();    // trigger again
                }
            }
        }
    }
}
