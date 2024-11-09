using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Astar : MonoBehaviour
{
    private Astar(){}

    private static Astar _instance;
    public static Astar Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.Find("Astar").GetComponent<Astar>();
            }
            return _instance;
        }
    }

    [SerializeField]private Node start = null;

    [SerializeField]private Node end = null;

    //存取全部Node
    [SerializeField] private List<Node> nodes = new List<Node>();//test
    private List<List<Node>> nodeList = new List<List<Node>>(); 

    [SerializeField]private List<Node> openList = new List<Node>();//搜尋到的節點先加入
    [SerializeField]private List<Node> closeList = new List<Node>();//在開啟列表中找出的最佳節點存放於此

    [SerializeField]private List<Node> finalPath = new List<Node>();//最終節點

    [SerializeField]private Node currentNode = null;

    private MapGenerator mapGenerator = null;

    public bool StraightPath = false;
    private bool straightPathTemp = false;

    public Node Start { get => start; set => start = value; }
    public Node End { get => end; set => end = value; }
    public List<Node> Nodes { get => nodes; set => nodes = value; }
    public List<List<Node>> NodeList { get => nodeList; set => nodeList = value; }

    private void Awake()
    {
        mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            straightPathTemp = StraightPath;
            StartFindingPath();
        }
        if(start && end && Input.GetKeyDown(KeyCode.Backspace))//重新整理
        {
            Refresh();
        }
        if (Input.GetKeyDown(KeyCode.C))//生成障礙物
        {
            mapGenerator.RandomInsObstacle();
        }
        if (Input.GetKeyDown(KeyCode.D))//清除障礙物
        {
            foreach(Node node in mapGenerator.ObstacleNodes)
            {
                node.ResetAvailable();
            }
        }
    }

    private void Refresh()
    {
        start = null;
        end = null;

        foreach(Node node in Nodes)
        {
            if(node.Available)
                node.Clear();
        }

        openList.Clear();
        closeList.Clear();
        finalPath.Clear();

        ClearConsole();
    }

    private void StartFindingPath()
    {
        if(start && end)
        {
            Debug.Log("Start Astar");
            currentNode = start;
            //openList.Add(currentNode);
            if (straightPathTemp)
            {
                SearchAvailableNodeStraight(currentNode);
            }
            else
            {
                SearchAvailableNode(currentNode);
            }
        }
    }

    /// <summary>
    /// 尋找一個點周圍的可移動點
    /// </summary>
    /// <param name="centerNode">中心點，第一次執行時為起點start</param>
    private void SearchAvailableNode(Node centerNode)
    {
        for (int i = centerNode.Coordinate[0] - 1; i <= centerNode.Coordinate[0] + 1; i++)
        {
            if (i < 0 || i >= mapGenerator.Columns) continue;
            for (int j = centerNode.Coordinate[1] - 1; j <= centerNode.Coordinate[1] + 1; j++)
            {
                if (j < 0 || j >= mapGenerator.Rows) continue;

                //Debug.Log("i " + i + " j " + j);

                if (nodeList[i][j] == centerNode)//排除centerNode自己
                    continue;
                else if (!nodeList[i][j].Available)//排除障礙
                    continue;
                else if (FindNodeInOpenList(nodeList[i][j])) continue;//排除已經在開啟列表的節點
                else if (FindNodeInCloseList(nodeList[i][j])) continue;//排除已經在關閉列表的節點
                else if (nodeList[i][j] == centerNode.ParentNode) continue;//排除自己的父節點
                else if (nodeList[i][j] == start) continue;//排除起點
                else//加入開啟列表
                {
                    openList.Add(nodeList[i][j]);
                    SetParent(nodeList[i][j], centerNode);
                    //Debug.Log(nodeList[i][j].ParentNode);
                    Calculator(nodeList[i][j]);
                }
            }
        }
        ChooseOpenListTarget();
    }

    private void SearchAvailableNodeStraight(Node centerNode)
    {
        for (int i = centerNode.Coordinate[0] - 1; i <= centerNode.Coordinate[0] + 1; i+=2)
        {
            if (i < 0 || i >= mapGenerator.Columns) continue;
            if (nodeList[i][centerNode.Coordinate[1]] == centerNode)//排除centerNode自己
                continue;
            else if (!nodeList[i][centerNode.Coordinate[1]].Available)//排除障礙
                continue;
            else if (FindNodeInOpenList(nodeList[i][centerNode.Coordinate[1]])) continue;//排除已經在開啟列表的節點
            else if (FindNodeInCloseList(nodeList[i][centerNode.Coordinate[1]])) continue;//排除已經在關閉列表的節點
            else if (nodeList[i][centerNode.Coordinate[1]] == centerNode.ParentNode) continue;//排除自己的父節點
            else if (nodeList[i][centerNode.Coordinate[1]] == start) continue;//排除起點
            else//加入開啟列表
            {
                openList.Add(nodeList[i][centerNode.Coordinate[1]]);
                SetParent(nodeList[i][centerNode.Coordinate[1]], centerNode);
                //Debug.Log(nodeList[i][j].ParentNode);
                Calculator(nodeList[i][centerNode.Coordinate[1]]);
            }
        }
        for (int j = centerNode.Coordinate[1] - 1; j <= centerNode.Coordinate[1] + 1; j += 2)
        {
            if (j < 0 || j >= mapGenerator.Rows) continue;

            //Debug.Log("i " + i + " j " + j);

            if (nodeList[centerNode.Coordinate[0]][j] == centerNode)//排除centerNode自己
                continue;
            else if (!nodeList[centerNode.Coordinate[0]][j].Available)//排除障礙
                continue;
            else if (FindNodeInOpenList(nodeList[centerNode.Coordinate[0]][j])) continue;//排除已經在開啟列表的節點
            else if (FindNodeInCloseList(nodeList[centerNode.Coordinate[0]][j])) continue;//排除已經在關閉列表的節點
            //else if (nodeList[centerNode.Coordinate[0]][j] == centerNode.ParentNode) continue;//排除自己的父節點
            else if (nodeList[centerNode.Coordinate[0]][j] == start) continue;//排除起點
            else//加入開啟列表
            {
                openList.Add(nodeList[centerNode.Coordinate[0]][j]);
                SetParent(nodeList[centerNode.Coordinate[0]][j], centerNode);
                //Debug.Log(nodeList[i][j].ParentNode);
                Calculator(nodeList[centerNode.Coordinate[0]][j]);
            }
        }
        ChooseOpenListTarget();
    }

    private void ChooseOpenListTarget()
    {
        openList.Sort((x, y) => x.F.CompareTo(y.F));//升冪排序
        closeList.Add(openList[0]);
        currentNode = openList[0];

        //如果找到終點了
        if (currentNode == end)
        {
            Debug.Log("找到終點");
            //進行父節點回朔，找出最終路徑
            FindFinalPath(currentNode.ParentNode);
            FinalPathDisplay();
            return;
        }
        //currentNode.SetNodeMatToChoosed();
        openList.RemoveAt(0);

        if (!straightPathTemp)
            SearchAvailableNode(currentNode);
        else
            SearchAvailableNodeStraight(currentNode);

    }

    private bool FindNodeInOpenList(Node node)
    {
        return openList.Contains(node);
    }

    private bool FindNodeInCloseList(Node node)
    {
        return closeList.Contains(node);
    }

    /// <summary>
    /// 設定父節點
    /// </summary>
    /// <param name="node"></param>
    private void SetParent(Node currentNode,Node parentNode)
    {
        currentNode.ParentNode = parentNode;
    }

    /// <summary>
    /// Calculate G, H and F of each Node
    /// </summary>
    private void Calculator(Node node)
    {
        //float distance = Mathf.Sqrt(Mathf.Pow(node.transform.position.x - node.ParentNode.transform.position.x, 2) +
        //Mathf.Pow(node.transform.position.z - node.ParentNode.transform.position.z, 2));//到父節點的距離
        float distance = (node.transform.position - node.ParentNode.transform.position).magnitude;//到父節點的距離
        
        node.G = node.ParentNode.G + distance;
        node.H = Mathf.Abs(end.Coordinate[0] - node.Coordinate[0]) + Mathf.Abs(end.Coordinate[1] - node.Coordinate[1]);
        node.F = node.G + node.H;
    }

    /// <summary>
    /// 找出並顯示最終路徑
    /// </summary>
    private void FindFinalPath(Node currentN)
    {
        if (currentN == start) return;
        FindFinalPath(currentN.ParentNode);
        Debug.Log(currentN);
        finalPath.Add(currentN);
    }

    private void FinalPathDisplay()
    {
        StartCoroutine("ShowFinalPath");
    }

    IEnumerator ShowFinalPath()
    {
        foreach(Node node in finalPath)
        {
            yield return new WaitForSeconds(0.1f);
            node.SetNodeMatTofinalPathMat();
        }
    }

    private void ClearConsole()
    {
#if UNITY_EDITOR
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.SceneView));
        System.Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        System.Reflection.MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
        clearConsoleMethod.Invoke(new object(), null);
#endif
    }
}
