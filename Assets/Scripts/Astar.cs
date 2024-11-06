using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //�s������Node
    [SerializeField] private List<Node> nodes = new List<Node>();//test
    private List<List<Node>> nodeList = new List<List<Node>>(); 

    [SerializeField]private List<Node> openList = new List<Node>();//�j�M�쪺�`�I���[�J
    [SerializeField]private List<Node> closeList = new List<Node>();//�b�}�ҦC����X���̨θ`�I�s���

    [SerializeField]private List<Node> finalPath = new List<Node>();//�̲׸`�I

    [SerializeField]private Node currentNode = null;

    private MapGenerator mapGenerator = null;

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
            StartFindingPath();
        }
        if(start && end && Input.GetKeyDown(KeyCode.Backspace))//���s��z
        {
            Refresh();
        }
        if (Input.GetKeyDown(KeyCode.O))//�ͦ���ê��
        {
            mapGenerator.RandomInsObstacle();
        }
    }

    private void Refresh()
    {
        start.SetNodeToOrigin();
        end.SetNodeToOrigin();
        start = null;
        end = null;

        foreach(Node node in Nodes)
        {
            node.Clear();
        }
        foreach(Node on in openList)
        {
            on.SetNodeToOrigin();
        }
        foreach (Node cn in closeList)
        {
            cn.SetNodeToOrigin();
        }

        openList.Clear();
        closeList.Clear();
        finalPath.Clear();
    }

    private void StartFindingPath()
    {
        if(start && end)
        {
            Debug.Log("Start Astar");
            currentNode = start;
            //openList.Add(currentNode);
            SearchAvailableNode(currentNode);
        }
    }

    /// <summary>
    /// �M��@���I�P�򪺥i�����I
    /// </summary>
    /// <param name="centerNode">�����I�A�Ĥ@������ɬ��_�Istart</param>
    private void SearchAvailableNode(Node centerNode)
    {
        for (int i = centerNode.Coordinate[0] - 1; i <= centerNode.Coordinate[0] + 1; i++)
        {
            if (i < 0 || i >= mapGenerator.Columns) continue;
            for (int j = centerNode.Coordinate[1] - 1; j <= centerNode.Coordinate[1] + 1; j++)
            {
                if (j < 0 || j >= mapGenerator.Rows) continue;

                //Debug.Log("i " + i + " j " + j);

                if (nodeList[i][j] == centerNode)//�ư�centerNode�ۤv
                    continue;
                else if (!nodeList[i][j].Available)//�ư���ê
                    continue;
                else if (FindNodeInOpenList(nodeList[i][j])) continue;//�ư��w�g�b�}�ҦC���`�I
                else if (FindNodeInCloseList(nodeList[i][j])) continue;//�ư��w�g�b�����C���`�I
                else if (nodeList[i][j] == centerNode.ParentNode) continue;//�ư��ۤv�����`�I
                else if (nodeList[i][j] == start) continue;//�ư��_�I
                else//�[�J�}�ҦC��
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

    private void ChooseOpenListTarget()
    {
        openList.Sort((x, y) => x.F.CompareTo(y.F));//�ɾ��Ƨ�
        closeList.Add(openList[0]);
        currentNode = openList[0];

        //�p�G�����I�F
        if (currentNode == end)
        {
            Debug.Log("�����I");
            //�i����`�I�^�ҡA��X�̲׸��|
            FindFinalPath(currentNode.ParentNode);
            return;
        }
        //currentNode.SetNodeMatToChoosed();
        openList.RemoveAt(0);

        SearchAvailableNode(currentNode);

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
    /// �]�w���`�I
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
        //Mathf.Pow(node.transform.position.z - node.ParentNode.transform.position.z, 2));//����`�I���Z��
        float distance = (node.transform.position - node.ParentNode.transform.position).magnitude;//����`�I���Z��
        node.G = node.ParentNode.G + distance;
        node.H = Mathf.Abs(end.Coordinate[0] - node.Coordinate[0]) + Mathf.Abs(end.Coordinate[1] - node.Coordinate[1]);
        node.F = node.G + node.H;
    }

    /// <summary>
    /// ��X�̲׸��|
    /// </summary>
    private void FindFinalPath(Node currentN)
    {
        /*while(currentN != start)
        {
            finalPath.Add(currentN);
            currentN.SetNodeMatTofinalPathMat();
            Debug.Log(currentN.ParentNode.name);
            currentN = currentN.ParentNode;
        }*/
        if (currentN == start) return;

        finalPath.Add(currentN);
        Debug.Log(currentN);
        currentN.SetNodeMatTofinalPathMat();
        FindFinalPath(currentN.ParentNode);
        
    }
}
