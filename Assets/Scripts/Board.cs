using System.Linq;
using System.Globalization;
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
    public int emptyTilesCnt = 0;    // drop from where
    public bool drop = false;
    public bool dropUsed = false;
    // public bool borderDrop = false;    // the tiles is removed at the border
    public Vector3 dropStartV;
    public Vector3 dropDestV;

    public void SetColor(int c = -1){
        if(c != -1){
            color = c;
        }
        script.SetColor(color);
    }

    public void SetType(int t = -1){
        if(t != -1){
            type = t;
        }
        script.SetType(type);
    }

    public void SetState(bool state){
        script.SetState(state);
    }

    public void UpdateLogic(){
        color = script.GetColorIndex();
        type = script.GetTypeIndex();
    }
}

public class Board : MonoBehaviour
{
    [SerializeField]
    public int m_randomSeed = 0;

    // board settings
    [SerializeField]
    const int k_row = 8;
    [SerializeField]
    const int k_col = 8;
    [SerializeField]
    public int m_dirIndex = 1;
    List<(int r, int c)> m_direction = new List<(int r, int c)>{(1, 0), (-1, 0), (0, -1), (0, 1)};    // to where
    float m_rDist;    // the distance between two row
    float m_cDist;

    // tiles settings
    int m_numTiles;
    [SerializeField]
    public int m_numColor;
    [SerializeField]
    public int m_numType;
    GameObject[] m_tilesInit = new GameObject[k_row * k_col];
    List<Tile> m_tiles = new List<Tile>();
    int m_aTile = -1;    // selected tiles
    int m_bTile = -1;

    // others
    //[SerializeField]
    //Player m_player;
    [SerializeField]
    Creature m_creatureScript;

