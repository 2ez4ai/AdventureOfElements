using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile{
    public GameObject tile;
    public TileLogic logic;
    public int color;
    public int type;
    public bool empty;
    public int emptyTilesCnt = 0;    // drop from where
    public bool drop = false;
    public bool dropUsed = false;
    public Vector3 dropStartV;
    public Vector3 dropDestV;

    public void SetColor(int c = -1){
        if(c != -1){
            color = c;
        }
        logic.SetColor(color);
    }

    public void SetType(int t = -1){
        if(t != -1){
            type = t;
        }
        logic.SetType(type);
    }

    public void SetEmptyState(bool state){
        logic.SetRemoveState(state);
    }

    public void UpdateLogic(){
        color = logic.GetColorIndex();
        type = logic.GetTypeIndex();
    }
}

public class Board : MonoBehaviour
{
    [SerializeField] public int randomSeed = 0;

    // board settings
    const int k_row = 8;
    const int k_col = 8;
    [SerializeField] public int m_dirIndex = 1;    // other direction can be buggy
    List<(int r, int c)> m_direction = new List<(int r, int c)>{(1, 0), (-1, 0), (0, -1), (0, 1)};    // to where
    float m_rDist;    // the distance between two row
    float m_cDist;

    // tiles settings
    int m_numTiles;
    [SerializeField] public int m_numColor;
    [SerializeField] public int m_numType;
    GameObject[] m_tilesInit = new GameObject[k_row * k_col];
    List<Tile> m_tiles = new List<Tile>();
    int m_aTile = -1;    // selected tiles
    int m_bTile = -1;

    // others
    [SerializeField] PlayerLogic m_playerScript;
    [SerializeField] CreatureLogic m_creatureScript;

    // animation related variables
    [SerializeField] Collider m_rayBlocker;
    [SerializeField] MeshRenderer m_rayBlockerR;
    // for swap
    bool m_isSwapping = false;
    bool m_isReversing = false;    // to reverse a swap if there is no match
    // for remove
    bool m_isRemoving = false;
    // for drop
    bool m_isDropping = false;
    public bool m_stepDone = true;    // whether a swap is processed
    // for shrink
    public bool m_isShrinking = false;
    float m_shrinkSpeed = 3f;
    public bool m_isExpanding = false;
    float m_expandSpeed = 3f;

    // skill things
    // for special
    [SerializeField] public int m_specialSkill = 0;
    int m_lastSpecial = -1;
    // for diagonal swap
    [SerializeField] public int m_diagonalSwapLV = 0;
    List<int> m_diagonalChance = new List<int>{0, 10, 20, 30};
    bool m_lastSwapIsDiagonal = false;
    // for stomp
    [SerializeField]public SkillStomp m_stompSkillLogic;

    // Start is called before the first frame update
    void Start()
    {
        InitSeed();
        //Initialization();
    }

    // Update is called once per frame
    void Update()
    {
        UDisableInteraction();
        UAniTileSwap();
        UAniRemove();
        UAniTileDrop();
        UCheckMap();
    }

    void FixedUpdate(){
        UBoardExpand();
        UBoardShrink();
    }

    // ------------------------------------------------------------------------
    // helper function things
    // ------------------------------------------------------------------------

    (int row, int col) IndexToRC(int index){
        int row = index / k_col;
        int col = index % k_col;
        return (row, col);
    }

    public int RCToIndex(int row, int col){
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
        m_tiles[j].SetType(tempType);

        if(!m_tiles[i].logic.m_isSpecial){
            m_tiles[i].SetColor(m_tiles[j].color);
        }
        if(!m_tiles[j].logic.m_isSpecial){
            m_tiles[j].SetColor(tempColor);
        }
    }

