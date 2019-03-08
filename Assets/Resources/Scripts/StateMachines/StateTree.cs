using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region StateTreeClass
public class StateTree<T> where T : new() {

    private Node<T> root;
    private Node<T> currentNode;

	public StateTree()
    {
        root = new Node<T>();
    }

    public StateTree(T item)
    {
        root = new Node<T>(item);
    }

    public StateTree(Node<T> rootNode)
    {
        root = rootNode;
    }

    public void ResetTree()
    {
        currentNode = root;
    }

    public void MoveLeft()
    {
        currentNode = currentNode.GetLeftNode();
    }

    public void MoveRight()
    {
        currentNode = currentNode.GetRightNode();
    }

    public void MoveCenter()
    {
        currentNode = currentNode.GetCenterNode();
    }

    public void MoveUp()
    {
        currentNode = currentNode.GetParent();
    }

    public T GetCurrentItem()
    {
        return currentNode.GetHeldItem();
    }
}
#endregion

public class Node<T> where T : new()
{
    private Node<T> leftNode; // for attacking, this is charge
    private Node<T> rightNode; // for attacking, this is wait
    private Node<T> centerNode; // for attacking, this is comboing
    private T heldItem;

    private Node<T> parent;

    public Node()
    {
        heldItem = default(T);
    }

    public Node(T n)
    {
        heldItem = n;
    }

    #region NodeGetters
    public Node<T> GetLeftNode()
    {
        return leftNode;
    }

    public Node<T> GetRightNode()
    {
        return rightNode;
    }

    public Node<T> GetCenterNode()
    {
        return centerNode;
    }
    public Node<T> GetParent()
    {
        return parent;
    }

    public T GetHeldItem()
    {
        return heldItem;
    }
    #endregion

    #region NodeSetters
    public void SetLeftNode(Node<T> nodeIn)
    {
        leftNode = nodeIn;
    }

    public void SetRightNode(Node<T> nodeIn)
    {
        centerNode = nodeIn;
    }

    public void SetCenterNode(Node<T> nodeIn)
    {
        centerNode = nodeIn;
    }
    
    public void SetParent(Node<T> nodeIn)
    {
        parent = nodeIn;
    }
    #endregion
}
