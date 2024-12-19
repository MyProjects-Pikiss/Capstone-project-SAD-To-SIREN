using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using System.Text.RegularExpressions;

public enum GameMode
{
    ViewTileInfo,
    PlaceDrones,
    BattleStart,
    PlayerTurn,
    EnemyTurn,
    Lose
}

public class BattleUnitList
{
    public List<BattleUnit> u_list = new List<BattleUnit>();

    public void SortBySpeed()
    {
        u_list.Sort((x, y) => y.u_speed.CompareTo(x.u_speed)); // 내림차순 정렬
    }
    public List<int> TeamIndices(int n)
    {
        return u_list.Select((unit, index) => new { unit, index })
                     .Where(x => x.unit.u_team == n && x.unit.u_hp > 0)
                     .Select(x => x.index)
                     .ToList();
    }
}

public class BattleUnit
{
    public int u_code = -1;
    public int u_team;
    public int u_tileIndex;
    public int u_ID;
    public string name;
    public int u_max_hp;
    public int u_hp;
    public int u_armor;
    public int u_speed;
    public int u_ap;
    public int u_max_weight;
    public int u_weight;
    public List<List<string>> u_skill;
    public List<string> u_gear;
    public List<ItemAmount> u_itemAmount;
    public List<magazine> u_magazine;

    public BattleUnit(int u_team_, int u_tileIndex_, string name_, int u_max_hp_, int u_hp_, int u_armor_, int u_speed_, int u_max_weight_, int u_weight_, int u_ap_)
    {
        u_team = u_team_; u_tileIndex = u_tileIndex_; name = name_; u_max_hp = u_max_hp_; u_hp = u_hp_; u_armor = u_armor_; u_speed = u_speed_; u_ap = u_ap_; u_max_weight = u_max_weight_; u_weight = u_weight_;
    }
}

public class feildTile_method : MonoBehaviour
{
    public Tilemap tilemap;
    public fieldMapTile_script fieldMapTileScript; //스크립트

    public GameMode currentMode = GameMode.ViewTileInfo;
    public TileBase deployTile;
    public TileBase zeroTile;

    public List<Drone> drones = new List<Drone>();
    public GameObject[] droneDeploy = new GameObject[5];
    public TMP_Text[] droneName = new TMP_Text[5];
    public TMP_Text[] droneHP = new TMP_Text[5];
    public TMP_Text[] droneWeight = new TMP_Text[5];
    public GameObject deploy_img;
    public TMP_Text deploy_text;
    private int? currentlySelectedDroneIndex = null;
    string[] letters = { "A", "B", "C", "D", "E" };
    public TileBase[] droneTile;
    public TileBase enemyTile;
    private Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>();
    private Dictionary<Vector3Int, TileInfo> originalInfos = new Dictionary<Vector3Int, TileInfo>();
    private Dictionary<Vector3Int, Drone> deployedDrones = new Dictionary<Vector3Int, Drone>();

    public GameObject deployDrones;
    public GameObject start;

    public FieldTileData battleTileData;
    public BattleUnitList battleUnitList = new BattleUnitList();
    public TextAsset item_s_database;
    public TextAsset skill_database;

    private List<GameObject> buttonsList1 = new List<GameObject>();
    public Transform buttonParent1;
    public GameObject buttonPrefab1;
    public GameObject scrollView1;

    public int nowTurn = 0;
    public int skillIndex = -1;
    private bool hasCoroutineStarted = false;

    private Vector3Int prevCellPosition; // 이전에 마우스가 올라간 셀 위치
    private Color prevColor; // 이전 셀의 원래 색상

    public feildTile_Astar_script feildTileAstarscript; //스크립트

    public GameObject unitUI;
    public TMP_Text[] unitText;
    public TMP_Text[] unitText2;
    public TMP_Text[] unitText3;
    public GameObject buttonPrefab3;
    public GameObject buttonPrefab4;
    public GameObject buttonPrefab5;
    public Transform buttonParent2;
    public int numberOfButtons1;
    private List<GameObject> buttonsList3 = new List<GameObject>();
    private List<GameObject> buttonsList4 = new List<GameObject>();
    private List<GameObject> buttonsList5 = new List<GameObject>();
    private Dictionary<string, item_s> itemDict1;

    public GameObject unitSkillUI;
    public GameObject[] skillBTN;
    public TMP_Text[] skillWeapon;
    public TMP_Text[] skillName;
    public TMP_Text[] skillTurn;
    public TMP_Text[] skillRange;
    public GameObject reloadUI;
    public TMP_Text[] reloadText;
    private bool playerCheck = false;
    private int playerAP = -1;
    private List<HexTile> field;
    public GameObject follower;
    public TMP_Text[] followerText;
    public GameObject attackInfo;
    public TMP_Text[] attackInfoText;
    public GameObject APobj;
    public TMP_Text APText;
    public GameObject turnEnd;

    public GameObject reloadFuncUI;
    public GameObject[] reloadFuncBTN;
    public TMP_Text[] reloadFuncText;
    public GameObject fieldReload;
    public GameObject fieldReloadPrefab;
    public Transform fieldReloadParent;
    public string reloadWeapon;
    private List<GameObject> fieldReloadList1 = new List<GameObject>();
    public GameObject winLose;
    public TMP_Text winLoseText;

    public GameObject rootingUI;
    public TMP_Text moveText2;
    private List<GameObject> buttonsListA1 = new List<GameObject>();
    private List<GameObject> buttonsListA2 = new List<GameObject>();
    private List<GameObject> buttonsListA3 = new List<GameObject>();
    private List<GameObject> buttonsListB1 = new List<GameObject>();
    private List<GameObject> buttonsListB2 = new List<GameObject>();
    private List<GameObject> buttonsListB3 = new List<GameObject>();
    private Dictionary<string, item_s> itemDictA;
    private Dictionary<string, item_s2> itemDictB;
    public int numberOfButtonsA;
    public int numberOfButtonsB;
    public Transform buttonParentA;
    public Transform buttonParentB;
    public GameObject buttonPrefab6;
    private Vector3Int rootPosition;

    public string skillString;

    public void Start()
    {
        DataManager.Instance.LoadGameData0();
        deployDrones.SetActive(true);
        start.SetActive(true);
        deploy_img.SetActive(false);
        unitUI.SetActive(false);
        unitSkillUI.SetActive(false);
        reloadUI.SetActive(false);
        drones = getDroneList();
        getOrigin();
    }

    private void Update()
    {
        if(currentMode == GameMode.ViewTileInfo && Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            HandleClick();
        }
        if (currentMode == GameMode.PlaceDrones && Input.GetMouseButtonDown(0) && !IsPointerOverUIObject()) // 0은 왼쪽 마우스 버튼
        {
            HandleClick();
        }
        if (currentMode == GameMode.BattleStart)
        {
            battleTileData = fieldMapTileScript.GetTileData();
            allUnitCalculate();
            GenerateButtons(battleUnitList);
            battlePhase();
        }
        if(currentMode == GameMode.PlayerTurn && skillIndex == -1 && !playerCheck && !IsPointerOverUIObject())
        {
            playerPhase();
            playerCheck = true;
        }
        if (currentMode == GameMode.PlayerTurn && skillIndex == -1 && !IsPointerOverUIObject())
        {
            HandleClick();
        }
        if (currentMode == GameMode.PlayerTurn && skillIndex != -1 && !IsPointerOverUIObject())
        {
            ChangeTileColorUnderMouse();
            HandleClick();
        }
        if (currentMode == GameMode.EnemyTurn && !hasCoroutineStarted)
        {
            hasCoroutineStarted = true;
            StartCoroutine(enemyPhase());
        }
    }

    private void HandleClick()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        cellPosition.z = 0;

