using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public class FieldTileData
{
    public int width;
    public int height;
    public int nodeInfo;
    public List<TileInfo> tilesInfo;
}
[System.Serializable]
public class TileInfo
{
    public int tileIndex;
    public int tileCode;
    public Enemy enemy = new Enemy();
    public Drone drone = new Drone();
    public List<ItemAmount> drops = new List<ItemAmount>();
}
[System.Serializable]
public class Enemy
{
    public string name = "";
    public string e_type = "";
    public int e_hp = 0;
    public int e_armor = 0;
    public int e_speed = 0;
    public int e_ap = 0;
    public int id = 0;

    public Enemy() { }
    public Enemy(string _name, string _e_type, int _id, int _e_hp, int _e_armor, int _e_speed, int _e_ap)
    {
        name = _name; id = _id; e_type = _e_type; e_hp = _e_hp; e_armor = _e_armor; e_speed = _e_speed; e_ap = _e_ap;
    }
}

public class fieldMapTile_script : MonoBehaviour
{
    public TileBase[] fieldTiles;
    public Tilemap fieldTilemap;
    public FieldTileData tileData;

    public FieldTileData TileData
    {
        get { return tileData; }
    }

    public TextAsset enemy_database;
    public TextAsset worldMap_database;
    public TextAsset item_s_database;

    private string GetSavePath(string fieldmapCode)
    {
        return Application.persistentDataPath + "/" + "field_" + fieldmapCode + ".json";
    }

    public void GenerateRandomTileMap(int nodeinfo, string fieldmapCode, int[] countTile, int rate, int height, int width)
    {
        string fieldMapSavePath = GetSavePath(fieldmapCode);

        if (File.Exists(fieldMapSavePath))
        {
            LoadTileData(fieldmapCode);
            return;
        }

        tileData = new FieldTileData();
        tileData.width = width;
        tileData.height = height;
        tileData.nodeInfo = nodeinfo;

        tileData.tilesInfo = new List<TileInfo>(width * height);

        int fixedTileIndex = 0; // 타일 0은 이 인덱스에서 고정됩니다.

        int countTile0 = tileData.width * tileData.height - countTile[0] - countTile[1] - 36 - 150;

        List<int> availableIndices = new List<int>();
        List<int> availableIndices2 = new List<int>();

        // 원하는 개수만큼의 타일 인덱스를 리스트에 추가합니다.
        for (int i = 0; i < countTile0; i++)
        {
            availableIndices.Add(0);
        }
        for (int i = 0; i < countTile[0]; i++)
        {
            availableIndices.Add(1);
        }
        for (int i = 0; i < countTile[1]; i++)
        {
            availableIndices.Add(2);
        }

        List<int> enemyList = new List<int>();
        enemyList = database_func_script_s.CalculationListEnemy(enemy_database, worldMap_database, rate, nodeinfo);

        int rateTile = enemyList.Count;
        int countTile2 = 150 - rateTile;
        for (int i = 0; i < countTile2; i++)
        {
            availableIndices2.Add(0);
        }
        for (int i = 0; i < rateTile; i++)
        {
            availableIndices2.Add(3);
        }

        // 리스트를 랜덤하게 섞습니다.
        for (int i = 0; i < availableIndices.Count; i++)
        {
            int temp = availableIndices[i];
            int randomIndex = Random.Range(i, availableIndices.Count);
            availableIndices[i] = availableIndices[randomIndex];
            availableIndices[randomIndex] = temp;
        }
        // 리스트를 랜덤하게 섞습니다.
        for (int i = 0; i < availableIndices2.Count; i++)
        {
            int temp = availableIndices2[i];
            int randomIndex = Random.Range(i, availableIndices2.Count);
            availableIndices2[i] = availableIndices2[randomIndex];
            availableIndices2[randomIndex] = temp;
        }
        // 리스트를 랜덤하게 섞습니다.
        for (int i = 0; i < enemyList.Count; i++)
        {
            int temp = enemyList[i];
            int randomIndex = Random.Range(i, enemyList.Count);
            enemyList[i] = enemyList[randomIndex];
            enemyList[randomIndex] = temp;
        }

        int currentIndex = 0;
        int enemyIndex = -1;
        int count1 = 0;
        int count2 = 0;

        for (int y = 0; y < tileData.height; y++) //가로 세로가 뒤바뀌어 있음
        {
            for (int x = 0; x < tileData.width; x++)
            {
                int index = x + y * tileData.width;
                int e_code = 20;
                // 왼쪽 밑 6x6 영역은 타일 0으로 고정
                if (x < 6 && y < 6)
                {
                    tileData.tilesInfo.Add(new TileInfo { tileIndex = currentIndex, tileCode = fixedTileIndex });
                    fieldTilemap.SetTile(new Vector3Int(x, y, 0), fieldTiles[fixedTileIndex]);
                    currentIndex++;
                }
                // 오른쪽 위 15x10 영역은 적 배치 타일
                else if (x >= tileData.width - 10 && y >= tileData.height - 15)
                {
                    int randomTileIndex = availableIndices2[count1];
                    if (randomTileIndex == 3)
                    {
                        if (enemyIndex < enemyList.Count-1)
                        {
                            enemyIndex++;
                        }
                        int find = enemyList[enemyIndex];
                        string[] lines = enemy_database.text.Substring(0, enemy_database.text.Length - 1).Split('\n');
                        for (int z = 0; z < lines.Length; z++)
                        {
                            string[] row = lines[z].Split('\t');
                            int no = int.Parse(row[0]);
                            int p_level = int.Parse(row[3]);
                            int hp = int.Parse(row[4]);
                            int armor = int.Parse(row[5]);
                            int speed = int.Parse(row[6]);
                            int ap = int.Parse(row[7]);
                            if (find == no)
                            {
                                Enemy enemy2 = new Enemy(row[2], row[1], no, hp, armor, speed, ap);
                                tileData.tilesInfo.Add(new TileInfo { tileIndex = currentIndex, tileCode = e_code, enemy = enemy2 });
                                e_code++;
                            }
                        }
                    }
                    else
                        tileData.tilesInfo.Add(new TileInfo { tileIndex = currentIndex, tileCode = randomTileIndex });
                    fieldTilemap.SetTile(new Vector3Int(x, y, 0), fieldTiles[randomTileIndex]);
                    currentIndex++;
                    count1++;
                }
                else
                {
                    //이후부터는 랜덤하게 배치합니다.
                    int randomTileIndex = availableIndices[count2];
                    tileData.tilesInfo.Add(new TileInfo { tileIndex = currentIndex, tileCode = randomTileIndex });
                    fieldTilemap.SetTile(new Vector3Int(x, y, 0), fieldTiles[randomTileIndex]);
                    currentIndex++;
                    count2++;
                }
            }
        }
        tileSpecialization(nodeinfo);
        SaveTileData(fieldmapCode, tileData);
    }

