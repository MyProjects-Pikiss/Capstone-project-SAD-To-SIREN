using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using Random = UnityEngine.Random;

[Serializable]
public class WorldLayer
{
    public int maxLayers;
    public int worldTurn;
    public List<WorldNode> nodes = new List<WorldNode>(); // 여러 개의 WorldNode를 담는 리스트
    public List<NodeConnectionData> connections = new List<NodeConnectionData>();
}
[Serializable]
public class WorldNode
{
    public int nodeCode;
    public int nodeInfo;
    public int layerN;
    public int nodePosition;
    public int userExist;
    public int nodeTurn;
    public Vector2 position;

    public WorldNode(int code, int info, int layer, int position, int exist, int turn)
    {
        nodeCode = code;
        nodeInfo = info;
        layerN = layer;
        nodePosition = position;
        userExist = exist;
        nodeTurn = turn;
    }
}
[Serializable]
public class NodeConnectionData
{
    public int node1Code;
    public int node2Code;

    public NodeConnectionData(int node1, int node2)
    {
        node1Code = node1;
        node2Code = node2;
    }
}

public class NodeConnection
{
    public WorldNode node1;
    public WorldNode node2;

    public NodeConnection(WorldNode n1, WorldNode n2)
    {
        node1 = n1;
        node2 = n2;
    }
}

public class NodeClickHandler : MonoBehaviour
{
    private WorldNode node;
    private worldMap_script worldMap;

    public void Init(WorldNode node, worldMap_script worldMap)
    {
        this.node = node;
        this.worldMap = worldMap;
    }

    private void OnMouseDown()
    {
        if (worldMap != null)
        {
            worldMap.DisplayNodeCode(node);
        }
    }
}


public class worldMap_script : MonoBehaviour
{
    public WorldLayer layer = new WorldLayer();
    public Image[] worldNode_icons = new Image[4];
    public Transform nodeParent;

    private List<NodeConnection> connections = new List<NodeConnection>();
    public Sprite lineSprite;

    private HashSet<WorldNode> visitedNodes = new HashSet<WorldNode>();
    public string saveFilePath;

    [SerializeField]
    private TextAsset worldMap_database;
    public GameObject worldTile_info;
    public TMP_Text nodeName_text;
    public TMP_Text nodeCode_text;
    public TMP_Text nodeEnemy_text;
    public TMP_Text nodeSpawn_text;
    public TMP_Text nodeNum1_text;
    public TMP_Text nodeNum2_text;
    public TMP_Text nodeNum3_text;
    public TMP_Text nodeNum4_text;
    public TMP_Text nodeRadio_text;

    public GameObject travel_btn;
    public int travel_code;
    public GameObject toVehicle_btn;

    private void Start()
    {
        DataManager.Instance.LoadGameData0();
        toVehicle_btn.SetActive(true);
        saveFilePath = "savedWorldMap" + DataManager.Instance.data0.dataNumber + ".json";
        if (LoadMapData()) // 저장된 데이터가 있으면 불러오기
        {
            DrawAllNodesAndConnections();
        }
        else
        {
            WorldMapGenerate();
            SaveMapData();
        }

        float scaleFactor = 70f; // 크기를 키우는 배율
        nodeParent.localScale = new Vector3(scaleFactor, scaleFactor, 1.0f);
    }

    public void WorldMapGenerate()
    {
        layer.maxLayers = 10;
        layer.worldTurn = 0;

        GenerateNodes();
        DrawConnections();
        CheckNodeReachabilityAndConnect();
    }

    public void travel_clicked()
    {
        if(travel_code == -1)
        {
            Debug.Log("travel_code not found");
            return;
        }
        string fullPath = Application.persistentDataPath + "/" + saveFilePath;
        string jsonData = File.ReadAllText(fullPath);
        layer = JsonUtility.FromJson<WorldLayer>(jsonData);
        foreach (var conNode in layer.nodes)
        {
            conNode.userExist = 0;
            if(conNode.nodeCode == travel_code)
                conNode.userExist = 1;
        }
        layer.worldTurn += 1;
        string jsonData2 = JsonUtility.ToJson(layer, true);
        File.WriteAllText(Application.persistentDataPath + "/" + saveFilePath, jsonData2);
        Debug.Log("Map Data Saved to " + Application.persistentDataPath + "/" + saveFilePath);
        SceneManager.LoadScene("FieldMap_scen");
    }

