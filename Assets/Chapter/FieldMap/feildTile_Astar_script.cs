using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class HexTile : IComparable<HexTile>
{
    public Vector3Int position; // 타일의 위치
    public bool isWalkable;     // 타일이 걸을 수 있는지 여부
    public float cost;          // 타일을 지나는데 필요한 비용
    public float gCost;    // 비용 G
    public float fCost;    // 비용 F
    public HexTile parent; // 부모 타일

    public List<HexTile> neighbours; // 이웃 타일들

    public HexTile()
    {
        neighbours = new List<HexTile>();
    }
    public int CompareTo(HexTile other)
    {
        if (fCost < other.fCost)
            return -1;
        else if (fCost > other.fCost)
            return 1;
        else
            return 0;
    }
}
public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> data;

    public PriorityQueue()
    {
        this.data = new List<T>();
    }

    public void Enqueue(T item)
    {
        data.Add(item);
        data.Sort(); // 현재 매번 정렬중. 개선 필요
    }

    public T Dequeue()
    {
        T frontItem = data[0];
        data.RemoveAt(0);
        return frontItem;
    }

    public bool Contains(T item)
    {
        return data.Contains(item);
    }

    public int Count()
    {
        return data.Count;
    }

    public void Remove(T item)
    {
        data.Remove(item);
    }

    public void Clear()
    {
        data.Clear();
    }
}


public class feildTile_Astar_script : MonoBehaviour
{
    public Tilemap tilemap;
    public fieldMapTile_script fieldMapTilescript;
    Dictionary<Vector3Int, HexTile> hexTiles = new Dictionary<Vector3Int, HexTile>();

