using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region StateTreeClass
public class StateTree<T> where T : new() {

    private Node<T> root;
    private Node<T> currentNode;

	public StateTree()
    {
        root = new Node<T>(); // root should be ready state, no attack
        currentNode = root;
    }

    public StateTree(T item)
    {
        root = new Node<T>(); // root should be ready state, no attack
        currentNode = root;

        SetChildNode(item, Direction.CENTER);
    }

    public StateTree(Node<T> rootNode)
    {
        root = new Node<T>(); // root should be ready state, no attack
        currentNode = root;
    }

    public void ResetTree()
    {
        currentNode = root;
    }

    public bool MoveLeft()
    {
        Node<T> nextNode = currentNode.GetLeftNode();
        if (nextNode == null)
        {
            return false;
        }
        currentNode = nextNode;
        return true;
    }

    public bool MoveRight()
    {
        Node<T> nextNode = currentNode.GetRightNode();
        if (nextNode == null)
        {
            return false;
        }
        currentNode = nextNode;
        return true;
    }

    public bool MoveCenter()
    {
        Node<T> nextNode = currentNode.GetCenterNode();
        if (nextNode == null)
        {
            return false;
        }
        currentNode = nextNode;
        return true;
    }

    public void MoveUp()
    {
        currentNode = currentNode.GetParent();
    }

    public T GetCurrentItem()
    {
        return currentNode.GetHeldItem();
    }

    #region NodeGeneration
    public void SetChildNode(T itemIn, Direction dirIn)
    {
        switch (dirIn)
        {
            case (Direction.LEFT):
            {
                currentNode.SetLeftNode(new Node<T>(itemIn));
                return;
            }
            case (Direction.CENTER):
            {
                currentNode.SetCenterNode(new Node<T>(itemIn));
                return;
            }
            case (Direction.RIGHT):
            {
                currentNode.SetRightNode(new Node<T>(itemIn));
                return;
            }
        }
    }

    // for tree generation, we need to make sure there's not already a node there.
    public bool DoesChildExist(Direction dirIn)
    {
        switch (dirIn)
        {
            case (Direction.LEFT):
            {
                if (currentNode.GetLeftNode() == null)
                {
                    return false;
                }
                break;
            }
            case (Direction.CENTER):
            {
                if (currentNode.GetCenterNode() == null)
                {
                    return false;
                }
                break;
            }
            case (Direction.RIGHT):
            {
                if (currentNode.GetRightNode() == null)
                {
                    return false;
                }
                break;
            }
        }
        return true;
    }

    public Node<T> PrintNode()
    {
        return currentNode;
    }
    #endregion
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
        rightNode = nodeIn;
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

#region TreeTraversal
public enum Direction
{
    LEFT = 0,
    CENTER,
    RIGHT,

    // Attack tree
    CHARGE = 0,
    COMBO = 1,
    WAIT = 2,
}
#endregion