    void SetTileEmpty(int index, bool state, bool nonSpecial=true){
        if(index == -1){
            return;
        }
        if(m_tiles[index].empty && state){
            return;    // already be removed
        }
        if(index == m_lastSpecial && state){
            // instead setting it empty to be removed, we set it as special
            m_tiles[m_lastSpecial].logic.SetSpecial(true);
            m_lastSpecial = -1;
            return;
        }
        m_tiles[index].empty = state;
        m_tiles[index].SetEmptyState(state);
        if(state && nonSpecial){    // there is a remove
            SoundManager.m_instance.PlayTileRemoveSound();
            if(m_specialSkill != 0){
                CheckSpecial(index);
            }
            m_creatureScript.UpdateStepDamage(m_tiles[index].color, m_tiles[index].type);
        }
    }

    void UDisableInteraction(){
        if(m_stepDone){
            m_rayBlocker.enabled = false;
            // m_rayBlockerR.enabled = false;
        }
        else if(!m_stepDone){
            m_rayBlocker.enabled = true;
            // m_rayBlockerR.enabled = true;
        }
    }

    public bool RCInBoard(int r, int c){
        if(r < 0 || c < 0 || r >= k_row || c >= k_col ){
            return false;
        }
        return true;
    }

    // ------------------------------------------------------------------------
    // initialization
    // ------------------------------------------------------------------------

    void InitSeed(){
        // init random seed
        // if(randomSeed == 0){
        //     // generate a new randomseed
        //     randomSeed = Random.Range(10000000, 99999999);
        // }
        randomSeed = Random.Range(10000000, 99999999);
        if(LocalizationManager.m_instance.loadChecker){
            randomSeed = PlayerPrefs.GetInt("RandomSeed");
        }
        Random.InitState(randomSeed);
    }

    public void Initialization(){
        InitSeed();
        InitTiles();
        InitCheck();    // make sure there is no matching at the begining
        InitDraw();
    }

    void InitTiles(){
        m_tilesInit = new GameObject[k_row * k_col];
        m_tiles = new List<Tile>();
        m_tilesInit = GameObject.FindGameObjectsWithTag("Tiles");
        m_numTiles = k_row * k_col;
        for(int i = 0; i < m_numTiles; i++){
            Tile temp = new Tile();
            temp.tile = m_tilesInit[i];
            temp.logic = m_tilesInit[i].GetComponent<TileLogic>();
            temp.logic.m_isSpecial = false;
            temp.color = Random.Range(0, m_numColor);    // logically
            temp.type = Random.Range(0, m_numType);    // logically
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
                List<int> temp = MatchesAt(i, true, true);
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
            t.logic.SetSwing(false);
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
        // set m_aTile, m_bTile as the selected tiles
        List<int> selected = new List<int>();
        for(int i = 0; i < k_col * k_row; i++){
            if(m_tiles[i].logic.m_selected){
                selected.Add(i);
                m_tiles[i].logic.m_selected = false;
            }
        }

        (int i, int j) A = IndexToRC(selected[0]);
        (int i, int j) B = IndexToRC(selected[1]);

        List<(int i, int j)> direction = new List<(int i, int j)>{(1, 0), (-1, 0), (0, -1), (0, 1)};    // four direction swap is allowed here
        if(m_diagonalSwapLV != 0){
            direction = new List<(int i, int j)>{(1, 0), (-1, 0), (0, -1), (0, 1), (-1, -1), (1, 1), (-1, 1), (1, -1)};
        }
        foreach((int i, int j) d in direction){
            if((A.i + d.i == B.i) && (A.j + d.j == B.j)){
                m_aTile = selected[0];
                m_bTile = selected[1];
                if(d.i + d.j == 0 || d.i + d.j == -2 || d.i + d.j == 2){
                    m_lastSwapIsDiagonal = true;
                }
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
        // to up
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
        // to down
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

    List<int> MatchesAt(int index, bool ur=false, bool check=false){
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
        // special tile generation
        if(m_specialSkill != 0 && temp.Count > 2 && !check){
            List<int> tempColList = new List<int>();
            tempColList.Add(index);
            foreach(int t in temp){
                tempColList.Add(t);
            }
            MarkSpecialTile(tempColList);
        }
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
                if(m_specialSkill != 0 && t == m_lastSpecial){
                    continue;    // skip the special one
                }
                SetTileEmpty(t, true);
            }
        }
        if(BMatch.Count > 0){
            IsMatch = true;
            foreach(int t in BMatch){
                if(m_specialSkill != 0 && t == m_lastSpecial){
                    continue;    // skip the special one
                }
                SetTileEmpty(t, true);
            }
        }

        if(!IsMatch){
            return false;
        }

        if(m_specialSkill != 0){
            SetTileEmpty(m_lastSpecial, true);    // skip the special one
        }

        return true;
    }

    void RemoveSpecial(int index){
        (int r, int c) = IndexToRC(index);
        List<(int r, int c)> direction = new List<(int r, int c)>();
        if(m_specialSkill == 1){
            direction = new List<(int r, int c)>{(1, 0), (-1, 0), (0, -1), (0, 1)};
        }
        if(m_specialSkill == 2){
            direction = new List<(int r, int c)>{(1, 0), (-1, 0), (0, -1), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1)};
        }
        if(m_specialSkill == 3){
            direction = new List<(int r, int c)>{(1, 0), (-1, 0), (0, -1), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1), (-2, 0), (2, 0), (0, 2), (0, -2)};
        }
        for(int i = 0; i < direction.Count; i++){
            int nr = r + direction[i].r;
            int nc = c + direction[i].c;
            if(RCInBoard(nr, nc)){
                int temp = RCToIndex(nr, nc);
                if(!m_tiles[temp].empty){
                    // remove special tiles
                    SetTileEmpty(temp, true, false);
                }
            }
        }
        m_tiles[index].logic.SetSpecial(false);
        SetTileEmpty(index, true, false);
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

        // change their color, type, start position, and target position
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

        m_isDropping = true;
    }

