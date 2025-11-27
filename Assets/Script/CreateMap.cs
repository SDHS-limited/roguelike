using System.Collections.Generic;
using UnityEngine;

public class BSPNode
{
    public Rect rect;
    public BSPNode left;
    public BSPNode right;
    public Rect room;

    public BSPNode(Rect rect) => this.rect = rect;
    public bool IsLeaf => left == null && right == null;
}

public class CreateMap : MonoBehaviour
{
    [SerializeField] int width = 60;
    [SerializeField] int height = 60;
    [SerializeField] int maxDepth = 4;

    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject corridorPrefab;

    private BSPNode root;
    private List<Rect> rooms = new List<Rect>();
    private int[,] map;

    void Start()
    {
        map = new int[width, height];
        root = new BSPNode(new Rect(0, 0, width, height));
        Divide(root, 0);
        GenerateRooms(root);
        ConnectRooms(root);

        BuildDungeon();
    }
    //맵을 계속 2개의 영역으로 쪼개기
    void Divide(BSPNode node, int depth)
    {
        if(depth >= maxDepth) return;

        //가로가 더 길면 세로로 분리, 세로가 길면 가로로 분리
        bool splitVertical = node.rect.width >= node.rect.height;
        float ratio = Random.Range(0.4f, 0.7f);
        //너무 작은 공간 방지
        int split = Mathf.RoundToInt(splitVertical ? node.rect.width * ratio : node.rect.height * ratio);

        //실제로 Rect를 기준으로 두 개의 공간으로 나눔
        if (splitVertical)
        {
            node.left = new BSPNode(new Rect(node.rect.x, node.rect.y, split, node.rect.height));
            node.right = new BSPNode(new Rect(node.rect.x + split, node.rect.y, node.rect.width - split, node.rect.height));
        }
        else
        {
            node.left = new BSPNode(new Rect(node.rect.x, node.rect.y, node.rect.width, split));
            node.right = new BSPNode(new Rect(node.rect.x, node.rect.y + split, node.rect.width, node.rect.height - split));
        }

        Divide(node.left, depth + 1);
        Divide(node.right, depth + 1);
    }

    void GenerateRooms(BSPNode node)
    {
         if (node.IsLeaf)
        {
            //방 크기 랜덤으로 정함
            int w = Mathf.RoundToInt(node.rect.width * Random.Range(0.5f, 0.8f));
            int h = Mathf.RoundToInt(node.rect.height * Random.Range(0.5f, 0.8f));
            //약간 여유 공간 생성
            int x = (int)Random.Range(node.rect.x + 1, node.rect.x + node.rect.width - w);
            int y = (int)Random.Range(node.rect.y + 1, node.rect.y + node.rect.height - h);
            //방 생성후 리스트에 저장
            node.room = new Rect(x, y, w, h);
            rooms.Add(node.room);
        }
        else
        {
            GenerateRooms(node.left);
            GenerateRooms(node.right);
        }
    }
    //복도 연결
    void ConnectRooms(BSPNode node)
    {

        Vector2Int p1 = GetRoomCenter(node.left);
        Vector2Int p2 = GetRoomCenter(node.right);

        CreateCorridor(p1, p2);

        if(!node.left.IsLeaf) ConnectRooms(node.left);
        if(!node.right.IsLeaf) ConnectRooms(node.right);
    }

    Vector2Int GetRoomCenter(BSPNode node)
    {
        if (node.room.width > 0) return RoomCenter(node.room);
        if (node.left != null) return GetRoomCenter(node.left);
        if (node.right != null) return GetRoomCenter(node.right);

        return Vector2Int.zero;
    }

    void BuildDungeon()
    {
        foreach (Rect room in rooms)
        {
            for (int x = (int)room.x; x < room.xMax; x++)
                for (int y = (int)room.y; y < room.yMax; y++)
                    map[x, y] = 1; // room floor
        
            BuildRoomWalls(room);
        }

        // 마지막 타일 Instantiate
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                    Instantiate(floorPrefab, new Vector3(x, y), Quaternion.identity, transform);
                else if (map[x, y] == 2)
                    Instantiate(wallPrefab, new Vector3(x, y), Quaternion.identity, transform);
                else if (map[x, y] == 3)
                    Instantiate(corridorPrefab, new Vector3(x, y), Quaternion.identity, transform);
            }
        }
    }

    void BuildRoomWalls(Rect room)
    {
        for (int x = (int)room.x - 1; x <= room.xMax; x++)
        {
            map[x, (int)room.y - 1] = 2;
            map[x, (int)room.yMax] = 2;
        }

        for (int y = (int)room.y - 1; y <= room.yMax; y++)
        {
            map[(int)room.x - 1, y] = 2;
            map[(int)room.xMax, y] = 2;
        }
    }

     void CreateCorridor(Vector2Int a, Vector2Int b)
    {
        while (a.x != b.x)
        {
            AddCorridor(a.x, a.y);
            a.x += a.x < b.x ? 1 : -1;
        }

        while (a.y != b.y)
        {
            AddCorridor(a.x, a.y);
            a.y += a.y < b.y ? 1 : -1;
        }
    }
    void AddCorridor(int x, int y)
    {
        // 벽이면 제거하고 corridor로 변경
        map[x, y] = 3;
    }

    Vector2Int RoomCenter(Rect r) =>
        new Vector2Int((int)(r.x + r.width / 2), (int)(r.y + r.height / 2));

        
}
