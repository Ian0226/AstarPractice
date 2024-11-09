using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject Node;

    public int Rows;
    public int Columns;

    private float xAxis = 0;
    private float zAxis = 0;

    public int ObstacleCount;//障礙物數量

    private List<Node> obstacleNodes = new List<Node>();

    public List<Node> ObstacleNodes { get => obstacleNodes; set => obstacleNodes = value; }

    private void Awake()
    {
        InitMap();
        MoveCamera();
    }

    private void InitMap()
    {
        for (int i = 0; i < Columns; i++)
        {
            float x = 0;
            List<Node> rowNode = new List<Node>();
            for (int j = 0; j < Rows; j++)
            {
                GameObject obj = Instantiate(Node, new Vector3(x, 0, zAxis), Quaternion.identity);
                obj.name = "Node " + i + " _ " + j;
                x += 1.1f;
                Astar.Instance.Nodes.Add(obj.GetComponent<Node>());//test
                rowNode.Add(obj.GetComponent<Node>());
                obj.GetComponent<Node>().Coordinate = new int[] { i, j };
            }
            Astar.Instance.NodeList.Add(rowNode);
            xAxis = x;
            zAxis += 1.1f;
        }
    }

    private void MoveCamera()
    {
        float x = (Astar.Instance.NodeList[Columns - 1][Rows - 1].gameObject.transform.position.x - Astar.Instance.NodeList[0][0].gameObject.transform.position.x)/2;
        Camera.main.transform.position = new Vector3(x, Camera.main.transform.position.y, Camera.main.transform.position.z);
    }

    /// <summary>
    /// 隨機生成障礙物
    /// </summary>
    public void RandomInsObstacle()
    {
        for(int i = 0; i < ObstacleCount; i++)
        {
            int num = Random.Range(0, Astar.Instance.Nodes.Count);
            if (Astar.Instance.Nodes[num].CanBeObstacle)
            {
                Astar.Instance.Nodes[num].SetNodeMatToObstacle();
                Astar.Instance.Nodes[num].Available = false;
                ObstacleNodes.Add(Astar.Instance.Nodes[num]);
            }
            else
            {
                num = Random.Range(0, Astar.Instance.Nodes.Count);
                i--;
            }
        }
    }
}
