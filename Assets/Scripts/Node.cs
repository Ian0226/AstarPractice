using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private MeshRenderer meshRendererComponent;

    private Material originMat;
    [SerializeField] private Material startNodeMat;
    [SerializeField] private Material endNodeMat;
    [SerializeField] private Material choosedMat;
    [SerializeField] private Material obstacleMat;
    [SerializeField] private Material finalPathMat;

    private int[] coordinate = { 0, 0, };

    //Properties
    [SerializeField]private float f;
    [SerializeField]private float g;
    [SerializeField]private float h;

    /// <summary>
    /// 格子可否為障礙
    /// </summary>
    private bool available = true;

    private bool canBeObstacle = true;

    /// <summary>
    /// 父節點
    /// </summary>
    [SerializeField]private Node parentNode = null;

    private Vector3 nodeOriginPos;

    public float F { get => f; set => f = value; }
    public float G { get => g; set => g = value; }
    public float H { get => h; set => h = value; }
    public int[] Coordinate { get => coordinate; set => coordinate = value; }
    public bool Available { get => available; set => available = value; }
    public Node ParentNode { get => parentNode; set => parentNode = value; }
    public bool CanBeObstacle { get => canBeObstacle; set => canBeObstacle = value; }

    void Start()
    {
        meshRendererComponent = this.GetComponent<MeshRenderer>();
        originMat = meshRendererComponent.material;

        nodeOriginPos = this.transform.position;
        Debug.Log(nodeOriginPos);
    }

    private void OnMouseDown()
    {
        if (available)
            CheckClick();
        else
            Debug.Log("該方塊為障礙方塊，無法點擊");
        //ChangeNodeMaterial(this.gameObject);
    }
    private void ChangeNodeMaterial(Material material)
    {
        meshRendererComponent.material = material;
    }

    public void SetNodeToOrigin()
    {
        ChangeNodeMaterial(originMat);
        Clear();
    }

    private void SetNodeToStart()
    {
        ChangeNodeMaterial(startNodeMat);
        SetNodeYPos(nodeOriginPos.y + 0.5f);
        CanBeObstacle = false;
    }

    private void SetNodeToEnd()
    {
        ChangeNodeMaterial(endNodeMat);
        SetNodeYPos(nodeOriginPos.y + 0.5f);
        CanBeObstacle = false;
    }

    public void SetNodeMatToChoosed()
    {
        ChangeNodeMaterial(choosedMat);
    }

    public void SetNodeMatToObstacle()
    {
        ChangeNodeMaterial(obstacleMat);
    }

    public void SetNodeMatTofinalPathMat()
    {
        ChangeNodeMaterial(finalPathMat);
        SetNodeYPos(nodeOriginPos.y + 0.5f);
        CanBeObstacle = false;
    }

    private void CheckClick()
    {
        if (Astar.Instance.Start && Astar.Instance.End) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Astar.Instance.Start == null)
            {
                Astar.Instance.Start = this;
                SetNodeToStart();
                //ChangeNodeMaterial(startNodeMat);
            }
            else if (Astar.Instance.Start == this)
            {
                Astar.Instance.Start = null;
                //ChangeNodeMaterial(originMat);
                Clear();
            }
            else if (Astar.Instance.End == null && Astar.Instance.End == this)
            {
                Astar.Instance.End = null;
                //ChangeNodeMaterial(originMat);
                Clear();
            }
            else if (Astar.Instance.Start != null && Astar.Instance.End == null)
            {
                Astar.Instance.End = this;
                SetNodeToEnd();
                ChangeNodeMaterial(endNodeMat);
            }
        }  
    }

    public void Clear()
    {
        f = 0;
        g = 0;
        h = 0;
        parentNode = null;

        ChangeNodeMaterial(originMat);
        SetNodeYPos(nodeOriginPos.y);

        CanBeObstacle = true;
    }

    public void ResetAvailable()
    {
        available = true;
        ChangeNodeMaterial(originMat);
    }

    private void SetNodeYPos(float targetY)
    {
        StartCoroutine("MoveNodeY", targetY);
        //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z);
    }

    IEnumerator MoveNodeY(float targetY)
    {
        float currentY = 0.0f;
        while (Mathf.Abs(this.transform.position.y - targetY) > 0.01f)
        {
            float yPos = Mathf.SmoothDamp(this.transform.position.y, targetY, ref currentY, 0.15f);
            this.transform.position = new Vector3(this.transform.position.x, yPos, this.transform.position.z);
            yield return null;
        }
        this.transform.position = new Vector3(this.transform.position.x, targetY, this.transform.position.z);
    }
}

