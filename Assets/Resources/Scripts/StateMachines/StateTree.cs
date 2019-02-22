using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region StateTreeClass
public class StateTree {

    private Node root;
    private Node currentNode;

	public StateTree()
    {

    }

    public void ResetTree()
    {
        currentNode = root;
    }
}
#endregion

public class Node
{
    private Node leftNode; // for attacking, this is charge
    private Node rightNode; // for attacking, this is wait
    private Node centerNode; // for attacking, this is comboing

    private int heldInt;

    public Node()
    {
        heldInt = 0;
    }

    public Node(int n)
    {
        heldInt = n;
    }

    #region NodeGetters
    public Node GetLeftNode()
    {
        return leftNode;
    }

    public Node GetRightNode()
    {
        return rightNode;
    }

    public Node GetCenterNode()
    {
        return centerNode;
    }
    #endregion

    #region NodeSetters
    public void SetLeftNode(Node nodeIn)
    {
        leftNode = nodeIn;
    }

    public void SetRightNode(Node nodeIn)
    {
        centerNode = nodeIn;
    }

    public void SetCenterNode(Node nodeIn)
    {
        centerNode = nodeIn;
    }
    #endregion
}
