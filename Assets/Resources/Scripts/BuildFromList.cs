using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildFromList : MonoBehaviour
{
    private readonly int square_size = 10;
    public int size;

    private readonly Tuple<int, int>[] directions = new Tuple<int, int>[4] {
        new Tuple<int, int>(0, -1),
        new Tuple<int, int>(-1, 0),
        new Tuple<int, int>(0, 1),
        new Tuple<int, int>(1, 0)
    };

    private readonly int[,] direction = new int[24, 4]
    {
        {0, 1, 2, 3 },
        {0, 1, 3, 2 },
        {0, 2, 1, 3 },
        {0, 2, 3, 1 },
        {0, 3, 2, 1 },
        {0, 3, 1, 2 },
        {1, 0, 2, 3 },
        {1, 0, 3, 2 },
        {1, 2, 0, 3 },
        {1, 2, 3, 0 },
        {1, 3, 2, 0 },
        {1, 3, 0, 2 },
        {2, 0, 1, 3 },
        {2, 0, 3, 1 },
        {2, 1, 3, 0 },
        {2, 1, 0, 3 },
        {2, 3, 0, 1 },
        {2, 3, 1, 0 },
        {3, 0, 2, 1 },
        {3, 0, 1, 2 },
        {3, 1, 2, 0 },
        {3, 1, 0, 2 },
        {3, 2, 1, 0 },
        {3, 2, 0, 1 },
    };

    public GameObject[] squares;
    void Start()
    {
        // BasicPoint basicPointTable = InstantiateDefaultBasicPointTable(size);                  // 1. not needed - already in 3.
        // int[,] tableTryTwo = BuildListOfZeroAndIncreasingNumber(size);                         // 2. not needed - already in 3.
        int[,] table = CreateMazeList();                                                          // 3.
        InstantiateMaze(table);                                                                   // 4.
    }

    private bool IsOutOfBounds(int row, int column)
    {
        return row > size + 1
            || row < 0
            || column > size + 1
            || column < 0;
    }

    private int[,] CreateMazeList()
    {
        int[,] tableList = BuildListOfZeroAndIncreasingNumber(size);
        BasicPoint basicPointTable = InstantiateDefaultBasicPointTable(size);
        int biggestValueOfBasicPoint = FindBiggestValueOfBasicPoint(basicPointTable);
        Debug.Log(biggestValueOfBasicPoint + "---------/////////////////-------------------------------------------------");
        int count = 150; // 50  * biggestValueOfBasicPoint; // just for the testing part.
        // int basicPointToChoose = 0; // this is the place of the basicPoint in the list of BasicPoints (here its the first basicPoint = basicPointTable)
        int direction = 0; // the direction from the BasicPoint to the other BasicPoint (when removing the point between them). 0,1,2,3 - the place int the list of tuples *directions*.
        Point actualPoint = RandomPointFromBasicPoint(basicPointTable); // = null?
        while (!IsDone(basicPointTable, biggestValueOfBasicPoint) && count >= 0) // 
        {
            // if is -1 (is there is no direction)
            count--;
            actualPoint = RandomPointFromBasicPoint(basicPointTable);
            direction = RandomDirectionToRemovingWall(actualPoint, tableList);
            if (direction != -1)
            {
                Debug.Log("374\n" + "direction:" + direction + "\nTableList:\n" + StringList(tableList) + "\nbasicPointTable:\n" + StringBasicPoints(basicPointTable) + "\nactualPoint:\n" + actualPoint.toString());
                tableList = UpdateTable(tableList, actualPoint, direction, basicPointTable); // that also update the basicPoints
                //numOfBasicPoints--;  only if the basicPoint has been deleted (check every time home much basicpoints are in the list?)
            }
        }
        // add something that say that it can be chosen only from basicpoints that have one node<Point>.
        Debug.Log("385\n" + "THE END\nTableList:\n" + StringList(tableList) + "\nbasicPointTable:\n" + StringBasicPoints(basicPointTable) + "\ncount: " + count); // + "\ncount: " + count
        return tableList;
    }

    private int[,] UpdateTable(int[,] tableList, Point actualPoint, int direction, BasicPoint basicPointTable)
    {
        Debug.Log("383\n" + direction);
        (int x, int y) = directions[direction];
        // Debug.Log("385\n" + actualPoint.getRow() + " " + actualPoint.getColumn());
        int valueOfPointedPoint = tableList[actualPoint.getRow() + 2 * x, actualPoint.getColumn() + 2 * y];
        if (valueOfPointedPoint != actualPoint.getValue())
        {
            Node<Point> nodeToMove = FindBasicPointValue(Math.Min(valueOfPointedPoint, actualPoint.getValue()), actualPoint, direction, basicPointTable);
            tableList = MoveNodeToOtherBasicPointAndUpdateTableList(nodeToMove, basicPointTable, Math.Max(valueOfPointedPoint, actualPoint.getValue()), tableList);
        }
        tableList[actualPoint.getRow() + x, actualPoint.getColumn() + y] = Math.Max(valueOfPointedPoint, actualPoint.getValue());
        return tableList;
    }

    private int[,] BuildListOfZeroAndIncreasingNumber(int size)
    {
        int[,] table = new int[size + 3 - size % 2, size + 3 - size % 2];
        int number = 1;
        table[1, 0] = number + 1;
        for (int i = 0; i < size + 2; i++)
        {
            for (int j = 0; j < size + 2; j++)
            {
                if (table[i, j] == 0)
                {
                    int value = 0;
                    if (i % 2 == 1 && j % 2 == 1)
                    {
                        number++;
                        value = number;
                    }
                    table[i, j] = value;
                }
            }
        }
        table[size, size + 1] = number;
        // Debug.Log("TABLE \n" + stringList(table));
        return table;
    }

    private BasicPoint InstantiateDefaultBasicPointTable(int size) // put first and last basicPointsNodes to the adjacent BasicPoints.
    {
        BasicPoint firstBasicPointTable = new BasicPoint(new Node<Point>(new Point(0, 0))); // new BasicPoint(new Node<Point>(new Point(1, 0, 1)))
        BasicPoint basicPointTable = firstBasicPointTable;
        int number = 1;
        for (int i = 0; i < size + 2; i++)
        {
            for (int j = 0; j < size + 2; j++)
            {
                if (i % 2 == 1 && j % 2 == 1)
                {
                    number++;
                    basicPointTable.setNext(new BasicPoint(new Node<Point>(new Point(i, j, number))));
                    basicPointTable = basicPointTable.getNext();
                }
            }
        }
        // if (firstBasicPointTable.getNext() != null)
        // {
        //     firstBasicPointTable.getNext().getValue().setNext(new Node<Point>(new Point(1, 0, 2))); // putting in the first BasicPoint another node
        // }
        // basicPointTable.getValue().setNext(new Node<Point>(new Point(size, size + 1, number)));
        Debug.Log("252\n" + StringBasicPoints(firstBasicPointTable));
        return firstBasicPointTable.getNext();
    }

    private int[,] MoveNodeToOtherBasicPointAndUpdateTableList(Node<Point> nodeToMove, BasicPoint basicPointTable, int valueOfBasicPoint, int[,] tableList)
    {
        BasicPoint actualBasicPoint = basicPointTable;
        while (actualBasicPoint != null)
        {
            if (actualBasicPoint.getValue().getValue().getValue() == valueOfBasicPoint)
            {
                break;
            }
            actualBasicPoint = actualBasicPoint.getNext();
        }
        Debug.Log("407\nTest");
        Debug.Log(StringNodes(nodeToMove));
        if (nodeToMove == null)
        {
            Debug.Log("409\n" + "------------------------------------------");
        }
        Node<Point> actualNode = FindFirstNodeOfBasicPoint(nodeToMove.getValue().getValue(), basicPointTable);
        nodeToMove = actualNode;
        Debug.Log("421\nactualNode:\n" + StringNodes(actualNode));
        //tableList[nodeToMove.getValue().getRow(), nodeToMove.getValue().getColumn()] = valueOfBasicPoint;
        int count = 10000;
        if (actualNode != null) // go over all the nodes that are in his basicPoint
        {
            while (actualNode.getNext() != null && count > 0)
            {
                tableList[actualNode.getValue().getRow(), actualNode.getValue().getColumn()] = valueOfBasicPoint;
                actualNode.getValue().setValue(valueOfBasicPoint);
                actualNode = actualNode.getNext();
                count--;
            }
            tableList[actualNode.getValue().getRow(), actualNode.getValue().getColumn()] = valueOfBasicPoint;
            actualNode.getValue().setValue(valueOfBasicPoint);
        }
        actualNode.setNext(actualBasicPoint.getValue());
        actualBasicPoint.setValue(nodeToMove);
        // nodeToMove.getValue().setValue(valueOfBasicPoint);
        actualBasicPoint = basicPointTable;
        if (actualBasicPoint.getValue() == nodeToMove)
        {
            // basicPointTable = basicPointTable.getNext(); // I think that it won't work because it changes only the reference of our basicPointTable and let the first "node" intact...
            basicPointTable.setValue(basicPointTable.getNext().getValue());
            basicPointTable.setNext(basicPointTable.getNext().getNext());
        }
        else
        {
            while (actualBasicPoint.getNext() != null)  // the basicPoint that had to stay is gone, and the one that had to go is still here when we talk about the first basicPoint in our list of basicPoints.
            {   // can be optimised by using getNext().
                if (actualBasicPoint.getNext().getValue() == nodeToMove)
                {
                    actualBasicPoint.setNext(actualBasicPoint.getNext().getNext());
                    break;
                }
                actualBasicPoint = actualBasicPoint.getNext();
            }
        }
        // if (actualBasicPoint.getNext() == null && basicPointTable.getValue() == nodeToMove)
        // {
        //     basicPointTable = basicPointTable.getNext();
        // }
        return tableList;
    }

    private Node<Point> FindFirstNodeOfBasicPoint(int value, BasicPoint basicPointTable)
    {
        BasicPoint actualBasicPoint = basicPointTable;
        while (actualBasicPoint != null)
        {
            if (actualBasicPoint.getValue().getValue().getValue() == value)
            {
                return actualBasicPoint.getValue();
            }
            actualBasicPoint = actualBasicPoint.getNext();
        }
        Debug.Log("457\n" + value + "\nbasicPointTable:\n" + basicPointTable);
        return null;
    }

    private Node<Point> FindBasicPointValue(int pointValue, Point actualPoint, int direction, BasicPoint basicPointTable)
    {
        if (pointValue == actualPoint.getValue())
        {
            int row = actualPoint.getRow();
            int column = actualPoint.getColumn();
            return FindPointNode(pointValue, row, column, basicPointTable);
        }
        (int x, int y) = directions[direction];
        return FindPointNode(pointValue, actualPoint.getRow() + 2 * x, actualPoint.getColumn() + 2 * y, basicPointTable);
    }

    private Node<Point> FindPointNode(int value, int row, int column, BasicPoint basicPointTable)
    {
        BasicPoint actualBasicPoint = basicPointTable;
        while (actualBasicPoint != null)
        {
            if (actualBasicPoint.getValue().getValue().getValue() == value)
            {
                Node<Point> actualNodePoint = actualBasicPoint.getValue();
                while (actualNodePoint != null)
                {
                    if (actualNodePoint.getValue().getRow() == row && actualNodePoint.getValue().getColumn() == column)
                    {
                        return actualNodePoint;
                    }
                    actualNodePoint = actualNodePoint.getNext();
                }
            }
            actualBasicPoint = actualBasicPoint.getNext();
        }
        Debug.Log("474\n" + "There is no appropriate Node. Value = " + value + " [" + row + "," + column + "]" + StringBasicPoints(basicPointTable));
        return null;
    }

    private int CountNumberOfBasicPoints(BasicPoint firstBasicPoint)
    {
        BasicPoint actualBasicPoint = firstBasicPoint;
        int countBasicPoints = 0;
        while (actualBasicPoint != null)
        {
            countBasicPoints++;
            actualBasicPoint = actualBasicPoint.getNext();
        }
        return countBasicPoints;
    }

    private int FindBiggestValueOfBasicPoint(BasicPoint firstBasicPoint)
    {
        int biggestValue = 0;
        while (firstBasicPoint != null)
        {
            biggestValue = Math.Max(biggestValue, firstBasicPoint.getValue().getValue().getValue());
            firstBasicPoint = firstBasicPoint.getNext();
        }
        return biggestValue;
    }

    private bool IsDone(BasicPoint firstBasicPoint, int biggestBasicPointValue) // isTheTableComlete
    {
        if (firstBasicPoint == null)
        {
            return false;
        }
        if (firstBasicPoint.getNext() != null || firstBasicPoint.getValue().getValue().getValue() < biggestBasicPointValue)
        {
            return false;
        }
        Debug.Log("THE END - 538\nfirstBasicPoint:\n" + StringBasicPoints(firstBasicPoint));
        return true;
    }

    private int RandomDirectionToRemovingWall(Point actualPoint, int[,] table)
    {
        int actualDirection = UnityEngine.Random.Range(0, 23);
        for (int i = 0; i < 4; i++)
        {
            if (IsFreeToBreakWall(actualPoint, table, direction[actualDirection, i]))
            {
                return direction[actualDirection, i];
            }
        }
        Debug.Log("//////////////////////////////////////////////////////////////////////////////////////////");
        return -1;
        // while (!isDone) // make a table of 24 table of possibilities = table of 24 X 4. then coose a random list. and then do a for (4). 
        // {
        //     direction = UnityEngine.Random.Range(0, 3);
        //     if (IsFreeToBreakWall(actualPoint, table, direction))
        //     {
        //         isDone = true;
        //     }
        // } 
        // if there is no directon possible, return -1.
        // return direction; // add verification that there is another BasicPoint in this direction.

        // int direction = 0;
        // while (direction < 3)
        // {
        //     int rnd = UnityEngine.Random.Range(0, 1000);
        //     if (rnd > DateTime.Now.Millisecond)
        //     {
        //         direction++;
        //     }
        // }
        // return direction;
    }

    private Point RandomPointFromBasicPoint(BasicPoint basicPointTable)
    {
        int iterations = UnityEngine.Random.Range(0, CountNumberOfBasicPoints(basicPointTable)); // random range is inclusif for the small number and exclusif for the big number.
        BasicPoint actualBasicPoint = basicPointTable;
        Debug.Log("551\niterations: " + iterations + "\nbasicPointTable:\n" + StringBasicPoints(basicPointTable)); // maxBasicPoint: " + maxBasicPoint + "\n
        for (int i = 0; i < iterations; i++)
        {
            actualBasicPoint = actualBasicPoint.getNext();
        }
        if (actualBasicPoint == null)
        {
            Debug.Log("558\nbasicPointTable:\n" + StringBasicPoints(basicPointTable));
        }
        return RandomPointFromNodePoint(actualBasicPoint.getValue());  // return actualBasicPoint.getValue().getValue(); 
    }

    private bool IsFreeToBreakWall(Point actualPoint, int[,] table, int actualDirection)
    {
        (int x, int y) = directions[actualDirection];
        if (IsOutOfBounds(actualPoint.getRow() + 2 * x, actualPoint.getColumn() + 2 * y))
        {
            int rowX = actualPoint.getRow() + 2 * x;
            int columnY = actualPoint.getColumn() + 2 * y;
            Debug.Log("///////////////////////////////////////////////////////+++++++++++++++++++++++++++++++++++++++++" + rowX + "." + columnY);
            return false;
        }
        //if (table[actualPoint.getRow() + x, actualPoint.getColumn() + y] != 0)
        //{
        //    return false;
        //}
        if ((actualPoint.getRow() == 1 && actualPoint.getColumn() == 0 && actualDirection != 2) || (actualPoint.getRow() == size && actualPoint.getColumn() == size + 1 && actualDirection != 0))
        {
            return false;
            Debug.Log("///////////////////////////////////////////////////////-----------------------------------------");
        }
        return true;
    }

    private Point RandomPointFromNodePoint(Node<Point> firstNodePoint) // add something if there is no Node (if firstNodePoint = null).
    {
        int numOfNodes = CountNumberOfNodes(firstNodePoint);
        if (numOfNodes == 1)
        {
            return firstNodePoint.getValue();
        }
        Node<Point> actualNode = firstNodePoint;
        int iteration = UnityEngine.Random.Range(0, numOfNodes - 1);
        for (int i = 0; i < iteration; i++)
        {
            actualNode = actualNode.getNext();
        }
        return actualNode.getValue();
    }

    private int CountNumberOfNodes(Node<Point> firstNode)
    {
        Node<Point> actualNode = firstNode;
        int count = 0;
        while (actualNode != null)
        {
            count++;
            actualNode = actualNode.getNext();
        }
        return count;
    }

    // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// printing methodes

    private string StringList(int[,] table)
    {
        string str = "[";
        for (int row = 0; row < table.GetLength(0); row++)
        {
            for (int column = 0; column < table.GetLength(0); column++)
            {
                if (column != table.GetLength(0) - 1)
                {
                    str += table[row, column] + "   ,   ";
                }
                else
                {
                    str += table[row, column];
                }
            }
            if (row != table.GetLength(0) - 1)
            {
                str += "\n";
            }
        }
        str += "]";
        return str;
    }

    private string StringNodes(Node<Point> firstNode)
    {
        Node<Point> actualNode = firstNode;
        string str = "||";
        while (actualNode != null)
        {
            str += " " + actualNode.getValue().toString() + " |";
            actualNode = actualNode.getNext();
        }
        str += "|";
        // Debug.Log(str);
        return str;
    }

    private string StringBasicPoints(BasicPoint firstBasicPoint)
    {
        BasicPoint actualBasicPoint = firstBasicPoint;
        string str = "|-|";
        while (actualBasicPoint != null)
        {
            str += "\n" + StringNodes(actualBasicPoint.getValue());
            actualBasicPoint = actualBasicPoint.getNext();
        }
        str += "\n|-|";
        // Debug.Log(str);
        return str;
    }

    // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////instantiating the maze in Unity. Already in previous commit
    private void InstantiateMaze(int[,] table)
    {
        // (j - 1) -> left [=0]. (j + 1) -> right [=2]. (i - 1) -> top [=1]. (i + 1) -> bottom [=3].


        for (int row = 0; row < table.GetLength(0); row++)
        {
            for (int column = 0; column < table.GetLength(0); column++)
            {
                if (table[row, column] == 0) { continue; }

                int[] num_of_item = new int[4] { 1, 1, 1, 1 };
                for (int i = 0; i < directions.Length; i++)
                {
                    (int x, int y) = directions[i];
                    if (!IsOutOfBounds(row + x, column + y) && table[row + x, column + y] != 0)
                    {
                        num_of_item[i] = 0;
                    }
                }
                InstantiateSquare(row, column, table.GetLength(0), num_of_item);
            }
        }
    }

    private void InstantiateSquare(int row, int column, int size, int[] walls)
    {
        // string debug_print = "";
        int num_of_walls = 0;
        int adjacent_walls = 0;
        int prefab_place = 0;
        int rotation = 0;
        for (int i = 0; i < walls.Length; i++)
        {
            num_of_walls += walls[i];
            if ((i == 0 && walls[i] == 1 && walls[3] == 1) || (i > 0 && walls[i] == 1 && walls[i - 1] == 1))
            {
                adjacent_walls++;
            }
        }
        if (num_of_walls == 1)
        {
            prefab_place = 0;
            foreach (int possible_wall in walls)
            {
                if (possible_wall == 1)
                {
                    break;
                }
                rotation += 90;
            }
            // debug_print = "1_Wall. Rotation: ";
        }
        else if (num_of_walls == 2)
        {
            if (adjacent_walls == 0)
            {
                prefab_place = 2;
                if (walls[0] == 0)
                {
                    rotation = 90;
                }
                // debug_print = "2_Walls_Opposite. Rotation: ";
            }
            else
            {
                int zeros_before_first_wall = 0;
                prefab_place = 1;
                foreach (int item in walls)
                {
                    if (item == 0)
                    {
                        zeros_before_first_wall += 1;
                    }
                    else
                    {
                        if (zeros_before_first_wall == 0)
                        {
                            rotation = 180;
                            if (walls[3] == 1)
                            {
                                rotation -= 90;
                            }
                            break;
                        }
                        else
                        {
                            rotation = 360 - (2 - zeros_before_first_wall) * 90;
                            break;
                        }
                    }
                }
                // debug_print = "2_Walls_Adjacent. Rotation: ";
            }
        }
        else if (num_of_walls == 3)
        {
            prefab_place = 3;
            rotation = -90;
            foreach (int item in walls)
            {
                if (item == 0)
                {
                    break;
                }
                rotation += 90;
            }
            // debug_print = "3_Walls. Rotation: ";
        }
        else
        {
            prefab_place = 4;
            // debug_print = "wall. Rotation: ";
        }
        int initial_place = ((-size / 2) * square_size) + (square_size / 2) - ((square_size / 2) * (size % 2));
        int place_x = initial_place + row * square_size;  // vertical = x
        int place_z = initial_place + column * square_size;  // horizontale = z
        // Debug.Log(debug_print + rotation.ToString());
        GameObject square = Instantiate(squares[prefab_place], new Vector3(place_x, 0, place_z), Quaternion.identity);
        square.transform.SetParent(GameObject.FindWithTag("Map").transform);
        square.transform.Rotate(0, rotation, 0);
        // square.transform.localRotation = new Quaternion(0, rotation, 0, 0);
    }
}
