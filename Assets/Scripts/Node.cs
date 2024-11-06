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
    /// 格子可否移動
    /// </summary>
    private bool available = true;

    /// <summary>
    /// 父節點
    /// </summary>
    [SerializeField]private Node parentNode = null;

    public float F { get => f; set => f = value; }
    public float G { get => g; set => g = value; }
    public float H { get => h; set => h = value; }
    public int[] Coordinate { get => coordinate; set => coordinate = value; }
    public bool Available { get => available; set => available = value; }
    public Node ParentNode { get => parentNode; set => parentNode = value; }

    void Start()
    {
        meshRendererComponent = this.GetComponent<MeshRenderer>();
        originMat = meshRendererComponent.material;
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
    }

    private void CheckClick()
    {
        if (Astar.Instance.Start && Astar.Instance.End) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Astar.Instance.Start == null)
            {
                Astar.Instance.Start = this;
                ChangeNodeMaterial(startNodeMat);
            }
            else if (Astar.Instance.Start == this)
            {
                Astar.Instance.Start = null;
                ChangeNodeMaterial(originMat);
            }
            else if (Astar.Instance.End == null && Astar.Instance.End == this)
            {
                Astar.Instance.End = null;
                ChangeNodeMaterial(originMat);
            }
            else if (Astar.Instance.Start != null && Astar.Instance.End == null)
            {
                Astar.Instance.End = this;
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
    }
}