    public void DisplayNodeCode(WorldNode node)
    {
        worldTile_info.SetActive(true);
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

            if (node.nodeInfo == int.Parse(nodeInfo_d))
            {
                nodeCode_text.text = "Node:" + node.nodeCode.ToString();
                nodeName_text.text = "[" + nodeName_d + "]";
                if (nodeEnemy2_d == "0") nodeEnemy_text.text = "Enemy type\n" + "[" + nodeEnemy1_d + "]";
                if (nodeEnemy2_d != "0") nodeEnemy_text.text = "Enemy type\n" + "[" + nodeEnemy1_d + "/" + nodeEnemy2_d + "]";
                nodeSpawn_text.text = "Rate: " + nodeSpawn_d;
                nodeNum1_text.text = nodeNum1_d;
                nodeNum2_text.text = nodeNum2_d;
                nodeNum3_text.text = nodeNum3_d;
                nodeNum4_text.text = nodeNum4_d;
                nodeRadio_text.text = "Radioactivity\nLevel: " + nodeRadio_d;
            }
        }
        if (GetConnectedNodes(node.nodeCode))
        {
            travel_code = node.nodeCode;
            travel_btn.SetActive(true);
        }
        else
        {
            travel_code = -1;
            travel_btn.SetActive(false);
        }
    }

    public bool GetConnectedNodes(int clickedNodeCode)
    {
        List<WorldNode> connectedNodes = new List<WorldNode>();

        string fullPath = Application.persistentDataPath + "/" + saveFilePath;
        string jsonData = File.ReadAllText(fullPath);
        layer = JsonUtility.FromJson<WorldLayer>(jsonData);

        foreach (var connData in layer.connections)
        {
            if (connData.node1Code == clickedNodeCode)
            {
                WorldNode connectedNode = layer.nodes.Find(n => n.nodeCode == connData.node2Code);
                if (connectedNode != null)
                {
                    connectedNodes.Add(connectedNode);
                }
            }
            else if (connData.node2Code == clickedNodeCode)
            {
                WorldNode connectedNode = layer.nodes.Find(n => n.nodeCode == connData.node1Code);
                if (connectedNode != null)
                {
                    connectedNodes.Add(connectedNode);
                }
            }
        }
        bool hasUserExistNode = false;
        foreach (var node in connectedNodes)
        {
            if (node.userExist == 1)
            {
                hasUserExistNode = true;
                break;
            }
        }
        return hasUserExistNode;
    }

    private void GenerateNodes()
    {
        int numNodes = 0;
        int code = 0;

        string[] lines = worldMap_database.text.Substring(0, worldMap_database.text.Length - 1).Split('\n');

        for (int i = 0; i < layer.maxLayers; i++)
        {
            List<int> nodeInfo_list = new List<int>();
            if (i == 0)
            {
                numNodes = 1;
            }
            else if (i == 1)
            {
                numNodes = Random.Range(4, 7);
            }
            else if (i == 2)
            {
                numNodes = Random.Range(9, 12);
            }
            else if (i == 3)
            {
                numNodes = Random.Range(12, 18);
            }
            else if (i == 4)
            {
                numNodes = Random.Range(18, 27);
            }
            else if (i == 5)
            {
                numNodes = Random.Range(25, 33);
            }
            else if (i == 6)
            {
                numNodes = Random.Range(32, 39);
            }
            else if (i == 7)
            {
                numNodes = Random.Range(41, 44);
            }
            else if (i == 8)
            {
                numNodes = Random.Range(43, 46);
            }
            else if (i == 9)
            {
                numNodes = Random.Range(48, 51);
            }

            for (int x = 0; x < lines.Length; x++)
            {
                string[] row = lines[x].Split('\t');
                string layerN_d = row[1];
                string nodeInfo_d = row[2];
                if(int.Parse(layerN_d) == i)
                {
                    nodeInfo_list.Add(int.Parse(nodeInfo_d));
                }
            }

            for (int j = 1; j <= numNodes; j++)
            {
                Vector2 nodePosition = nodeParent.position; // nodeParent의 중심을 기준으로 설정

                float radius = i * 2.0f;
                float angle = 360.0f / numNodes * j * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
                nodePosition += offset;

                int randomIndex = Random.Range(0, nodeInfo_list.Count);
                int randomValue = nodeInfo_list[randomIndex];

                WorldNode randomNode = new WorldNode(code, randomValue, i, 36000 / numNodes * j, 0, -6);
                code++;
                randomNode.position = nodePosition;
                layer.nodes.Add(randomNode);

                if (i == layer.maxLayers - 1 && j == numNodes)
                {
                    randomNode.userExist = 1;
                }

                // 노드용 스프라이트 생성
                GameObject nodeObject = new GameObject("Node" + code);
                nodeObject.transform.SetParent(nodeParent);
                SpriteRenderer spriteRenderer = nodeObject.AddComponent<SpriteRenderer>();

                if (randomNode.userExist == 1)
                {
                    spriteRenderer.sprite = worldNode_icons[1].sprite;
                }
                else if (randomNode.nodeTurn >= layer.worldTurn - 5)
                {
                    spriteRenderer.sprite = worldNode_icons[2].sprite;
                }
                else
                {
                    spriteRenderer.sprite = worldNode_icons[0].sprite;
                }
                nodeObject.transform.position = nodePosition;
                nodeObject.AddComponent<BoxCollider2D>(); // 클릭 영역을 만들기 위해 Collider 추가
                nodeObject.AddComponent<NodeClickHandler>().Init(randomNode, this); // 클릭 핸들러 추가
            }
        }
        ConnectNodes(); // 노드 연결
    }

    private void ConnectNodes()
    {
        float maxDistance = 3.2f; // 연결할 노드들의 최대 거리
        float connectionProbability = 0.4f; // 연결할 확률 설정

        foreach (WorldNode node1 in layer.nodes)
        {
            foreach (WorldNode node2 in layer.nodes)
            {
                if (node1 != node2)
                {
                    float distance = Vector2.Distance(node1.position, node2.position);
                    if (distance <= maxDistance && Random.value <= connectionProbability) // 확률에 따른 연결 추가
                    {
                        NodeConnection connection = new NodeConnection(node1, node2);
                        connections.Add(connection);
                    }
                }
            }
        }
    }

    private void DrawConnections()
    {
        foreach (NodeConnection connection in connections)
        {
            Vector2 midPoint = (connection.node1.position + connection.node2.position) / 2f;

            GameObject lineObject = new GameObject("Line");
            lineObject.transform.SetParent(nodeParent);
            SpriteRenderer lineRenderer = lineObject.AddComponent<SpriteRenderer>();
            lineRenderer.sprite = lineSprite;
            lineRenderer.drawMode = SpriteDrawMode.Tiled; // 선의 길이에 맞게 반복되도록 설정
            lineRenderer.size = new Vector2(Vector2.Distance(connection.node1.position, connection.node2.position), 0.1f);
            lineObject.transform.position = midPoint;

            float angle = Mathf.Atan2(connection.node2.position.y - connection.node1.position.y, connection.node2.position.x - connection.node1.position.x) * Mathf.Rad2Deg;
            lineObject.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    private void DrawConnection2(NodeConnection connection)
    {
        Vector2 midPoint = (connection.node1.position + connection.node2.position) / 2f;

        GameObject lineObject = new GameObject("Line");
        lineObject.transform.SetParent(nodeParent);
        SpriteRenderer lineRenderer = lineObject.AddComponent<SpriteRenderer>();
        lineRenderer.sprite = lineSprite;
        lineRenderer.drawMode = SpriteDrawMode.Tiled;
        lineRenderer.size = new Vector2(Vector2.Distance(connection.node1.position, connection.node2.position), 0.1f);
        lineObject.transform.position = midPoint;

        float angle = Mathf.Atan2(connection.node2.position.y - connection.node1.position.y, connection.node2.position.x - connection.node1.position.x) * Mathf.Rad2Deg;
        lineObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void CheckNodeReachabilityAndConnect()
    {
        visitedNodes.Clear();
        DFS(layer.nodes[0]);

        foreach (WorldNode node in layer.nodes)
        {
            if (!visitedNodes.Contains(node))
            {
                Debug.Log("Node " + node.nodeCode + " is unreachable. Connecting to the closest node.");

                WorldNode closestNode = FindClosestNode(node);
                if (closestNode != null)
                {
                    NodeConnection connection = new NodeConnection(node, closestNode);
                    connections.Add(connection);
                    DrawConnection2(connection); //선 추가로 그리기
                }
            }
        }
    }
    private WorldNode FindClosestNode(WorldNode targetNode)
    {
        WorldNode closestNode = null;
        float minDistance = float.MaxValue;

        foreach (WorldNode node in layer.nodes)
        {
            if (visitedNodes.Contains(node))
            {
                float distance = Vector2.Distance(targetNode.position, node.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestNode = node;
                }
            }
        }
        return closestNode;
    }
    private void DFS(WorldNode currentNode) //깊이 우선 탐색
    {
        visitedNodes.Add(currentNode);

        foreach (NodeConnection connection in connections)
        {
            if (connection.node1 == currentNode && !visitedNodes.Contains(connection.node2))
            {
                DFS(connection.node2);
            }
            else if (connection.node2 == currentNode && !visitedNodes.Contains(connection.node1))
            {
                DFS(connection.node1);
            }
        }
    }

    private void SaveMapData()
    {
        layer.connections.Clear();
        foreach (var conn in connections)
        {
            layer.connections.Add(new NodeConnectionData(conn.node1.nodeCode, conn.node2.nodeCode));
        }

        string jsonData = JsonUtility.ToJson(layer, true);
        File.WriteAllText(Application.persistentDataPath + "/" + saveFilePath, jsonData);
        Debug.Log("Map Data Saved to " + Application.persistentDataPath + "/" + saveFilePath);
    }

    private bool LoadMapData()
    {
        string fullPath = Application.persistentDataPath + "/" + saveFilePath;
        if (File.Exists(fullPath))
        {
            string jsonData = File.ReadAllText(fullPath);
            layer = JsonUtility.FromJson<WorldLayer>(jsonData);

            connections.Clear();
            foreach (var connData in layer.connections)
            {
                WorldNode node1 = layer.nodes.Find(n => n.nodeCode == connData.node1Code);
                WorldNode node2 = layer.nodes.Find(n => n.nodeCode == connData.node2Code);
                if (node1 != null && node2 != null)
                {
                    connections.Add(new NodeConnection(node1, node2));
                }
            }

            Debug.Log("Map Data Loaded from " + fullPath);
            return true;
        }
        else
        {
            Debug.Log("No saved map data found.");
            return false;
        }
    }

    private void DrawAllNodesAndConnections()
    {
        // 불러온 노드와 연결선을 그리는 함수
        foreach (WorldNode node in layer.nodes)
        {
            // 노드용 스프라이트 생성
            GameObject nodeObject = new GameObject("Node" + node.nodeCode);
            nodeObject.transform.SetParent(nodeParent);
            SpriteRenderer spriteRenderer = nodeObject.AddComponent<SpriteRenderer>();
            if (node.userExist == 1)
            {
                spriteRenderer.sprite = worldNode_icons[1].sprite;
            }
            else
            {
                spriteRenderer.sprite = worldNode_icons[0].sprite;
            }


            nodeObject.transform.position = node.position;
            nodeObject.AddComponent<BoxCollider2D>(); // 클릭 영역을 만들기 위해 Collider 추가
            nodeObject.AddComponent<NodeClickHandler>().Init(node, this); // 클릭 핸들러 추가
        }
        DrawConnections();
    }
    public void toVehicle_btn_clicked()
    {
        SceneManager.LoadScene("vehicle_scen");
    }
}