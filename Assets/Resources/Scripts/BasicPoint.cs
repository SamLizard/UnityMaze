using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPoint
{
    public Node<Point> point;
    public BasicPoint next;

    public BasicPoint(Node<Point> point, BasicPoint next)
    {
        this.point = point;
        this.next = next;
    }

    public BasicPoint(Node<Point> point)
    {
        this.point = point;
        this.next = null;
    }

    public Node<Point> getValue()
    {
        return this.point;
    }

    public BasicPoint getNext()
    {
        return this.next;
    }

    public void setValue(Node<Point> point)
    {
        this.point = point;
    }

    public void setNext(BasicPoint next)
    {
        this.next = next;
    }

    public void addBefore(Node<Point> newPointNode)
    {
        newPointNode.setNext(this.point);
        this.point = newPointNode;  
    }

    public string toString()
    {
        return this.point.toString();
    }
}