    // animation related variables
    [SerializeField]
    Collider m_rayBlocker;
    [SerializeField]
    MeshRenderer m_rayBlockerR;
    // for swap
    bool m_isSwapping = false;
    bool m_isReversing = false;    // to reverse a swap if there is no match
    // for drop
    bool m_isDropping = false;

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
        UCheckMap();
    }

    // ------------------------------------------------------------------------
    // helper function things
    // ------------------------------------------------------------------------

    (int row, int col) IndexToRC(int index){
        int row = index / k_col;
        int col = index % k_col;
        return (row, col);
    }

    int RCToIndex(int row, int col){
        return row * k_col + col;
    }

    bool TilesCmp(int indexA, int indexB){
        // whether two tiles are the same
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

    void TilesSwap(int i = -1, int j = -1, bool logically = false){
        // swap materials of two selected tiles
        if(i == -1){
            i = m_aTile;
            j = m_bTile;
        }
        int tempColor = m_tiles[i].color;
        int tempType = m_tiles[i].type;

        if(logically){
            m_tiles[i].type = m_tiles[j].type;
            m_tiles[i].color = m_tiles[j].color;
            m_tiles[j].type = tempType;
            m_tiles[j].color = tempColor;
            return;
        }
        m_tiles[i].SetType(m_tiles[j].type);
        m_tiles[i].SetColor(m_tiles[j].color);
        m_tiles[j].SetType(tempType);
        m_tiles[j].SetColor(tempColor);
    }

    void SetTileEmpty(int index, bool state){
        if(m_tiles[index].empty && state){
            return;    // already be removed
        }
        m_tiles[index].SetState(state);
        m_tiles[index].empty = state;
        if(state){    // there is a remove
            m_creatureScript.UpdateDamage(m_tiles[index].color, m_tiles[index].type);
        }
    }

    // ------------------------------------------------------------------------
    // initialization
    // ------------------------------------------------------------------------

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
        m_rDist = m_tiles[RCToIndex(1, 0)].tile.transform.position.y - m_tiles[RCToIndex(0, 0)].tile.transform.position.y;
        m_cDist = m_tiles[RCToIndex(0, 1)].tile.transform.position.z - m_tiles[RCToIndex(0, 0)].tile.transform.position.z;
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
        Shuffle();
    }

    void InitSwing(){
        // initialization
        foreach(Tile t in m_tiles){
            t.script.SetSwing(false);
        }
    }

    void InitDraw(){
        foreach(Tile t in m_tiles){
            t.SetColor();
            t.SetType();
        }
    }

    // ------------------------------------------------------------------------
    // remove
    // ------------------------------------------------------------------------

    public bool IsValidSwap(){
        // called when two tiles are selected at one time
        // returns true if it is a valid swap (the two selected tiles are contiguous or something like that)
        // and m_aTile, m_bTile will be set as the selected tiles
        List<int> selected = new List<int>();
        for(int i = 0; i < k_col * k_row; i++){
            if(m_tiles[i].script.m_selected){
                selected.Add(i);
                m_tiles[i].script.m_selected = false;
            }
        }

        (int i, int j) A = IndexToRC(selected[0]);
        (int i, int j) B = IndexToRC(selected[1]);
        // we may have other directions
        // List<(int i, int j)> direction = new List<(int i, int j)>{(1, 0), (-1, 0), (0, -1), (0, 1), (-1, -1), (1, 1), (-1, 1), (1, -1)};
        List<(int i, int j)> direction = new List<(int i, int j)>{(1, 0), (-1, 0), (0, -1), (0, 1)};    // four direction swap
        foreach((int i, int j) d in direction){
            if((A.i + d.i == B.i) && (A.j + d.j == B.j)){
                m_aTile = selected[0];
                m_bTile = selected[1];
                return true;
            }
        }
        return false;
    }

    List<int> CheckRowMatch(int index, bool ur=false){
        // return tiles only if there are more than 1 same tiles
        (int row, int col) = IndexToRC(index);
        int cnt = 0;
        List<int> tempSeq = new List<int>();
        // to right
        for(int c = col + 1; c < k_col; c++){
            int newIndex = RCToIndex(row, c);
            if(TilesCmp(index, newIndex)){
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
            if(TilesCmp(index, newIndex)){
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
            if(TilesCmp(index, newIndex)){
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
            if(TilesCmp(index, newIndex)){
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

    List<int> MatchesAt(int index, bool ur=false){
        // if there is no match involves tiles[index], return a list of size 0
        // ur means only up and right search
        List<int> matchedTiles = new List<int>();
        matchedTiles.Add(index);    // add as a marker

        // row-wise
        List<int> temp = CheckRowMatch(index, ur);
        foreach (int t in temp){
            matchedTiles.Add(t);
        }

        // col-wise
        temp = CheckColMatch(index, ur);
        foreach (int t in temp){
            matchedTiles.Add(t);
        }

        if(matchedTiles.Count < 2){
            return new List<int>();
        }
        return matchedTiles;
    }

    bool RemoveMatch(int a, int b){
        // remove matches at a and b, set tiles empty if valid
        List<int> AMatch = MatchesAt(a);
        List<int> BMatch = MatchesAt(b);
        bool IsMatch = false;

        if(AMatch.Count > 0){
            IsMatch = true;
            foreach(int t in AMatch){
                SetTileEmpty(t, true);
            }
        }
        if(BMatch.Count > 0){
            IsMatch = true;
            foreach(int t in BMatch){
                SetTileEmpty(t, true);
            }
        }

        if(!IsMatch){
            return false;
        }
        return true;
    }

    void SetDropState(){
        // initialization
        InitSwing();
        foreach(Tile t in m_tiles){
            t.dropUsed = false;
            t.drop = false;
            t.emptyTilesCnt = 0;
        }

        // counter how many empty tiles along the direction
        for(int i = 0; i< m_numTiles; i++){
            if(m_tiles[i].empty){
                int numEmpty = 1;
                (int eR, int eC) = IndexToRC(i);
                int r = eR - m_direction[m_dirIndex].r;
                int c = eC -  m_direction[m_dirIndex].c;
                while(r > -1 && r < k_row && c > -1 && c < k_col){
                    int temp = RCToIndex(r, c);
                    if(m_tiles[temp].empty){
                        numEmpty += 1;
                    }
                    r -= m_direction[m_dirIndex].r;
                    c -= m_direction[m_dirIndex].c;
                }
                r = eR + m_direction[m_dirIndex].r;
                c = eC +  m_direction[m_dirIndex].c;
                while(r > -1 && r < k_row && c > -1 && c < k_col){
                    int temp = RCToIndex(r, c);
                    if(m_tiles[temp].empty){
                        numEmpty += 1;
                    }
                    r += m_direction[m_dirIndex].r;
                    c += m_direction[m_dirIndex].c;
                }
                m_tiles[i].emptyTilesCnt = numEmpty;
                m_tiles[i].dropUsed = true;
            }
        }

        // mark all tiles that need to drop, and update the counter for all
        for(int i = 0; i < m_numTiles; i++){
            if(m_tiles[i].empty){
                m_tiles[i].drop = true;
                (int destR, int destC) = IndexToRC(i);
                int r = destR - m_direction[m_dirIndex].r;
                int c = destC -  m_direction[m_dirIndex].c;
                while(r > -1 && r < k_row && c > -1 && c < k_col){
                    int temp = RCToIndex(r, c);
                    m_tiles[temp].emptyTilesCnt = m_tiles[i].emptyTilesCnt;    // how many empty tiles along the direction
                    m_tiles[temp].drop = true;
                    r -= m_direction[m_dirIndex].r;
                    c -= m_direction[m_dirIndex].c;
                }
            }
        }

        // change their start position (with correct figure) and target position
        for(int i = 0; i < m_numTiles; i++){
            if(m_tiles[i].drop){
                int cnt = m_tiles[i].emptyTilesCnt;
                Vector3 startPos = Vector3.zero;
                bool setDone = false;
                (int r, int c) = IndexToRC(i);
                // find unused first
                r -= m_direction[m_dirIndex].r;
                c -= m_direction[m_dirIndex].c;
                while(r > -1 && r < k_row && c > -1 && c < k_col){
                    // start position is on the board
                    int index = RCToIndex(r, c);
                    if(m_tiles[index].dropUsed){
                        r -= m_direction[m_dirIndex].r;
                        c -= m_direction[m_dirIndex].c;
                        continue;
                    }
                    startPos = m_tiles[index].tile.transform.position;
                    m_tiles[index].dropUsed = true;
                    m_tiles[i].SetColor(m_tiles[index].color);
                    m_tiles[i].SetType(m_tiles[index].type);
                    setDone = true;
                    break;
                }
                if(!setDone){
                    // start position is out of the board
                    startPos = m_tiles[i].tile.transform.position;
                    startPos.y -= cnt * m_rDist * m_direction[m_dirIndex].r;
                    startPos.z -= cnt * m_cDist * m_direction[m_dirIndex].c;
                    m_tiles[i].SetColor(Random.Range(0, m_numColor));
                    m_tiles[i].SetType(Random.Range(0, m_numType));
                }
                SetTileEmpty(i, false);
                m_tiles[i].dropStartV = startPos;
                // target position is its corrent position
                Vector3 destPos = m_tiles[i].tile.transform.position;
                m_tiles[i].dropDestV = destPos;
            }
        }

        m_rayBlocker.enabled = true;
        // m_rayBlockerR.enabled = true;
        m_isDropping = true;
    }

    void RemoveMatchAfterDrop(){
        bool finished = true;
        for(int i = 0; i < m_numTiles; i++){
            List<int> temp = MatchesAt(i);
            foreach(int t in temp){
                SetTileEmpty(t, true);
                finished = false;
            }
        }
        if(finished){
            Shuffle();
            return;
        }
        SetDropState();
        AniTileDrop();
    }

    List<(int i, int j)> CheckMap(bool init = false){
        if(init){
            InitSwing();
        }

        // from bottom left check whether there is a solution to the map
        List<(int i, int j)> res = new List<(int i, int j)>();    // each element contains two tiles for a solution
        for(int i = 0; i < m_numTiles; i++){
            (int r, int c) = IndexToRC(i);
            // swap with up
            int upTile = r + 1 < k_row && c < k_col ? RCToIndex(r + 1, c) : m_numTiles;
            if(upTile < m_numTiles){
                TilesSwap(i, upTile, true);
                int hasMatch = MatchesAt(i, true).Count + MatchesAt(upTile).Count;
                if(hasMatch > 0){
                    res.Add((i, upTile));
                }
                TilesSwap(i, upTile, true);
            }
            // swap with up
            int rightTile = r < k_row && c + 1 < k_col ? RCToIndex(r, c + 1) : m_numTiles;
            if(rightTile < m_numTiles){
                TilesSwap(i, rightTile, true);
                int hasMatch = MatchesAt(i, true).Count + MatchesAt(rightTile).Count;
                if(hasMatch > 0){
                    res.Add((i, rightTile));
                }
                TilesSwap(i, rightTile, true);
            }
        }
        return res;
    }

    void Shuffle(){
        List<(int i, int j)> solutions = CheckMap();
        bool hasMatching = false;
        for(int i = 0; i< m_numTiles; i++){
            if(MatchesAt(i).Count != 0){
                hasMatching = true;
            }
        }
        if(solutions.Count == 0 || hasMatching){
            for(int i = 0; i < m_numTiles; i++){
                TilesSwap(i, Random.Range(0, m_numTiles));
            }
            Shuffle();
        }
        else{
            return;
        }
    }

    // ------------------------------------------------------------------------
    // animation triggers
    //      to turn on the trigger of related variables
    // ------------------------------------------------------------------------

    public void AniTileSwap(){
        // animation for swapping tile a and tile b
        // turn on the triggers
        m_isSwapping = true;
        m_rayBlocker.enabled = true;    // play animation; diabled click
        // m_rayBlockerR.enabled = true;
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
        // trigger
        // change position
        for(int i = 0; i < m_numTiles; i ++){
            if(m_tiles[i].drop){
                m_tiles[i].tile.transform.position = m_tiles[i].dropStartV;
                // trigger
                m_tiles[i].script.m_moveAniV.trigger = true;
                m_tiles[i].script.m_moveAniV.target = m_tiles[i].dropDestV;
                m_tiles[i].script.m_drop = true;
            }
        }
    }

    void UCheckMap(){
        if(Input.GetButtonDown("Jump") && !m_rayBlocker.enabled){
            List<(int i, int j)> solutions = CheckMap(true);
            (int i, int j) sol = solutions[Random.Range(0, solutions.Count)];
            m_tiles[sol.i].script.SetSwing(true);
            m_tiles[sol.j].script.SetSwing(true);
        }
    }

    // ------------------------------------------------------------------------
    // animation updates
    //      to detect whether an animation is done and start next job
    // ------------------------------------------------------------------------

    void UAniTileSwap(){
        // called every swap
        if(m_isSwapping && !m_tiles[m_aTile].script.m_moveAniV.trigger){
            // the swap is finished visually, but we still need to change the appearance...
            m_isSwapping = false;
            TilesSwap();    // when the animation is done, change logically
            m_isReversing = !m_isReversing;    // check whether the swap needs to be reversed, the default value is false
            m_rayBlocker.enabled = false;
            // m_rayBlockerR.enabled = false;
        }

        if(m_isReversing){
            // check
            if(!RemoveMatch(m_aTile, m_bTile)){
                // no valid remove
                AniTileSwap();
            }
            else{
                // the matched tiles are set to be empty now
                // fill in the empty tiles
                SetDropState();
                AniTileDrop();
                m_isReversing = false;
            }
        }
    }

    void UAniTileDrop(){
        // animation for dropping a to d
        if(m_isDropping){
            bool hasDrop = false;
            for(int i = 0; i < m_numTiles; i++){
                if(m_tiles[i].script.m_drop){
                    hasDrop = true;
                }
            }
            if(!hasDrop){
                // drop done
                m_isDropping = false;
                m_rayBlocker.enabled = false;  // no blocker
                // m_rayBlockerR.enabled = false;
                RemoveMatchAfterDrop();
                m_creatureScript.TakeDamage();
            }
        }
    }
}