    void RemoveMatchAfterDrop(){
        bool finished = true;
        for(int i = 0; i < m_numTiles; i++){
            if(!m_tiles[i].empty && !m_tiles[i].logic.m_isSpecial){
                List<int> temp = MatchesAt(i);
                foreach(int t in temp){
                    if(m_specialSkill != 0 && t == m_lastSpecial){
                        continue;    // skip the special one
                    }
                    SetTileEmpty(t, true);
                    finished = false;
                }
                SetTileEmpty(m_lastSpecial, true);
            }
        }
        if(finished){
            Shuffle();
            if(m_creatureScript.m_HP > 0){
                m_playerScript.TakeDamage();
            }
            m_stepDone = true;
            return;
        }
        else{
            m_isRemoving = true;
        }
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
                int hasMatch = MatchesAt(i, true, true).Count + MatchesAt(upTile, false, true).Count;
                if(hasMatch > 0){
                    res.Add((i, upTile));
                }
                TilesSwap(i, upTile, true);
            }
            // swap with up
            int rightTile = r < k_row && c + 1 < k_col ? RCToIndex(r, c + 1) : m_numTiles;
            if(rightTile < m_numTiles){
                TilesSwap(i, rightTile, true);
                int hasMatch = MatchesAt(i, true, true).Count + MatchesAt(rightTile, false, true).Count;
                if(hasMatch > 0){
                    res.Add((i, rightTile));
                }
                TilesSwap(i, rightTile, true);
            }
            //diagonal
            if(m_diagonalSwapLV != 0){
                int upRightTile = r + 1 < k_row && c + 1 < k_col ? RCToIndex(r + 1, c + 1) : m_numTiles;
                if(upRightTile < m_numTiles){
                    TilesSwap(i, upRightTile, true);
                    int hasMatch = MatchesAt(i, true, true).Count + MatchesAt(upRightTile, false, true).Count;
                    if(hasMatch > 0){
                        res.Add((i, upRightTile));
                    }
                    TilesSwap(i, upRightTile, true);
                }
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
    // battle
    // ------------------------------------------------------------------------
    public List<Tile> TilesToPlayer(){
        List<Tile> tiles = new List<Tile>(m_tiles);
        return tiles;
    }

    // ------------------------------------------------------------------------
    // skill
    // ------------------------------------------------------------------------
    void CheckSpecial(int index){
        // check whether there is a special tile around tile[index]
        List<(int r, int c)> direction = new List<(int r, int c)>{(1, 0), (-1, 0), (0, -1), (0, 1)};
        (int r, int c) = IndexToRC(index);
        for(int i = 0; i < direction.Count; i++){
            int nr = r + direction[i].r;
            int nc = c + direction[i].c;
            if(RCInBoard(nr, nc)){
                if(m_tiles[RCToIndex(nr, nc)].logic.m_isSpecial){
                    RemoveSpecial(RCToIndex(nr, nc));
                }
            }
        }
    }

    void MarkSpecialTile(List<int> matchedTiles){
        (int r, int c) = IndexToRC(matchedTiles[0]);
        for(int i = 1; i < matchedTiles.Count; i++){
            (int nr, int nc) = IndexToRC(matchedTiles[i]);
            if(nr < r){    // find the tile with the lowest row index
                r = nr;
                c = nc;
            }
        }
        m_lastSpecial = RCToIndex(r, c);
    }

    void GenerateSpecialTIle(){
        if(m_lastSpecial != -1){
            // we need to generate a special tile at m_lastSpecial
            m_tiles[m_lastSpecial].logic.SetSpecial(true);
        }
        m_lastSpecial = -1;
    }

    public void GenerateStompArea(int level){
        if(level == 0){
            foreach(Tile t in m_tiles){
               TileLogic tl = t.logic;
                tl.SetStompDamage(0);
            }
            m_stompSkillLogic.Activate(0, 0, 0, 0);
            return;
        }
        int LUIndex = -1;
        (int r, int c) = IndexToRC(LUIndex);
        while(r < level + 1 || r >= k_row || c < 0 || k_col - 1 - c < level + 2){
            LUIndex = Random.Range(0, m_numTiles);
            (r, c) = IndexToRC(LUIndex);
        }
        m_stompSkillLogic.Activate(r, c, level + 2, m_playerScript.m_injureType);
        for(int i = level + 2; i > 0; i--){
            for(int j = 0; j < level + 2; j++){
                int nr = r - (level + 2 - i);
                int nc = c + j + 1;
                // Debug.Log("nr : " + nr + "; nc : " + nc);
                m_tiles[RCToIndex(nr, nc)].logic.SetStompDamage(level);
                // Debug.Log("Tile " + RCToIndex(nr, nc) + " would cause " + level + " extra damage.");
            }
        }
    }
    // ------------------------------------------------------------------------
    // animation triggers
    //      to turn on the trigger of related variables
    // ------------------------------------------------------------------------

    public void AniTileSwap(){
        // animation for swapping tile a and tile b
        // turn on the triggers
        m_stepDone = false;
        m_isSwapping = true;    // will trigger UAniTileSwap()
        // two tiles start moving
        Vector3 aPos = m_tiles[m_aTile].tile.transform.position;
        Vector3 bPos = m_tiles[m_bTile].tile.transform.position;
        aPos.x = 0.0f;
        bPos.x = 0.0f;
        m_tiles[m_aTile].logic.m_moveAniV.trigger = true;
        m_tiles[m_aTile].logic.m_moveAniV.target = bPos;
        m_tiles[m_bTile].logic.m_moveAniV.trigger = true;
        m_tiles[m_bTile].logic.m_moveAniV.target = aPos;
        bool specail = m_tiles[m_aTile].logic.m_isSpecial;
        m_tiles[m_aTile].logic.m_moveAniV.targetSpecial = m_tiles[m_bTile].logic.m_isSpecial;
        m_tiles[m_bTile].logic.m_moveAniV.targetSpecial = specail;
    }

    void AniTileDrop(){
        // trigger
        // change position
        for(int i = 0; i < m_numTiles; i ++){
            if(m_tiles[i].drop){
                m_tiles[i].tile.transform.position = m_tiles[i].dropStartV;
                // trigger
                m_tiles[i].logic.m_moveAniV.trigger = true;
                m_tiles[i].logic.m_moveAniV.target = m_tiles[i].dropDestV;
                m_tiles[i].logic.m_drop = true;
            }
        }
    }

    void UCheckMap(){
        if(Input.GetButtonDown("Jump") && m_stepDone){
            List<(int i, int j)> solutions = CheckMap(true);
            (int i, int j) sol = solutions[Random.Range(0, solutions.Count)];
            m_tiles[sol.i].logic.SetSwing(true);
            m_tiles[sol.j].logic.SetSwing(true);
        }
    }

    void AniDamageToCreature(int index){
        // trigger the animation of damage dealer of tile index
    }
    // ------------------------------------------------------------------------
    // animation updates
    //      to detect whether an animation is done and start next job
    // ------------------------------------------------------------------------

    void UAniTileSwap(){
        // called every swap
        if(m_isSwapping && !m_tiles[m_aTile].logic.m_moveAniV.trigger){
            // the swap is finished visually, but we still need to change the appearance...
            m_isSwapping = false;
            TilesSwap();    // when the animation is done, change logically
            m_isReversing = !m_isReversing;    // check whether the swap needs to be reversed, the default value is false
        }

        if(m_isReversing){
            // check
            if(!RemoveMatch(m_aTile, m_bTile)){
                // no valid remove
                AniTileSwap();
                m_lastSwapIsDiagonal = false;
                m_stepDone = true;
            }
            else{
                // the matched tiles are set to be empty now
                // fill in the empty tiles
                if(m_diagonalSwapLV != 0 && m_lastSwapIsDiagonal){
                    m_lastSwapIsDiagonal = false;
                    int dice = Random.Range(0, 100);
                    if(dice > m_diagonalChance[m_diagonalSwapLV]){
                        m_playerScript.IncreStepCnt();
                    }
                }
                else{
                    m_playerScript.IncreStepCnt();
                }
                m_isRemoving = true;    // trigger UAniRemove()
                m_isReversing = false;
            }
        }
    }

    void UAniRemove(){
        // check whether remove animation is done
        if(m_isRemoving){
            bool removeAniDone = true;
            foreach(Tile t in m_tiles){
                removeAniDone &= t.logic.m_removeDone;
            }
            if(removeAniDone){
                SetDropState();
                AniTileDrop();
                m_isRemoving = false;
            }
        }
    }

    void UAniTileDrop(){
        // animation for dropping s to d
        if(m_isDropping){
            bool hasDrop = false;
            for(int i = 0; i < m_numTiles; i++){
                if(m_tiles[i].logic.m_drop){
                    hasDrop = true;
                }
            }
            if(!hasDrop){
                // drop done
                m_isDropping = false;
                m_creatureScript.TakeDamage();
                RemoveMatchAfterDrop();
            }
        }
    }

    void UBoardShrink(){
        if(m_isShrinking && transform.localScale.y > 2.01f){
            if(m_isExpanding){
                m_isExpanding = false;
            }
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.1f, 2.0f, 2.0f), m_shrinkSpeed * Time.deltaTime);
        }
        else{
            m_isShrinking = false;
        }
    }

    void UBoardExpand(){
        if(m_isExpanding && transform.localScale.y < 15.99f){
            if(m_isShrinking){
                m_isShrinking = false;
            }
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.1f, 16.0f, 16.0f), m_expandSpeed * Time.deltaTime);
        }
        else{
            m_isExpanding = false;
        }
    }
}