    public void tileSpecialization(int nodeinfo)
    {
        foreach (TileInfo tile in tileData.tilesInfo)
        {
            int tileCode = tile.tileCode;
            if(tileCode == 1) //아이템칸
            {
                List<string> itemList = new List<string>();
                itemList = database_func_script_s.CalculationListItems(item_s_database, worldMap_database, nodeinfo);

                List<ItemAmount> itemAmountList = new List<ItemAmount>();

                foreach (string itemName in itemList)
                {
                    ItemAmount existingItem = itemAmountList.Find(x => x.name == itemName);
                    if (existingItem != null)
                    {
                        existingItem.amount++;
                    }
                    else
                    {
                        ItemAmount newItem = new ItemAmount { name = itemName, amount = 1 };
                        itemAmountList.Add(newItem);
                    }
                }
                tile.drops = itemAmountList;
            }
        }
    }

    private void SaveTileData(string fieldmapCode, FieldTileData tileData)
    {
        string fieldMapSavePath = GetSavePath(fieldmapCode);
        string json = JsonUtility.ToJson(tileData);
        File.WriteAllText(fieldMapSavePath, json);
    }

    public void LoadTileData(string fieldmapCode)
    {
        string fieldMapSavePath = GetSavePath(fieldmapCode);

        if (File.Exists(fieldMapSavePath))
        {
            string json = File.ReadAllText(fieldMapSavePath);
            tileData = JsonUtility.FromJson<FieldTileData>(json);

            for (int y = 0; y < tileData.height; y++)
            {
                for (int x = 0; x < tileData.width; x++)
                {
                    int index = x + y * tileData.width;
                    int tileIndex = tileData.tilesInfo[index].tileIndex;
                    int tileCode = tileData.tilesInfo[index].tileCode;
                    if(tileCode >= 20 && tileCode <= 39) tileCode = 3;
                    fieldTilemap.SetTile(new Vector3Int(x, y, 0), fieldTiles[tileCode]);
                }
            }
        }
        else
        {
            Debug.Log("File not found: " + fieldMapSavePath);
        }
    }
    public FieldTileData GetTileData()
    {
        return tileData;
    }
}