    public Dictionary<Vector3Int, HexTile> GetTilesFromTilemap()
    {

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                int index = x + y * bounds.size.x;
                TileBase tile = allTiles[index];
                if (tile != null)
                {
                    Vector3Int localPlace = new Vector3Int(x, y, 0);
                    Vector3Int tilePosition = new Vector3Int(bounds.x + x, bounds.y + y, bounds.z);

                    HexTile hexTile = new HexTile();
                    hexTile.position = tilePosition;

                    FieldTileData data = fieldMapTilescript.TileData;

                    if (data.tilesInfo[index].tileCode >= 10 && data.tilesInfo[index].tileCode <= 14)
                        hexTile.isWalkable = false;
                    else if (data.tilesInfo[index].tileCode >= 20 && data.tilesInfo[index].tileCode <= 39)
                        hexTile.isWalkable = false;
                    else if (data.tilesInfo[index].tileCode == 1)
                        hexTile.isWalkable = false;
                    else
                        hexTile.isWalkable = true;
                    hexTile.cost = 1.0f;

                    hexTiles[tilePosition] = hexTile;
                }
            }
        }

        // 모든 타일에 대해 이웃 타일들을 설정
        foreach (var hexTile in hexTiles.Values)
        {
            List<Vector3Int> neighbourPositions = GetNeighbouringTiles(hexTile.position);
            foreach (var pos in neighbourPositions)
            {
                if (hexTiles.ContainsKey(pos))
                {
                    hexTile.neighbours.Add(hexTiles[pos]);
                }
            }
        }
        return hexTiles;
    }

    public List<Vector3Int> GetNeighbouringTiles(Vector3Int currentTile)
    {
        List<Vector3Int> neighbouringTiles = new List<Vector3Int>();

        // 평면 (flat-topped) 헥스 그리드의 이웃 타일 오프셋
        Vector3Int[] evenRowOffsets = {
        new Vector3Int(0, -1, 0),  // NorthWest
        new Vector3Int(1, 0, 0),   // North
        new Vector3Int(0, 1, 0),   // NorthEast
        new Vector3Int(-1, 1, 0),  // SouthEast
        new Vector3Int(-1, 0, 0),  // South
        new Vector3Int(-1, -1, 0)  // SouthWest
    };

        Vector3Int[] oddRowOffsets = {
        new Vector3Int(1, -1, 0),  // NorthWest
        new Vector3Int(1, 0, 0),   // North
        new Vector3Int(1, 1, 0),   // NorthEast
        new Vector3Int(0, 1, 0),   // SouthEast
        new Vector3Int(-1, 0, 0),  // South
        new Vector3Int(0, -1, 0)   // SouthWest
    };

        // 현재 타일의 행이 짝수인지 홀수인지에 따라 오프셋 선택
        Vector3Int[] offsets = (currentTile.y % 2 == 0) ? evenRowOffsets : oddRowOffsets;

        foreach (Vector3Int offset in offsets)
        {
            Vector3Int neighbourTile = currentTile + offset;
            neighbouringTiles.Add(neighbourTile);
        }
        return neighbouringTiles;
    }

    public float ManhattanDistanceHeuristic(HexTile start, HexTile goal)
    {
        Vector3Int startCoord = start.position;
        Vector3Int goalCoord = goal.position;

        // 각 타일의 z 좌표를 계산
        int startZ = -startCoord.x - startCoord.y;
        int goalZ = -goalCoord.x - goalCoord.y;

        int dx = Mathf.Abs(startCoord.x - goalCoord.x);
        int dy = Mathf.Abs(startCoord.y - goalCoord.y);
        int dz = Mathf.Abs(startZ - goalZ);

        // 맨하탄 거리 계산
        return Mathf.Max(dx, dy, dz);
    }

    public List<HexTile> FindPath(Vector3Int startTilePos, Vector3Int endTilePos)
    {
        Dictionary<Vector3Int, HexTile> tiles = GetTilesFromTilemap();

        if (!tiles.ContainsKey(startTilePos) || !tiles.ContainsKey(endTilePos))
            return null; // 시작점 또는 끝점이 존재하지 않는 경우

        HexTile startTile = tiles[startTilePos];
        HexTile endTile = tiles[endTilePos];

        PriorityQueue<HexTile> openList = new PriorityQueue<HexTile>();
        HashSet<HexTile> closedList = new HashSet<HexTile>();
        List<HexTile> endTileNeighbours = endTile.neighbours; // endTile의 이웃 타일들

        openList.Enqueue(startTile);

        while (openList.Count() > 0)
        {
            HexTile currentTile = openList.Dequeue();

            if (endTileNeighbours.Contains(currentTile))
            {
                // 경로 찾기 성공
                HexTile reachedNeighbourTile = currentTile;
                return RetracePath(startTile, reachedNeighbourTile);
            }

            closedList.Add(currentTile);

            foreach (HexTile neighbour in currentTile.neighbours)
            {
                if (!neighbour.isWalkable || closedList.Contains(neighbour))
                    continue;

                float newGCost = currentTile.gCost + neighbour.cost;
                if (newGCost < neighbour.gCost || !openList.Contains(neighbour))
                {
                    neighbour.gCost = newGCost;
                    neighbour.fCost = neighbour.gCost + ManhattanDistanceHeuristic(neighbour, endTile);
                    neighbour.parent = currentTile;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Enqueue(neighbour);
                    }
                }
            }
        }
        HexTile closestTile = FindClosestTileToGoal(closedList, endTile);

        if (closestTile != null)
        {
            return RetracePath(startTile, closestTile);
        }

        return null; // 경로를 찾을 수 없는 경우
    }

    private HexTile FindClosestTileToGoal(IEnumerable<HexTile> tiles, HexTile goalTile)
    {
        float closestDistance = float.MaxValue;
        HexTile closestTile = null;

        foreach (HexTile tile in tiles)
        {
            float distance = ManhattanDistanceHeuristic(tile, goalTile);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tile;
            }
        }

        return closestTile;
    }

    private List<HexTile> RetracePath(HexTile startTile, HexTile endTile)
    {
        List<HexTile> path = new List<HexTile>();
        HexTile currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Reverse();

        return path;
    }

    //추가기능

    public void HighlightPath(List<HexTile> path, Color highlightColor)
    {
        foreach (HexTile hexTile in path)
        {
            Vector3Int cellPosition = hexTile.position;
            tilemap.SetTileFlags(cellPosition, TileFlags.None);
            tilemap.SetColor(cellPosition, highlightColor);
        }
    }
    public List<HexTile> GetTilesWithinRange(Vector3Int startTilePos, int range)
    {
        Dictionary<Vector3Int, HexTile> tiles = GetTilesFromTilemap();

        if (!tiles.ContainsKey(startTilePos))
            return null;

        HexTile startTile = tiles[startTilePos];

        List<HexTile> resultTiles = new List<HexTile>();
        List<HexTile> returnTiles = new List<HexTile> { startTile };

        for (int i = 0; i < range; i++)
        {
            foreach (HexTile tilex in returnTiles)
            {
                foreach (HexTile neighbour in tilex.neighbours)
                {
                    if (!resultTiles.Contains(neighbour))
                    {
                        resultTiles.Add(neighbour);
                    }
                }
            }
            returnTiles = new List<HexTile>(resultTiles);
        }
        return resultTiles;
    }
    public HexTile GetHexTileAtPosition(Vector3Int position)
    {
        if (hexTiles != null && hexTiles.ContainsKey(position))
        {
            return hexTiles[position];
        }
        return null; // 해당 위치에 대한 HexTile이 없는 경우
    }
    public List<HexTile> GetMovableTiles(List<HexTile> checkTiles, Vector3Int startTilePos, int steps)
    {
        if (checkTiles == null || checkTiles.Count == 0)
            return new List<HexTile>();

        HashSet<HexTile> visited = new HashSet<HexTile>();
        List<HexTile> result = new List<HexTile>();

        Queue<(HexTile tile, int depth)> queue = new Queue<(HexTile, int)>();
        queue.Enqueue((GetHexTileAtPosition(startTilePos), 0));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            HexTile currentTile = current.tile;
            int currentStep = current.depth;

            if (currentStep > steps)
                continue;

            if (checkTiles.Contains(currentTile))
                result.Add(currentTile);

            foreach (HexTile neighbour in currentTile.neighbours)
            {
                if (!visited.Contains(neighbour) && neighbour.isWalkable)
                {
                    visited.Add(neighbour);
                    queue.Enqueue((neighbour, currentStep + 1));
                }
            }
        }
        return result;
    }
    public int GetTileDistanceWithoutWalkableCheck(Vector3Int startTilePos, Vector3Int endTilePos)
    {
        Dictionary<Vector3Int, HexTile> tiles = GetTilesFromTilemap();

        if (!tiles.ContainsKey(startTilePos) || !tiles.ContainsKey(endTilePos))
            return -1; // 시작점 또는 끝점이 존재하지 않는 경우

        HexTile startTile = tiles[startTilePos];
        HexTile endTile = tiles[endTilePos];

        Queue<HexTile> queue = new Queue<HexTile>();
        HashSet<HexTile> visited = new HashSet<HexTile>();

        queue.Enqueue(startTile);

        int depth = 0;

        while (queue.Count > 0)
        {
            int count = queue.Count;

            for (int i = 0; i < count; i++)
            {
                HexTile currentTile = queue.Dequeue();

                if (currentTile == endTile)
                    return depth;

                foreach (HexTile neighbour in currentTile.neighbours)
                {
                    if (!visited.Contains(neighbour))
                    {
                        visited.Add(neighbour);
                        queue.Enqueue(neighbour);
                    }
                }
            }
            depth++;
        }
        return -1; // 경로를 찾을 수 없는 경우
    }
}