        if (currentMode == GameMode.ViewTileInfo)
        {
            // 클릭된 위치에서 타일 정보 가져오기
            TileInfo clickedTileInfo = GetTileInfoAtPosition(cellPosition);
            if (clickedTileInfo != null)
            {
                Debug.Log("Index: " + clickedTileInfo.tileIndex + "/Code: " + clickedTileInfo.tileCode + "/Position: " + cellPosition);
                if(clickedTileInfo.tileCode >= 10 && clickedTileInfo.tileCode <= 14)
                {
                    RemoveDrone(cellPosition);
                }
            }
        }
        else if (currentMode == GameMode.PlaceDrones)
        {
            PlaceDrone(cellPosition);
        }
        else if (!Input.GetMouseButtonDown(0) && currentMode == GameMode.PlayerTurn)
        {
            TargetInfo1(cellPosition);
        }
        else if (Input.GetMouseButtonDown(0) && currentMode == GameMode.PlayerTurn)
        {
            playerAct(cellPosition);
        }
    }

    private void TargetInfo1(Vector3Int cellPosition)
    {
        if (cellPosition == null) return;
        TileInfo onTileInfo = GetTileInfoAtPosition(cellPosition);
        follower.SetActive(true);
        Vector3 worldPosition = tilemap.CellToWorld(cellPosition) + new Vector3(0, 150, 0);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        follower.transform.position = screenPosition;
        if (onTileInfo == null || GetUnitFromIndex(onTileInfo.tileIndex) == null) { follower.SetActive(false); return; }
        BattleUnit unit = GetUnitFromIndex(onTileInfo.tileIndex);

        if (onTileInfo.tileCode >= 10 && onTileInfo.tileCode <= 14 && unit != null)
        {
            followerText[0].text = unit.name;
            followerText[1].text = "HP: " + unit.u_hp + "/" + unit.u_max_hp;
            followerText[2].text = "Armor: " + unit.u_armor + " / Tile: " + unit.u_tileIndex;
        }
        else if (onTileInfo.tileCode >= 20 && onTileInfo.tileCode <= 39 & unit != null)
        {
            followerText[0].text = unit.name;
            followerText[1].text = "HP: " + unit.u_hp + "/" + unit.u_max_hp;
            followerText[2].text = "Armor: " + unit.u_armor + " / Tile: " + unit.u_tileIndex;
        }
        else
        {
            follower.SetActive(false);
        }
        
    }
    private void TargetInfo2(Vector3Int cellPosition, int a, string b, int c, int d)
    {
        if (cellPosition == null) return;
        TileInfo onTileInfo = GetTileInfoAtPosition(cellPosition);
        attackInfo.SetActive(true);
        Vector3 worldPosition = tilemap.CellToWorld(cellPosition) + new Vector3(0, 150, 0);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        attackInfo.transform.position = screenPosition;

        attackInfoText[0].text = "DMG: " + a;
        attackInfoText[1].text = b;
        attackInfoText[2].text = c + "->" + d;
    }

    private void playerAct(Vector3Int cellPosition)
    {
        int n = nowTurn;
        if (nowTurn >= battleUnitList.u_list.Count) n = nowTurn % battleUnitList.u_list.Count;
        BattleUnit nowUnit = battleUnitList.u_list[n];
        List<string> nowSkill = nowUnit.u_skill[skillIndex];
        TileInfo t_info = GetTileInfoAtPosition(cellPosition);
        if(t_info.tileCode >= 10 && t_info.tileCode <= 14)
        {
            unitTextSet(nowUnit);
            follower.SetActive(false);
            return;
        }
        if(t_info.tileCode == 1)
        {
            int x = feildTileAstarscript.GetTileDistanceWithoutWalkableCheck(GetPositionFromTileIndex(nowUnit.u_tileIndex), GetPositionFromTileIndex(t_info.tileIndex));
            if(x == 1)
            {
                rootPosition = cellPosition;
                rootingSys(nowUnit.u_itemAmount, t_info.drops);
            }
        }

        if (skillString == "move")
        {
            if (!feildTileAstarscript.GetHexTileAtPosition(cellPosition).isWalkable) return;
            Vector3Int nPos = GetPositionFromTileIndex(nowUnit.u_tileIndex);
            Vector3Int tPos = GetPositionFromTileIndex(t_info.tileIndex);
            List<HexTile> path = feildTileAstarscript.FindPath(nPos, tPos);

            TileInfo unitInfo = GetTileInfoAtPosition(nPos);
            TileBase unitTile = tilemap.GetTile(nPos);

            if (path.Count < playerAP)
            {
                Vector3Int position = nPos;
                int index = position.x + position.y * fieldMapTileScript.tileData.width;
                if(fieldMapTileScript.tileData.tilesInfo[index] == originalInfos[position])
                {
                    fieldMapTileScript.tileData.tilesInfo[index] = new TileInfo { tileIndex = index, tileCode = 0 };
                    originalInfos[position] = new TileInfo { tileIndex = index, tileCode = 0 };
                }
                else fieldMapTileScript.tileData.tilesInfo[index] = originalInfos[position];

                tilemap.SetTile(tPos, unitTile);
                tilemap.SetTile(nPos, originalTiles[nPos]);

                Vector3Int position2 = tPos;
                int index2 = position2.x + position2.y * fieldMapTileScript.tileData.width;
                nowUnit.u_tileIndex = GetTileInfoAtPosition(cellPosition).tileIndex;
                battleUnitList.u_list[n] = nowUnit;
                unitInfo.tileIndex = GetTileInfoAtPosition(tPos).tileIndex;
                fieldMapTileScript.tileData.tilesInfo[index2] = unitInfo;

                playerAP -= path.Count + 1;
                setSkillIndex(skillIndex);
            }
        }
        else
        {
            if (GetUnitFromIndex(t_info.tileIndex) != null)
            {
                BattleUnit target = GetUnitFromIndex(t_info.tileIndex);
                int x = feildTileAstarscript.GetTileDistanceWithoutWalkableCheck(GetPositionFromTileIndex(nowUnit.u_tileIndex), GetPositionFromTileIndex(target.u_tileIndex));
                if (x <= int.Parse(nowSkill[3]) && playerAP >= int.Parse(nowSkill[2]))
                {
                    playerAP -= int.Parse(nowSkill[2]);
                    fireBattleCalculate(nowUnit, skillString, target);
                }
            }
        }
        droneSkillUISet(nowUnit);
        if (playerAP <= 0)
        {
            nowTurn++;
            battlePhase();
        }
    }

    private void playerPhase()
    {
        int n = nowTurn;
        if (nowTurn >= battleUnitList.u_list.Count) n = nowTurn % battleUnitList.u_list.Count;
        BattleUnit nowUnit = battleUnitList.u_list[n];
        playerAP = nowUnit.u_ap;

        droneSkillUISet(nowUnit);
    }

    private IEnumerator enemyPhase()
    {
        int n = nowTurn;
        if (nowTurn >= battleUnitList.u_list.Count) n = nowTurn % battleUnitList.u_list.Count;
        BattleUnit nowUnit = battleUnitList.u_list[n];
        int ap = nowUnit.u_ap;

        if(nowUnit.u_hp <= 0)
        {
            nowTurn++;
            battlePhase();
            hasCoroutineStarted = false;
            yield break;
        }
        
        List<List<string>> a_skill = nowUnit.u_skill.OrderByDescending(innerList => int.Parse(innerList[2])).ToList();
        List<int> teamIndices = battleUnitList.TeamIndices(1);
        Vector3Int nPos = GetPositionFromTileIndex(nowUnit.u_tileIndex);
        List<List<HexTile>> pathes = new List<List<HexTile>>();
        for (int i = 0; i < teamIndices.Count; i++)
        {
            Vector3Int tPos = GetPositionFromTileIndex(battleUnitList.u_list[teamIndices[i]].u_tileIndex);
            pathes.Add(feildTileAstarscript.FindPath(nPos, tPos));
        }
        List<HexTile> path = pathes[0];
        // 모든 경로를 반복하며 가장 짧은 것을 찾음
        foreach (var p in pathes)
        {
            if (p.Count < path.Count)
            {
                path = p;
            }
        }

        int maxSkillRange = int.MinValue;
        foreach (var skill in nowUnit.u_skill)
        {
            int value;
            if (int.TryParse(skill[3], out value))
            {
                if (value > maxSkillRange)
                {
                    maxSkillRange = value;
                }
            }
        }
        if (path.Count + 1 > maxSkillRange) //move
        {
            int moveLength = path.Count + 1 - maxSkillRange;
            int repetition = ap;
            if (ap > moveLength) repetition = moveLength;
            for (int i = 0; i < repetition; i++)
            {
                if (ap <= 0) yield break;
                Vector3Int originalPos = GetPositionFromTileIndex(nowUnit.u_tileIndex);
                TileInfo pathInfo = GetTileInfoAtPosition(path[i].position);
                TileBase unitTile = tilemap.GetTile(originalPos);
                TileInfo unitInfo = GetTileInfoAtPosition(originalPos);
                if (originalTiles[originalPos] == unitTile)
                {
                    tilemap.SetTile(originalPos, zeroTile);
                    Vector3Int position = originalPos;
                    int index = position.x + position.y * fieldMapTileScript.tileData.width;
                    int tileIndex2 = fieldMapTileScript.tileData.tilesInfo[index].tileIndex;
                    if (index >= 0 && index < fieldMapTileScript.tileData.tilesInfo.Count)
                        fieldMapTileScript.tileData.tilesInfo[index] = new TileInfo { tileIndex = tileIndex2, tileCode = 0 };
                }
                else
                {
                    tilemap.SetTile(originalPos, originalTiles[originalPos]);
                    Vector3Int position = originalPos;
                    int index = position.x + position.y * fieldMapTileScript.tileData.width;
                    int tileIndex2 = fieldMapTileScript.tileData.tilesInfo[index].tileIndex;
                    if (index >= 0 && index < fieldMapTileScript.tileData.tilesInfo.Count)
                        fieldMapTileScript.tileData.tilesInfo[index] = originalInfos[originalPos];
                }
                Vector3Int pathPos = path[i].position;
                tilemap.SetTile(pathPos, unitTile);
                int index2 = pathPos.x + pathPos.y * fieldMapTileScript.tileData.width;
                unitInfo.tileIndex = fieldMapTileScript.tileData.tilesInfo[index2].tileIndex;
                fieldMapTileScript.tileData.tilesInfo[index2] = unitInfo;

                nowUnit.u_tileIndex = GetTileInfoAtPosition(path[i].position).tileIndex;
                battleUnitList.u_list[n] = nowUnit;

                List<string> foundInnerList = nowUnit.u_skill.FirstOrDefault(innerList => innerList[1] == "move");
                ap = ap - int.Parse(foundInnerList[2]);
                yield return new WaitForSeconds(0.5f); //대기시간 조정
                follower.SetActive(false);
            }
        }
        // 최대 사거리 내
        while (ap > 0)
        {
            bool flag = false;
            foreach (var skill in a_skill)
            {
                if (skill[1] == "move") continue;
                List<int> teamIndices2 = battleUnitList.TeamIndices(1);
                Vector3Int nPos2 = GetPositionFromTileIndex(nowUnit.u_tileIndex);

                Vector3Int shortestPathEnd = new Vector3Int();
                List<HexTile> path2 = null;
                for (int i = 0; i < teamIndices2.Count; i++)
                {
                    Vector3Int tPos2 = GetPositionFromTileIndex(battleUnitList.u_list[teamIndices2[i]].u_tileIndex);
                    List<HexTile> pathB = feildTileAstarscript.FindPath(nPos2, tPos2);
                    if (path2 == null || path2.Count > pathB.Count)
                    {
                        path2 = pathB;
                        shortestPathEnd = tPos2;
                    }
                }
                if (path2.Count + 1 <= int.Parse(skill[3]))
                {
                    if (ap >= int.Parse(skill[2]))
                    {
                        flag = true;
                        BattleUnit target = GetUnitFromIndex(GetTileInfoAtPosition(shortestPathEnd).tileIndex);
                        int AHP = target.u_hp;
                        coldBattleCalculate(nowUnit, skill, target);
                        int BHP = target.u_hp;
                        TargetInfo2(shortestPathEnd, AHP - BHP, target.name, AHP, BHP);
                        ap -= int.Parse(skill[2]);
                        yield return new WaitForSeconds(1.00f);
                        follower.SetActive(false);
                        break;
                    }
                }
            }
            if (!flag) //ap가 남았을 때
            {
                List<int> teamIndices2 = battleUnitList.TeamIndices(1);
                Vector3Int nPos2 = GetPositionFromTileIndex(nowUnit.u_tileIndex);
                List<List<HexTile>> pathes2 = new List<List<HexTile>>();
                for (int i = 0; i < teamIndices2.Count; i++)
                {
                    Vector3Int tPos2 = GetPositionFromTileIndex(battleUnitList.u_list[teamIndices2[i]].u_tileIndex);
                    pathes2.Add(feildTileAstarscript.FindPath(nPos2, tPos2));
                }
                List<HexTile> path2 = pathes2[0];
                // 모든 경로를 반복하며 가장 짧은 것을 찾음
                foreach (var p2 in pathes2)
                {
                    if (p2.Count < path2.Count)
                    {
                        path2 = p2;
                    }
                }

                if (path2.Count == 0 || path2 == null)
                {
                    ap = 0;
                }
                else //move
                {
                    int moveLength = (path2.Count < ap) ? path2.Count : ap;
                    for (int i = 0; i < moveLength; i++)
                    {
                        if (ap <= 0) break;

                        Vector3Int originalPos = GetPositionFromTileIndex(nowUnit.u_tileIndex);
                        Vector3Int targetPos = path2[i].position;
                        TileBase unitTile = tilemap.GetTile(originalPos);
                        TileInfo unitInfo = GetTileInfoAtPosition(originalPos);
                        // 원래 위치에서 유닛 제거
                        if (originalTiles[originalPos] == unitTile)
                        {
                            tilemap.SetTile(originalPos, zeroTile);
                            Vector3Int position = originalPos;
                            int index = position.x + position.y * fieldMapTileScript.tileData.width;
                            int tileIndex2 = fieldMapTileScript.tileData.tilesInfo[index].tileIndex;
                            if (index >= 0 && index < fieldMapTileScript.tileData.tilesInfo.Count)
                                fieldMapTileScript.tileData.tilesInfo[index] = new TileInfo { tileIndex = tileIndex2, tileCode = 0 };
                        }
                        else
                        {
                            tilemap.SetTile(originalPos, originalTiles[originalPos]);
                            Vector3Int position = originalPos;
                            int index = position.x + position.y * fieldMapTileScript.tileData.width;
                            int tileIndex2 = fieldMapTileScript.tileData.tilesInfo[index].tileIndex;
                            if (index >= 0 && index < fieldMapTileScript.tileData.tilesInfo.Count)
                                fieldMapTileScript.tileData.tilesInfo[index] = originalInfos[originalPos];
                        }

                        // 대상 위치로 유닛 이동
                        tilemap.SetTile(targetPos, unitTile);
                        int targetIndex = targetPos.x + targetPos.y * fieldMapTileScript.tileData.width;
                        TileInfo targetInfo = GetTileInfoAtPosition(targetPos);
                        targetInfo.tileIndex = fieldMapTileScript.tileData.tilesInfo[targetIndex].tileIndex;
                        fieldMapTileScript.tileData.tilesInfo[targetIndex] = targetInfo;

                        nowUnit.u_tileIndex = GetTileInfoAtPosition(targetPos).tileIndex;
                        battleUnitList.u_list[n] = nowUnit;

                        List<string> foundInnerList = nowUnit.u_skill.FirstOrDefault(innerList => innerList[1] == "move");
                        ap = ap - int.Parse(foundInnerList[2]);
                        yield return new WaitForSeconds(0.5f);
                        follower.SetActive(false);
                    }
                }
            }
        }
        nowTurn++;
        battlePhase();
        hasCoroutineStarted = false;
    }
    public void rootingSys(List<ItemAmount> drone, List<ItemAmount> drops)
    {
        rootingUI.SetActive(true);
        itemDictA = new Dictionary<string, item_s>();
        itemDictB = new Dictionary<string, item_s2>();
        string[] lines = item_s_database.text.Substring(0, item_s_database.text.Length - 1).Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split('\t');
            string No_s = row[0];
            string type_c = row[1];
            string name = row[2];
            string amount_s = database_func_script_s.FindAmountByName(drone, name).ToString();
            string weight = row[3];
            string value = row[4];
            string explanation = row[5];
            string[] stats = new string[] { row[6], row[8], row[10], row[12], row[14] };
            string[] statValues = new string[] { row[7], row[9], row[11], row[13], row[15] };

            item_s item = new item_s(name, type_c, explanation, No_s, weight, value, amount_s, stats, statValues);
            if (database_func_script_s.FindAmountByName(drone, name) != 0) itemDictA.Add(name, item);
        }
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split('\t');
            string No_s = row[0];
            string type_c = row[1];
            string name = row[2];
            string amount_s = database_func_script_s.FindAmountByName(drops, name).ToString();
            string weight = row[3];
            string value = row[4];
            string explanation = row[5];
            string[] stats = new string[] { row[6], row[8], row[10], row[12], row[14] };
            string[] statValues = new string[] { row[7], row[9], row[11], row[13], row[15] };

            item_s2 item = new item_s2(name, type_c, explanation, No_s, weight, value, amount_s, stats, statValues);
            if (database_func_script_s.FindAmountByName(drops, name) != 0) itemDictB.Add(name, item);
        }

        ChangeButtonCountA(itemDictA.Count);
        FillButtonsListsWithDataA(itemDictA);
        ChangeButtonCountB(itemDictB.Count);
        FillButtonsListsWithDataB(itemDictB);
    }

    public void droneSkillUISet(BattleUnit unit)
    {
        APobj.SetActive(true);
        turnEnd.SetActive(true);
        APText.text = "AP: " + playerAP;
        unitSkillUI.SetActive(true);
        foreach (GameObject obj in skillBTN)
        {
            obj.SetActive(false);
        }
        for (int i = 0; i < unit.u_skill.Count; i++)
        {
            skillBTN[i].SetActive(true);
        }
        List<string> move = unit.u_skill.FirstOrDefault(innerList => innerList[1] == "move");
        List<List<string>> notMove = unit.u_skill.Where(innerList => innerList[1] != "move").ToList();

        skillWeapon[0].text = itemNameGet(move[0]);
        skillName[0].text = move[1];
        skillTurn[0].text = "AP: " + move[2];
        skillRange[0].text = "Range: " + move[3];
        for(int i = 1; i < unit.u_skill.Count; i++)
        {
            skillWeapon[i].text = itemNameGet(notMove[i - 1][0]);
            skillName[i].text = notMove[i - 1][1];
            skillTurn[i].text = "AP: " + notMove[i - 1][2];
            skillRange[i].text = "Range: " + notMove[i - 1][3];
        }
        reloadUI.SetActive(true);
        List<magazine> list = (from gearWeapon in unit.u_gear
                               join mag in unit.u_magazine on gearWeapon equals mag.weapon
                               select mag).ToList();
        for(int i = 0; i < list.Count; i++)
        {
            reloadText[i].text = list[i].weapon + ":" + list[i].ammunition + " [" + list[i].current + "/" + list[i].max + "]";
        }
        for(int i = list.Count; i < 4; i++)
        {
            reloadText[i].text = "";
        }
    }
    public void setSkillIndex(int a)
    {
        if(field != null)
            feildTileAstarscript.HighlightPath(field, Color.white);
        skillIndex = a;
        if (a == -1) return;
        int n = nowTurn;
        if (nowTurn >= battleUnitList.u_list.Count) n = nowTurn % battleUnitList.u_list.Count;
        BattleUnit nowUnit = battleUnitList.u_list[n];

        int r = int.Parse(nowUnit.u_skill[a][3]);
        skillString = nowUnit.u_skill[a][1];
        if (a == 0)
        {
            r = playerAP;
            skillString = "move";
            field = feildTileAstarscript.GetTilesWithinRange(GetPositionFromTileIndex(nowUnit.u_tileIndex), r);
            Vector3Int pos = GetPositionFromTileIndex(nowUnit.u_tileIndex);
            field = feildTileAstarscript.GetMovableTiles(field, pos, playerAP);
            feildTileAstarscript.HighlightPath(field, new Color(124, 252, 0));
            follower.SetActive(false);
        }
        else if(skillString == "reload")
        {
            reloadFuncOn(nowUnit);
            follower.SetActive(false);
        }
        else
        {
            field = feildTileAstarscript.GetTilesWithinRange(GetPositionFromTileIndex(nowUnit.u_tileIndex), r);
            feildTileAstarscript.HighlightPath(field, new Color(124, 252, 0));
            follower.SetActive(false);
        }
    }
    public void reloadFuncOn(BattleUnit unit)
    {
        reloadFuncUI.SetActive(true);
        List<magazine> list = (from gearWeapon in unit.u_gear
                               join mag in unit.u_magazine on gearWeapon equals mag.weapon
                               select mag).ToList();
        for (int i = 0; i < list.Count; i++)
        {
            int capturedIndex = i;
            reloadFuncBTN[i].SetActive(true);
            reloadFuncText[i].text = list[i].weapon + "\n" + list[i].ammunition + " [" + list[i].current + "/" + list[i].max + "]";
            string weapon = list[i].weapon;
            Button btnComponent = reloadFuncBTN[i].GetComponent<Button>();
            btnComponent.onClick.RemoveAllListeners(); // 기존 리스너 제거
            btnComponent.onClick.AddListener(() => reloadBTNFunc(unit, capturedIndex, weapon));
        }
        for (int i = list.Count; i < 4; i++)
        {
            reloadFuncBTN[i].SetActive(false);
        }
    }
    public void reloadBTNFunc(BattleUnit unit, int a, string weapon)
    {
        reloadWeapon = weapon;
        string gear = reloadFuncText[a].text;
        string type = "0";
        string pattern = @"\[(.*?)\]";
        Match match = Regex.Match(gear, pattern);
        if (match.Success)
        {
            type = match.Groups[1].Value;
        }
        List<ItemAmount> filteredItems = unit.u_itemAmount
            .Where(item => item.name.Contains(type) && item.amount >= 1)
            .ToList();
        fieldReload.SetActive(true);
        GenerateReloadButtons(filteredItems);
    }

    public void fireBattleCalculate(BattleUnit Aunit, string skillName2, BattleUnit Bunit)
    {
        List<string> s_skill = new List<string>();
        List<string> s_item = new List<string>();
        string weaponName = "";
        foreach (var skill in Aunit.u_skill)
        {
            if (skill[1] == skillName2)
            {
                s_skill = skill;
                weaponName = skill[10];
                weaponName = Regex.Replace(weaponName, @"[\u0000-\u001F]+", string.Empty);
                break;
            }
        }
        magazine foundMagazine = Aunit.u_magazine.FirstOrDefault(mag => mag.weapon == weaponName);
        if (foundMagazine != null)
        {
            string[] lines = item_s_database.text.Substring(0, item_s_database.text.Length - 1).Split('\n');
            for (int z = 0; z < lines.Length; z++)
            {
                string[] row = lines[z].Split('\t');
                string name = row[2];
                if (name == foundMagazine.ammunition)
                {
                    for (int i = 0; i < row.Length; i++)
                    {
                        s_item.Add(row[i]);
                    }
                    break;
                }
            }
        }

        if (s_skill[7] == "attack")
        {
            if (foundMagazine.current > 0)
            {
                foundMagazine.current -= int.Parse(s_skill[4]);
                int dmg = int.Parse(s_item[7]) + int.Parse(s_skill[8]);

                //최대 +-5% 랜덤화
                int randomFactor = Random.Range(-5, 6);
                int change = (dmg * randomFactor) / 100;
                dmg = dmg + change;

                if (int.Parse(s_skill[9]) >= Random.Range(1, 101))
                {
                    //40~60% 랜덤화
                    int randomFactor2 = Random.Range(40, 61);
                    int change2 = (dmg * randomFactor2) / 100;
                    dmg = dmg + change2;
                }
                Bunit.u_hp -= dmg;
            }
            else
            {
                return;
            }
        }
    }
    public void coldBattleCalculate(BattleUnit Aunit, List<string> skill, BattleUnit Bunit)
    {
        if (skill[7] == "attack")
        {
            int dmg = int.Parse(skill[8]);

            //최대 +-5% 랜덤화
            int randomFactor = Random.Range(-5, 6);
            int change = (dmg * randomFactor) / 100;
            dmg = dmg + change;
            if (int.Parse(skill[9]) >= Random.Range(1, 101))
            {
                //40~60% 랜덤화
                int randomFactor2 = Random.Range(40, 61);
                int change2 = (dmg * randomFactor2) / 100;
                dmg = dmg + change2;
            }
            Bunit.u_hp -= dmg;
        }
    }

    public void unitTextSet(BattleUnit unit)
    {
        unitUI.SetActive(true);
        unitText[0].text = unit.name;
        unitText[1].text = "Tile index: " + unit.u_tileIndex;
        unitText[2].text = "HP: " + unit.u_hp + "/" + unit.u_max_hp;
        unitText[3].text = "Armor: " + unit.u_armor;
        unitText[4].text = "Speed: " + unit.u_speed;
        unitText[5].text = "AP: " + unit.u_ap;
        unitText[6].text = "Weight: " + unit.u_weight + "/" + unit.u_max_weight;

        unitText2[0].text = "Artifact: " + unit.u_gear[6];
        unitText2[1].text = "Artifact: " + unit.u_gear[7];
        unitText2[2].text = "Artifact: " + unit.u_gear[8];

        unitText3[0].text = "Rhand: " + unit.u_gear[0];
        unitText3[1].text = "Lhand: " + unit.u_gear[1];
        unitText3[2].text = "Body: " + unit.u_gear[2];
        unitText3[3].text = "Head: " + unit.u_gear[3];
        unitText3[4].text = "Leg: " + unit.u_gear[4];
        unitText3[5].text = "Back: " + unit.u_gear[5];
        if (unit.u_gear[6] == "0") unitText2[0].text = "";
        if (unit.u_gear[7] == "0") unitText2[1].text = "";
        if (unit.u_gear[8] == "0") unitText2[2].text = "";

        if (unit.u_gear[0] == "0") unitText3[0].text = "";
        if (unit.u_gear[1] == "0") unitText3[1].text = "";
        if (unit.u_gear[2] == "0") unitText3[2].text = "";
        if (unit.u_gear[3] == "0") unitText3[3].text = "";
        if (unit.u_gear[4] == "0") unitText3[4].text = "";
        if (unit.u_gear[5] == "0") unitText3[5].text = "";

        unitItemGet(unit);
    }
    public void unitUIOut()
    {
        unitUI.SetActive(false);
        ClearButtons1();
    }

    private void battlePhase()
    {
        attackInfo.SetActive(false);
        reloadUI.SetActive(false);
        unitSkillUI.SetActive(false);
        APobj.SetActive(false);
        turnEnd.SetActive(false);
        if (battleUnitList.TeamIndices(2).Count <= 0)
        {
            winFunc();
            int n = nowTurn;
            if (nowTurn >= battleUnitList.u_list.Count) n = nowTurn % battleUnitList.u_list.Count;
            BattleUnit nowUnit = battleUnitList.u_list[n];
            if (nowUnit.u_team == 1)
            {
                currentMode = GameMode.PlayerTurn;
            }
            else if (nowUnit.u_team == 2)
            {
                currentMode = GameMode.EnemyTurn;
            }
        }
        else if(battleUnitList.TeamIndices(1).Count <= 0)
        {
            loseFunc();
            currentMode = GameMode.Lose;
        }
        else
        {
            int n = nowTurn;
            if (nowTurn >= battleUnitList.u_list.Count) n = nowTurn % battleUnitList.u_list.Count;
            BattleUnit nowUnit = battleUnitList.u_list[n];
            if (nowUnit.u_team == 1)
            {
                currentMode = GameMode.PlayerTurn;
            }
            else if (nowUnit.u_team == 2) {
                currentMode = GameMode.EnemyTurn;
            }
        }
        playerCheck = false;
        setSkillIndex(-1);
        skillString = "";
        textReset(battleUnitList);
    }
    public void loseFunc()
    {
        winLose.SetActive(true);
        winLoseText.text = "Try Again";
    }

    public void winFunc()
    {
        winLose.SetActive(true);
        winLoseText.text = "Return to Vehicle";
        Button btnComponent = winLose.GetComponent<Button>();
        btnComponent.onClick.RemoveAllListeners(); // 기존 리스너 제거
        btnComponent.onClick.AddListener(() => winBTNClicked());
    }
    public void winBTNClicked()
    {
        DataManager.Instance.LoadGameData0();
        foreach (var unit in battleUnitList.u_list)
        {
            if(unit.u_team == 1)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (DataManager.Instance.data0.drone[i].d_code == unit.u_code)
                    {
                        DataManager.Instance.data0.drone[i].d_hp = unit.u_hp;
                        DataManager.Instance.data0.drone[i].d_magazine = unit.u_magazine;
                        DataManager.Instance.data0.drone[i].d_itemAmount = unit.u_itemAmount;
                    }
                }
            }
        }
        DataManager.Instance.SaveGameData0();
        SceneManager.LoadScene("vehicle_scen");
    }

    private void allUnitCalculate()
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        int n = 0;
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    if(battleTileData.tilesInfo[n].tileCode >= 10 && battleTileData.tilesInfo[n].tileCode <= 14)
                    {
                        string name = battleTileData.tilesInfo[n].drone.d_name;
                        int index = battleTileData.tilesInfo[n].tileIndex;
                        int maxHp = battleTileData.tilesInfo[n].drone.d_max_hp;
                        int hp = battleTileData.tilesInfo[n].drone.d_hp;
                        int armor = battleTileData.tilesInfo[n].drone.d_armor;
                        int speed = battleTileData.tilesInfo[n].drone.d_speed;
                        int maxWeight = battleTileData.tilesInfo[n].drone.d_max_weight;
                        int weight = battleTileData.tilesInfo[n].drone.d_weight;
                        int ap = battleTileData.tilesInfo[n].drone.d_ap;
                        BattleUnit newUnit = new BattleUnit(1, index, name, maxHp, hp, armor, speed, maxWeight, weight, ap);
                        newUnit.u_code = battleTileData.tilesInfo[n].drone.d_code;
                        newUnit.u_gear = battleTileData.tilesInfo[n].drone.d_gear;
                        newUnit.u_itemAmount = battleTileData.tilesInfo[n].drone.d_itemAmount;
                        newUnit.u_skill = getSkillList(battleTileData.tilesInfo[n]);
                        newUnit.u_magazine = battleTileData.tilesInfo[n].drone.d_magazine;
                        battleUnitList.u_list.Add(newUnit);
                    }
                    if(battleTileData.tilesInfo[n].tileCode >= 20 && battleTileData.tilesInfo[n].tileCode <= 39)
                    {
                        string name = battleTileData.tilesInfo[n].enemy.name;
                        int index = battleTileData.tilesInfo[n].tileIndex;
                        int maxHp = battleTileData.tilesInfo[n].enemy.e_hp;
                        int hp = battleTileData.tilesInfo[n].enemy.e_hp;
                        int armor = battleTileData.tilesInfo[n].enemy.e_armor;
                        int speed = battleTileData.tilesInfo[n].enemy.e_speed;
                        int ap = battleTileData.tilesInfo[n].enemy.e_ap;
                        BattleUnit newUnit = new BattleUnit(2, index, name, maxHp, hp, armor, speed, 0, 0, ap);
                        newUnit.u_ID = battleTileData.tilesInfo[n].enemy.id;
                        newUnit.u_skill = getSkillList(battleTileData.tilesInfo[n]);
                        battleUnitList.u_list.Add(newUnit);
                    }
                    //Debug.Log(battleTileData.tilesInfo[n].tileIndex);
                    n++;
                }
            }
        }
    }

    public List<List<string>> getSkillList(TileInfo tileInfo)
    {
        List<List<string>> List = new List<List<string>>();
        if(tileInfo.tileCode >= 10 && tileInfo.tileCode <= 14)
        {
            int itemNo = 0;
            string droneName = tileInfo.drone.d_name;
            int droneNo = 0;
            foreach (string gear in tileInfo.drone.d_gear)
            {
                string[] lines = item_s_database.text.Substring(0, item_s_database.text.Length - 1).Split('\n');
                for (int z = 0; z < lines.Length; z++)
                {
                    string[] row = lines[z].Split('\t');
                    string name = row[2];
                    if(name == gear)
                    {
                        itemNo = int.Parse(row[0]);
                    }
                    if(name == droneName)
                    {
                        droneNo = int.Parse(row[0]);
                    }
                }
                string[] lines2 = skill_database.text.Substring(0, skill_database.text.Length - 1).Split('\n');
                for (int z = 0; z < lines2.Length; z++)
                {
                    string[] row = lines2[z].Split('\t');
                    if(itemNo == int.Parse(row[0]))
                    {
                        List.Add(new List<string>(row));
                    }
                    if(droneNo == int.Parse(row[0]))
                    {
                        List.Add(new List<string>(row));
                    }
                }
            }
            List = List.GroupBy(x => string.Join(",", x))
                   .Select(g => g.First())
                   .ToList();
            return List;
        }
        else if(tileInfo.tileCode >= 20 && tileInfo.tileCode <= 39)
        {
            int enemyID = tileInfo.enemy.id;
            string[] lines2 = skill_database.text.Substring(0, skill_database.text.Length - 1).Split('\n');
            for (int z = 0; z < lines2.Length; z++)
            {
                string[] row = lines2[z].Split('\t');
                if (enemyID == int.Parse(row[0]))
                {
                    List.Add(new List<string>(row));
                }
            }
            List = List.GroupBy(x => string.Join(",", x))
                   .Select(g => g.First())
                   .ToList();
            return List;
        }
        else
        {
            return null;
        }
    }

    public void droneDeployButtonClicked(int droneIndex)
    {
        if (currentMode == GameMode.ViewTileInfo)
        {
            ChangeGameMode(GameMode.PlaceDrones);
            deploy_img.SetActive(true);
            deploy_text.text = "Deploy: " + letters[droneIndex];
            currentlySelectedDroneIndex = droneIndex;
        }
        else if (currentMode == GameMode.PlaceDrones)
        {
            if (currentlySelectedDroneIndex == droneIndex)  // 이미 선택된 드론 버튼을 다시 클릭한 경우
            {
                ChangeGameMode(GameMode.ViewTileInfo);
                deploy_img.SetActive(false);
                currentlySelectedDroneIndex = null;
            }
            else  // 다른 드론 버튼을 클릭한 경우
            {
                deploy_text.text = "Deploy: " + letters[droneIndex];
                currentlySelectedDroneIndex = droneIndex;
            }
        }
    }

    private void getOrigin()
    {
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase originalTile = tilemap.GetTile(cellPosition);
                TileInfo originalInfo = GetTileInfoAtPosition(cellPosition);

                if (originalTile != null && !originalTiles.ContainsKey(cellPosition))
                {
                    originalTiles.Add(cellPosition, originalTile);
                    originalInfos.Add(cellPosition, originalInfo);
                }
            }
        }
    }

    private void PlaceDrone(Vector3Int cellPosition)
    {
        if (currentlySelectedDroneIndex == null) return;

        if (!deployedDrones.ContainsKey(cellPosition))
        {
            if (tilemap.GetTile(cellPosition) == deployTile)
            {
                Drone droneToDeploy = drones[(int)currentlySelectedDroneIndex];

                // 드론 타일 배치
                tilemap.SetTile(cellPosition, droneTile[(int)currentlySelectedDroneIndex]);
                droneDeploy[(int)currentlySelectedDroneIndex].SetActive(false);
                // 추후 참조를 위해 딕셔너리에 저장
                deployedDrones[cellPosition] = droneToDeploy;

                Debug.Log($"Drone {droneToDeploy.d_name} deployed at {cellPosition}");

                int index = cellPosition.x + cellPosition.y * fieldMapTileScript.tileData.width;

                TileInfo tileinfo = fieldMapTileScript.tileData.tilesInfo[index];
                tileinfo.tileCode = 10 + (int)currentlySelectedDroneIndex;
                tileinfo.drone = droneToDeploy;
                fieldMapTileScript.tileData.tilesInfo[index] = tileinfo;

                // 드론 배치 후 모드 전환 및 선택된 드론 초기화
                ChangeGameMode(GameMode.ViewTileInfo);
                currentlySelectedDroneIndex = null;
                deploy_img.SetActive(false);
            }
        }
        else
        {
            Debug.Log("There's already a drone deployed at this position.");
        }
    }

    private void RemoveDrone(Vector3Int cellPosition)
    {
        // 해당 위치에 드론이 배치되어 있는지 확인
        if (deployedDrones.ContainsKey(cellPosition))
        {
            Drone droneToRemove = deployedDrones[cellPosition];
            int droneIndex = drones.IndexOf(droneToRemove);

            // 타일맵에서 해당 위치의 타일을 교체
            TileBase originalTile = originalTiles[cellPosition];
            if (originalTile == deployTile)
                originalTile = zeroTile;
            tilemap.SetTile(cellPosition, originalTile);

            int index = cellPosition.x + cellPosition.y * fieldMapTileScript.tileData.width;

            TileInfo tileinfo = fieldMapTileScript.tileData.tilesInfo[index];
            tileinfo.tileIndex = fieldMapTileScript.tileData.tilesInfo[index].tileIndex;
            tileinfo.tileCode = 0;
            fieldMapTileScript.tileData.tilesInfo[index] = tileinfo;

            // 드론 정보를 deployedDrones 딕셔너리에서 제거
            deployedDrones.Remove(cellPosition);

            // UI 업데이트
            droneDeploy[droneIndex].SetActive(true);

            Debug.Log($"Drone {droneToRemove.d_name} removed from {cellPosition}");
        }
        else
        {
            Debug.Log("There's no drone deployed at this position.");
        }
    }


    public List<Drone> getDroneList()
    {
        DataManager.Instance.LoadGameData0();
        List<Drone> activeDrones = new List<Drone>();
        for (int i = 0; i < 5; i++)
        {
            if(DataManager.Instance.data0.drone[i].d_name != "")
            {
                droneDeploy[i].SetActive(true);
                droneName[i].text = DataManager.Instance.data0.drone[i].d_name;
                droneHP[i].text = "HP:" + DataManager.Instance.data0.drone[i].d_hp + "/" + DataManager.Instance.data0.drone[i].d_max_hp;
                droneWeight[i].text = "Weight:" + DataManager.Instance.data0.drone[i].d_weight + "/" + DataManager.Instance.data0.drone[i].d_max_weight;
                activeDrones.Add(DataManager.Instance.data0.drone[i]);
            }
            else
            {
                droneDeploy[i].SetActive(false);
            }
        }
        return activeDrones;
    }

    public void ChangeGameMode(GameMode newMode)
    {
        currentMode = newMode;
        if (newMode == GameMode.ViewTileInfo)
        {
            ReturnDeployTile();
            Debug.Log("Switched to Tile Viewing Mode.");
        }
        else if (newMode == GameMode.PlaceDrones)
        {
            DeployTile();
            Debug.Log("Switched to Drone Placement Mode.");
        }
    }

    public void startOfBattle()
    {
        currentMode = GameMode.BattleStart;
        deployDrones.SetActive(false);
        start.SetActive(false);
    }

    public void unitItemGet(BattleUnit unit)
    {
        itemDict1 = new Dictionary<string, item_s>();
        string[] lines = item_s_database.text.Substring(0, item_s_database.text.Length - 1).Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split('\t');
            string No_s = row[0];
            string type_c = row[1];
            string name = row[2];
            string amount_s = database_func_script_s.FindAmountByName(unit.u_itemAmount, name).ToString();
            string weight = row[3];
            string value = row[4];
            string explanation = row[5];
            string[] stats = new string[] { row[6], row[8], row[10], row[12], row[14] };
            string[] statValues = new string[] { row[7], row[9], row[11], row[13], row[15] };

            item_s item = new item_s(name, type_c, explanation, No_s, weight, value, amount_s, stats, statValues);
            if (database_func_script_s.FindAmountByName(unit.u_itemAmount, name) != 0) itemDict1.Add(name, item);
        }
        ChangeButtonCount1(itemDict1.Count);
        FillButtonsListsWithData1(itemDict1);
    }

    public string itemNameGet(string code)
    {
        string[] lines = item_s_database.text.Substring(0, item_s_database.text.Length - 1).Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split('\t');

            if (code == row[0])
            {
                return row[2];
            }
        }
        return "0";
    }
        private void ClearButtons1()
    {
        foreach (GameObject button in buttonsList3)
        {
            Destroy(button);
        }
        buttonsList3.Clear();

        foreach (GameObject button in buttonsList4)
        {
            Destroy(button);
        }
        buttonsList4.Clear();

        foreach (GameObject button in buttonsList5)
        {
            Destroy(button);
        }
        buttonsList5.Clear();
    }
    public void ChangeButtonCount1(int newButtonCount)
    {
        numberOfButtons1 = newButtonCount;
        GenerateButtons1();
    }
    private void GenerateButtons1()
    {
        RectTransform contentRect = buttonParent2.GetComponent<RectTransform>();
        contentRect.pivot = new Vector2(0, 1);

        // 버튼 높이를 가져와서 버튼 개수만큼 더한 높이를 계산
        int totalButtonHeight = numberOfButtons1 * 50;

        // 버튼 시작 Y 포지션 값
        int startYPosition = totalButtonHeight;

        // 지정한 개수만큼 버튼 생성
        for (int i = 0; i < numberOfButtons1; i++)
        {
            int buttonIndex = i;
            // 버튼 1 생성
            GameObject buttonInstance = Instantiate(buttonPrefab3, buttonParent2);
            buttonInstance.name = "Button " + i;
            buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(-120, startYPosition / 2 - i * 50);
            TMP_Text buttonText1 = buttonInstance.GetComponentInChildren<TMP_Text>();
            buttonsList3.Add(buttonText1.gameObject);
            buttonsList3.Add(buttonInstance);

            // 버튼 2 생성
            GameObject buttonInstance2 = Instantiate(buttonPrefab4, buttonParent2);
            buttonInstance2.name = "Button_n " + i;
            buttonInstance2.GetComponent<RectTransform>().anchoredPosition = new Vector2(300, startYPosition / 2 - i * 50);
            TMP_Text buttonText2 = buttonInstance2.GetComponentInChildren<TMP_Text>();
            buttonsList4.Add(buttonText2.gameObject);
            buttonsList4.Add(buttonInstance2);

            // 버튼 3 생성
            GameObject buttonInstance3 = Instantiate(buttonPrefab5, buttonParent2);
            buttonInstance3.name = "Button_w " + i;
            buttonInstance3.GetComponent<RectTransform>().anchoredPosition = new Vector2(420, startYPosition / 2 - i * 50);
            TMP_Text buttonText3 = buttonInstance3.GetComponentInChildren<TMP_Text>();
            buttonsList5.Add(buttonText3.gameObject);
            buttonsList5.Add(buttonInstance3);
        }

        // Content 영역의 크기를 조정
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalButtonHeight + 50);
    }
    private void FillButtonsListsWithData1(Dictionary<string, item_s> itemDict)
    {
        for (int i = 0; i < buttonsList3.Count; i++)
        {
            if (i < itemDict.Count)
            {
                var item = itemDict.ElementAt(i);
                string name = item.Key;
                item_s itemInfo = item.Value;

                i = i * 2;
                buttonsList3[i].GetComponentInChildren<TMP_Text>().text = name;
                buttonsList4[i].GetComponentInChildren<TMP_Text>().text = itemInfo.amount_s;
                buttonsList5[i].GetComponentInChildren<TMP_Text>().text = (int.Parse(itemInfo.weight_s) * int.Parse(itemInfo.amount_s)).ToString();
                i = i / 2;
            }
        }
    }

    private TileInfo GetTileInfoAtPosition(Vector3Int position)
    {
        int index = position.x + position.y * fieldMapTileScript.tileData.width;
        if (index >= 0 && index < fieldMapTileScript.tileData.tilesInfo.Count)
        {
            return fieldMapTileScript.tileData.tilesInfo[index];
        }
        return null;
    }
    private Vector3Int GetPositionFromTileIndex(int index)
    {
        int width = fieldMapTileScript.tileData.width; // 타일맵의 너비
        int x = index % width;
        int y = index / width;
        return new Vector3Int(x, y, 0);
    }
    private BattleUnit GetUnitFromIndex(int index)
    {
        for(int i = 0; i < battleUnitList.u_list.Count(); i++)
        {
            if (battleUnitList.u_list[i].u_tileIndex == index) return battleUnitList.u_list[i];
        }
        return null;
    }

    public void endTurn()
    {
        playerAP = 0;
        nowTurn++;
        battlePhase();
    }

    public void DeployTile()
    {
        for (int y = 0; y < fieldMapTileScript.tileData.height; y++) //가로 세로가 뒤바뀌어 있음
        {
            for (int x = 0; x < fieldMapTileScript.tileData.width; x++)
            {
                int index = x + y * fieldMapTileScript.tileData.width;
                // 왼쪽 밑 6x6 영역은 타일 0으로 고정
                if (x < 6 && y < 6)
                {
                    if(fieldMapTileScript.tileData.tilesInfo[index].tileCode == 0)
                        tilemap.SetTile(new Vector3Int(x, y, 0), deployTile);
                }
            }
        }
    }
    public void ReturnDeployTile()
    {
        for (int y = 0; y < fieldMapTileScript.tileData.height; y++) //가로 세로가 뒤바뀌어 있음
        {
            for (int x = 0; x < fieldMapTileScript.tileData.width; x++)
            {
                int index = x + y * fieldMapTileScript.tileData.width;
                // 왼쪽 밑 6x6 영역은 타일 0으로 고정
                if (x < 6 && y < 6)
                {
                    if (fieldMapTileScript.tileData.tilesInfo[index].tileCode == 0)
                        tilemap.SetTile(new Vector3Int(x, y, 0), zeroTile);
                }
            }
        }
    }

    private void ChangeTileColorUnderMouse()
    {
        // 마우스 포인터의 월드 좌표 가져오기
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        cellPosition.z = 0;

        // 이전에 마우스가 올라간 셀의 색상을 원래대로 복원
        if (prevCellPosition != cellPosition)
        {
            tilemap.SetColor(prevCellPosition, prevColor);
            prevColor = tilemap.GetColor(cellPosition);
            prevCellPosition = cellPosition;
        }

        // 현재 마우스가 올라간 셀의 색상 변경
        tilemap.SetTileFlags(cellPosition, TileFlags.None);
        tilemap.SetColor(cellPosition, Color.green);
    }
    private void GenerateReloadButtons(List<ItemAmount> items)
    {
        RectTransform contentRect = fieldReloadParent.GetComponent<RectTransform>();
        //contentRect.pivot = new Vector2(0, 1);

        // 버튼 시작 X 포지션 값
        int startXPosition = 5;
        int startYPosition = 450;

        // 지정한 개수만큼 버튼 생성
        for (int i = 0; i < items.Count(); i++)
        {
            if (i % 2 == 0) { startXPosition -= 330; startYPosition -= 200; }
            else { startXPosition += 330; }
            string name = items[i].name;
            GameObject buttonInstance = Instantiate(fieldReloadPrefab, fieldReloadParent);
            buttonInstance.name = "Button_R" + i;
            buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(startXPosition, startYPosition);
            buttonInstance.GetComponent<Button>().onClick.AddListener(() => ammunitionSelect(name));
            TMP_Text[] textComponents = buttonInstance.GetComponentsInChildren<TMP_Text>();
            textComponents[0].text = items[i].name;
            textComponents[1].text = "Amount: " + items[i].amount.ToString();
            fieldReloadList1.Add(buttonInstance);
        }
        // Content 영역의 크기를 조정
        //contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, contentRect.sizeDelta.y);
    }
    public void ammunitionSelect(string a_name)
    {
        int n = nowTurn;
        if (nowTurn >= battleUnitList.u_list.Count) n = nowTurn % battleUnitList.u_list.Count;
        BattleUnit nowUnit = battleUnitList.u_list[n];

        magazine magazine1 = nowUnit.u_magazine.FirstOrDefault(mag => mag.weapon == reloadWeapon);
        int amount = database_func_script_s.FindAmountByName(nowUnit.u_itemAmount, a_name);
        if (magazine1.current != 0)
        {
            if (magazine1.ammunition == a_name)
            {
                amount += magazine1.current;
            }
            else
            {
                int plus = database_func_script_s.FindAmountByName(nowUnit.u_itemAmount, magazine1.ammunition);
                plus += magazine1.current;
                database_func_script_s.ChangeAmountByName(nowUnit.u_itemAmount, magazine1.ammunition, plus);
            }
        }
        if (amount >= magazine1.max)
        {
            amount -= magazine1.max;
            magazine1.current = magazine1.max;
            magazine1.ammunition = a_name;
        }
        else
        {
            magazine1.current = amount;
            amount = 0;
            magazine1.ammunition = a_name;
        }
        database_func_script_s.ChangeAmountByName(nowUnit.u_itemAmount, magazine1.ammunition, amount);
        int minus = 1;
        foreach (var skill in nowUnit.u_skill)
        {
            if (skill[10] == reloadWeapon)
            {
                minus = int.Parse(skill[2]);
            }
        }
        playerAP -= minus;
        ClearButtons3();
        droneSkillUISet(nowUnit);
        reloadFuncUI.SetActive(false);
        fieldReload.SetActive(false);
    }
    private void ClearButtons3()
    {
        foreach (GameObject button in fieldReloadList1)
        {
            Destroy(button);
        }
        fieldReloadList1.Clear();
    }
    public void reloadExit()
    {
        reloadFuncUI.SetActive(false);
        ClearButtons3();
    }
    public void ChangeButtonCountA(int newButtonCount)
    {
        numberOfButtonsA = newButtonCount;
        GenerateButtonsA();
    }
    public void ChangeButtonCountB(int newButtonCount)
    {
        numberOfButtonsB = newButtonCount;
        GenerateButtonsB();
    }
    private void GenerateButtonsA()
    {
        RectTransform contentRect = buttonParentA.GetComponent<RectTransform>();
        //contentRect.pivot = new Vector2(0, 1);

        // 버튼 높이를 가져와서 버튼 개수만큼 더한 높이를 계산
        int totalButtonHeight = numberOfButtonsA * 50;

        // 버튼 시작 Y 포지션 값
        int startYPosition = totalButtonHeight;

        // 지정한 개수만큼 버튼 생성
        for (int i = 0; i < numberOfButtonsA; i++)
        {
            int buttonIndex = i;
            // 버튼 1 생성
            GameObject buttonInstance = Instantiate(buttonPrefab6, buttonParentA);
            buttonInstance.name = "Button " + i;
            buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(-120, startYPosition / 2 - i * 50);
            TMP_Text buttonText1 = buttonInstance.GetComponentInChildren<TMP_Text>();
            buttonsListA1.Add(buttonText1.gameObject);
            buttonsListA1.Add(buttonInstance);

            // 버튼 2 생성
            GameObject buttonInstance2 = Instantiate(buttonPrefab4, buttonParentA);
            buttonInstance2.name = "Button_n " + i;
            buttonInstance2.GetComponent<RectTransform>().anchoredPosition = new Vector2(210, startYPosition / 2 - i * 50);
            TMP_Text buttonText2 = buttonInstance2.GetComponentInChildren<TMP_Text>();
            buttonsListA2.Add(buttonText2.gameObject);
            buttonsListA2.Add(buttonInstance2);

            // 버튼 3 생성
            GameObject buttonInstance3 = Instantiate(buttonPrefab5, buttonParentA);
            buttonInstance3.name = "Button_w " + i;
            buttonInstance3.GetComponent<RectTransform>().anchoredPosition = new Vector2(330, startYPosition / 2 - i * 50);
            TMP_Text buttonText3 = buttonInstance3.GetComponentInChildren<TMP_Text>();
            buttonsListA3.Add(buttonText3.gameObject);
            buttonsListA3.Add(buttonInstance3);
        }

        // Content 영역의 크기를 조정
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalButtonHeight + 50);
    }
    private void GenerateButtonsB()
    {
        RectTransform contentRect = buttonParentB.GetComponent<RectTransform>();
        //contentRect.pivot = new Vector2(0, 1);

        // 버튼 높이를 가져와서 버튼 개수만큼 더한 높이를 계산
        int totalButtonHeight = numberOfButtonsB * 50;

        // 버튼 시작 Y 포지션 값
        int startYPosition = totalButtonHeight;

        // 지정한 개수만큼 버튼 생성
        for (int i = 0; i < numberOfButtonsB; i++)
        {
            int buttonIndex = i;
            // 버튼 1 생성
            GameObject buttonInstance = Instantiate(buttonPrefab6, buttonParentB);
            buttonInstance.name = "Button " + i;
            buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(-120, startYPosition / 2 - i * 50);
            buttonInstance.GetComponent<Button>().onClick.AddListener(() => ButtonClicked2(buttonIndex));
            TMP_Text buttonText1 = buttonInstance.GetComponentInChildren<TMP_Text>();
            buttonsListB1.Add(buttonText1.gameObject);
            buttonsListB1.Add(buttonInstance);

            // 버튼 2 생성
            GameObject buttonInstance2 = Instantiate(buttonPrefab4, buttonParentB);
            buttonInstance2.name = "Button_n " + i;
            buttonInstance2.GetComponent<RectTransform>().anchoredPosition = new Vector2(210, startYPosition / 2 - i * 50);
            TMP_Text buttonText2 = buttonInstance2.GetComponentInChildren<TMP_Text>();
            buttonsListB2.Add(buttonText2.gameObject);
            buttonsListB2.Add(buttonInstance2);

            // 버튼 3 생성
            GameObject buttonInstance3 = Instantiate(buttonPrefab5, buttonParentB);
            buttonInstance3.name = "Button_w " + i;
            buttonInstance3.GetComponent<RectTransform>().anchoredPosition = new Vector2(330, startYPosition / 2 - i * 50);
            TMP_Text buttonText3 = buttonInstance3.GetComponentInChildren<TMP_Text>();
            buttonsListB3.Add(buttonText3.gameObject);
            buttonsListB3.Add(buttonInstance3);
        }

        // Content 영역의 크기를 조정
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalButtonHeight + 50);
    }
    private void FillButtonsListsWithDataA(Dictionary<string, item_s> itemDict)
    {
        for (int i = 0; i < buttonsListA1.Count; i++)
        {
            if (i < itemDict.Count)
            {
                var item = itemDict.ElementAt(i);
                string name = item.Key;
                item_s itemInfo = item.Value;

                i = i * 2;
                buttonsListA1[i].GetComponentInChildren<TMP_Text>().text = name;
                buttonsListA2[i].GetComponentInChildren<TMP_Text>().text = itemInfo.amount_s;
                buttonsListA3[i].GetComponentInChildren<TMP_Text>().text = (int.Parse(itemInfo.weight_s) * int.Parse(itemInfo.amount_s)).ToString();
                i = i / 2;
            }
        }
    }
    private void FillButtonsListsWithDataB(Dictionary<string, item_s2> itemDict)
    {
        for (int i = 0; i < buttonsListB1.Count; i++)
        {
            if (i < itemDict.Count)
            {
                var item = itemDict.ElementAt(i);
                string name = item.Key;
                item_s2 itemInfo = item.Value;

                i = i * 2;
                buttonsListB1[i].GetComponentInChildren<TMP_Text>().text = name;
                buttonsListB2[i].GetComponentInChildren<TMP_Text>().text = itemInfo.amount_s;
                buttonsListB3[i].GetComponentInChildren<TMP_Text>().text = (int.Parse(itemInfo.weight_s) * int.Parse(itemInfo.amount_s)).ToString();
                i = i / 2;
            }
        }
    }

    private void ButtonClicked2(int buttonIndex)
    {
        TileInfo t_info = GetTileInfoAtPosition(rootPosition);
        int n = nowTurn;
        if (nowTurn >= battleUnitList.u_list.Count) n = nowTurn % battleUnitList.u_list.Count;
        BattleUnit nowUnit = battleUnitList.u_list[n];

        buttonIndex = buttonIndex * 2;
        moveText2.text = buttonsListB1[buttonIndex].GetComponentInChildren<TMP_Text>().text;
        int x = database_func_script_s.FindAmountByName(nowUnit.u_itemAmount, moveText2.text);
        database_func_script_s.ChangeAmountByName(nowUnit.u_itemAmount, moveText2.text, x + 1);
        int y = database_func_script_s.FindAmountByName(t_info.drops, moveText2.text);
        database_func_script_s.ChangeAmountByName(t_info.drops, moveText2.text, y - 1);
        listUpdate();
    }
    public void listUpdate()
    {
        TileInfo t_info = GetTileInfoAtPosition(rootPosition);
        int n = nowTurn;
        if (nowTurn >= battleUnitList.u_list.Count) n = nowTurn % battleUnitList.u_list.Count;
        BattleUnit nowUnit = battleUnitList.u_list[n];
        ClearButtons();
        rootingSys(nowUnit.u_itemAmount, t_info.drops);
    }
    public void rootingOut()
    {
        rootingUI.SetActive(false);
        ClearButtons();
    }
    private void ClearButtons()
    {
        foreach (GameObject button in buttonsListA1)
        {
            Destroy(button);
        }
        buttonsListA1.Clear();

        foreach (GameObject button in buttonsListA2)
        {
            Destroy(button);
        }
        buttonsListA2.Clear();

        foreach (GameObject button in buttonsListA3)
        {
            Destroy(button);
        }
        buttonsListA3.Clear();
        foreach (GameObject button in buttonsListB1)
        {
            Destroy(button);
        }
        buttonsListB1.Clear();

        foreach (GameObject button in buttonsListB2)
        {
            Destroy(button);
        }
        buttonsListB2.Clear();

        foreach (GameObject button in buttonsListB3)
        {
            Destroy(button);
        }
        buttonsListB3.Clear();
    }
    private void GenerateButtons(BattleUnitList g_Units)
    {
        scrollView1.SetActive(true);
        RectTransform contentRect = buttonParent1.GetComponent<RectTransform>();
        contentRect.pivot = new Vector2(0, 1);
        g_Units.SortBySpeed();
        int numberOfButtons = g_Units.u_list.Count;
        int n = 1;
        // 버튼 너비를 가져와서 버튼 개수만큼 더한 너비를 계산
        int totalButtonWeight = numberOfButtons * 305;

        // 지정한 개수만큼 버튼 생성
        for (int i = 0; i < numberOfButtons; i++)
        {
            int buttonIndex = i;
            // 버튼 1 생성
            GameObject buttonInstance = Instantiate(buttonPrefab1, buttonParent1);
            buttonInstance.name = "Button " + i;
            buttonInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(-800 + i * 305, 25);
            TMP_Text[] allTexts = buttonInstance.GetComponentsInChildren<TMP_Text>();
            RectTransform textRect1 = allTexts[0].GetComponent<RectTransform>();
            textRect1.anchoredPosition = new Vector2(0, 50);
            RectTransform textRect2 = allTexts[1].GetComponent<RectTransform>();
            textRect2.anchoredPosition = new Vector2(0, 0);
            RectTransform textRect3 = allTexts[2].GetComponent<RectTransform>();
            textRect3.anchoredPosition = new Vector2(0, -50);
            RectTransform textRect4 = allTexts[3].GetComponent<RectTransform>();
            textRect4.anchoredPosition = new Vector2(0, -100);
            allTexts[0].text = g_Units.u_list[i].name;
            allTexts[1].text = "Speed: " + g_Units.u_list[i].u_speed;
            allTexts[2].text = "Index: " + g_Units.u_list[i].u_tileIndex;
            allTexts[3].text = "[Turn: " + n + "]";
            n++;
            buttonsList1.Add(buttonInstance);
        }

        // Content 영역의 크기를 조정
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, contentRect.sizeDelta.y);
    }
    private void textReset(BattleUnitList g_Units)
    {
        for (int i = 0; i < buttonsList1.Count; i++)
        {
            TMP_Text[] allTexts = buttonsList1[i].GetComponentsInChildren<TMP_Text>();
            if (allTexts.Length >= 4)
            {
                allTexts[0].text = g_Units.u_list[i].name;
                allTexts[1].text = "Speed: " + g_Units.u_list[i].u_speed;
                allTexts[2].text = "Index: " + g_Units.u_list[i].u_tileIndex;
                allTexts[3].text = "[Turn: " + (i + 1) + "]";
            }
        }
    }

    private bool IsPointerOverUIObject()
    {
        // PointerEventData의 새 인스턴스 생성
        UnityEngine.EventSystems.PointerEventData eventDataCurrentPosition = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);

        // 현재 마우스 위치를 eventData에 설정
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        // 레이캐스팅의 결과를 저장할 리스트 생성
        List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();

        // 현재 마우스 위치에서 레이캐스팅을 실행
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        // 결과 리스트에서 UI 오브젝트를 감지하면 true 반환
        return results.Count > 0;
    }
}
