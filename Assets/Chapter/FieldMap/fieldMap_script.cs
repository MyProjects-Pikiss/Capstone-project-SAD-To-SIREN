using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.IO;

public class fieldMap_script : MonoBehaviour
{
    public string saveFilePath;
    [SerializeField]
    private TextAsset worldMap_database;
    private fieldMapTile_script fieldMapTileScript;
    public Transform nodeParent;
    public TextAsset item_s_database;

    private void Awake()
    {
        // fieldMapTile_script 스크립트를 붙인 GameObject의 인스턴스를 가져옴
        fieldMapTileScript = FindObjectOfType<fieldMapTile_script>();
    }

    private void Start()
    {
        DataManager.Instance.LoadGameData0();
        saveFilePath = "savedWorldMap" + DataManager.Instance.data0.dataNumber + ".json";
        callGenerateRandomTileMap();

        float scaleFactor = 70f; // 크기를 키우는 배율
        nodeParent.localScale = new Vector3(scaleFactor, scaleFactor, 1.0f);
    }

    public void callGenerateRandomTileMap()
    {
        WorldLayer layer = new WorldLayer();
        string fullPath = Application.persistentDataPath + "/" + saveFilePath;
        string jsonData = File.ReadAllText(fullPath);
        layer = JsonUtility.FromJson<WorldLayer>(jsonData);

        foreach (var conNode in layer.nodes)
        {
            int loot = 0;
            int radio = 0;
            int info = 0;
            int rate = 0;

            if (conNode.userExist == 1)
            {
                string fieldMap_code = DataManager.Instance.data0.dataNumber + " " + conNode.nodeCode;
                int fieldSize = (18 - conNode.layerN) * 2;

                string[] lines = worldMap_database.text.Substring(0, worldMap_database.text.Length - 1).Split('\n');
                for (int x = 0; x < lines.Length; x++)
                {
                    string[] row = lines[x].Split('\t');
                    string layerN_d = row[1];
                    string nodeInfo_d = row[2];
                    string nodeName_d = row[3];
                    string nodeEnemy1_d = row[4];
                    string nodeEnemy2_d = row[5];
                    string nodeSpawn_d = row[6];
                    string nodeNum1_d = row[7];
                    string nodeNum2_d = row[8];
                    string nodeNum3_d = row[9];
                    string nodeNum4_d = row[10];
                    string nodeRadio_d = row[11];

                    if (conNode.nodeInfo == int.Parse(nodeInfo_d))
                    {
                        loot = 12 - conNode.layerN + Random.Range(-1, 2);
                        radio = int.Parse(nodeRadio_d) * Random.Range(2, 3) + Random.Range(0, 5);
                        info = int.Parse(nodeInfo_d);
                        rate = database_func_script_s.CalculationEnemySpawn(item_s_database, DataManager.Instance.data0.itemAmount, DataManager.Instance.data0.drone, int.Parse(nodeSpawn_d));
                    }
                }
                fieldMapTileScript.GenerateRandomTileMap(info, fieldMap_code, new int[] {loot, radio}, rate, fieldSize*2, fieldSize);
            }
        }
    }
}
