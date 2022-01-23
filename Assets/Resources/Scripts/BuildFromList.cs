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

    private readonly int[,] direction = new int[24, 4] {
        {0, 1, 2, 3 }, {0, 1, 3, 2 }, {0, 2, 1, 3 }, {0, 2, 3, 1 }, {0, 3, 2, 1 }, {0, 3, 1, 2 },
        {1, 0, 2, 3 }, {1, 0, 3, 2 }, {1, 2, 0, 3 }, {1, 2, 3, 0 }, {1, 3, 2, 0 }, {1, 3, 0, 2 },
        {2, 0, 1, 3 }, {2, 0, 3, 1 }, {2, 1, 3, 0 }, {2, 1, 0, 3 }, {2, 3, 0, 1 }, {2, 3, 1, 0 },
        {3, 0, 2, 1 }, {3, 0, 1, 2 }, {3, 1, 2, 0 }, {3, 1, 0, 2 }, {3, 2, 1, 0 }, {3, 2, 0, 1 },
    };

    public GameObject[] squares;

    void Start()
    {
        int seed = (DateTime.Now.Millisecond + 10) * (DateTime.Now.Second + 10);
        UnityEngine.Random.seed = seed;
        Debug.Log(seed); //Debug.Log("ActualTime " + DateTime.Now.Second.ToString() + " " + DateTime.Now.Millisecond.ToString());

        int[,] tableTryThree = CreateMazeListThree();
        InstantiateMaze(tableTryThree);
    }

    private bool IsOutOfBounds(int row, int column)
    {
        return row > size + 1
            || row < 0
            || column > size + 1
            || column < 0;
    }

    private int[,] BuildListOfZeroAndOne(int size)
    {
        int[,] table = new int[size + 3 - size % 2, size + 3 - size % 2];
        int number = 1;
        for (int i = 0; i < size + 2; i++)
        {
            for (int j = 0; j < size + 2; j++)
            {
                if (i % 2 == 1 && j % 2 == 1)
                {
                    table[i, j] = number;
                }
            }
        }
        table[1, 0] = number;
        table[size, size + 1] = number;
        return table;
    }

    private PointThree[,] BuildListOfPointThrees(int size)
    {
        int tableLength = (int)Math.Ceiling(size / 2.0);
        PointThree[,] table = new PointThree[tableLength, tableLength];
        for (int i = 0; i < tableLength; i++)
        {
            for (int j = 0; j < tableLength; j++)
            {
                if (i == 0 || j == 0 || i == tableLength - 1 || j == tableLength - 1)
                {
                    table[i, j] = new PointThree(new bool[4] { j != 0, i != 0, j != tableLength - 1, i != tableLength - 1}, false, i * tableLength + j + 1);
                }
                else
                {
                    table[i, j] = new PointThree(i * tableLength + j + 1);
                }
            }
        }
        // Debug.Log(StringPointThree(table));
        return table;
    }

    // private string StringPointThree(PointThree[,] table)
    // {
    //     int tableLength = (int)Math.Ceiling(size / 2.0);
    //     string str = "|";
    //     for (int i = 0; i < tableLength; i++)
    //     {
    //         for (int j = 0; j < tableLength; j++)
    //         {
    //             str += "| " + table[i, j].toString() + " ";
    //         }
    //         str += "\n";
    //     }
    //     str += "|";
    //     return str;
    // }

    private int ChoosePointThreeDirection(PointThree actualPoint, PointThree[,] tablePointThree, int row, int column) // remove : PointThree actualPoint, . Chage it to bool[] pointDirections
    {
        int directionTable = UnityEngine.Random.Range(0, 23);
        bool[] pointDirections = actualPoint.getDirections(); // remove that
        for (int i = 0; i < 4; i++)
        {
            if (pointDirections[direction[directionTable, i]])
            {
                (int x, int y) = directions[direction[directionTable, i]];
                if (tablePointThree[row + x, column + y].canConnect())
                {
                    return direction[directionTable, i];
                }
            }
        }
        return -1;
    }

    private int NumberOfNewConnections(PointThree[,] tablePointThree, int actualDirection, int row, int column)
    {
        int count = 0;
        if (!tablePointThree[row, column].getConnection())
        {
            count++;
        }
        (int x, int y) = directions[actualDirection];
        if (!tablePointThree[row + x, column + y].getConnection())
        {
            count++;
        }
        return count;
    }

    private int InverseDirection(int actualDirection)
    {
        if (actualDirection % 2 == 0)
        {
            if (actualDirection == 0)
            {
                return 2;
            }
            return 0;
        }
        if (actualDirection == 1)
        {
            return 3;
        }
        return 1;
    }

    // public Compartiment FindLastCompartiment(Compartiment actualCompartiment)
    // {
    //     int count = 20;
    //     while (actualCompartiment.getNext() != null && count > 0)
    //     {
    //         actualCompartiment = actualCompartiment.getNext();
    //         count--;
    //     }
    //     return actualCompartiment;
    // }
       
    // public Compartiment FindSmallerCompatimentThan(Compartiment actualCompartiment, int compartimentBiggestValue)
    // {
    //     int count = 20;
    //     while (actualCompartiment.getNext() != null && actualCompartiment.getNext().getValue() < compartimentBiggestValue && count > 0)
    //     {
    //         actualCompartiment = actualCompartiment.getNext();
    //         count--;
    //     }
    //     return actualCompartiment;
    // }

    public void ConnectCompartiments(Compartiment first, Compartiment second)
    {
        if (first.getValue() > second.getValue())
        {
            first.addCount(second.getCount());
            second.setNext(first);
        }
        else
        {
            if (second.getValue() > first.getValue())
            {
                second.addCount(first.getCount());
                first.setNext(second);
            }
        }
    }

    public Compartiment ConnectToLastCompartimentOf(Compartiment actualCompartiment, int count)
    {
        if (actualCompartiment.getNext() != null)
        {
            Compartiment lastCompartiment = ConnectToLastCompartimentOf(actualCompartiment.getNext(), actualCompartiment.getCount());
            actualCompartiment.setNext(lastCompartiment);
            actualCompartiment.addCount(-count);
            return lastCompartiment;
        }
        return actualCompartiment;
    }

    public void UpdatePointThreeCompartiment(PointThree actualPoint, PointThree pointedPoint, PointThree[,] tablePointThree)
    {
        int biggestCompartimentValue = Math.Max(actualPoint.getCompartiment().getValue(), pointedPoint.getCompartiment().getValue());
        if (actualPoint.getCompartiment().getValue() == biggestCompartimentValue)
        {
            PointThree actualPointThree = actualPoint;
            actualPoint = pointedPoint;
            pointedPoint = actualPointThree;
        }
        if (!actualPoint.getConnection() && !pointedPoint.getConnection())
        {
            actualPoint.setCompartiment(pointedPoint.getCompartiment());
            pointedPoint.getCompartiment().addCount(1);
        }
        else
        {
            Compartiment actualPointLastCompartiment = ConnectToLastCompartimentOf(actualPoint.getCompartiment(), 0);
            Compartiment pointedPointLastCompartiment = ConnectToLastCompartimentOf(pointedPoint.getCompartiment(), 0);
            ConnectCompartiments(actualPointLastCompartiment, pointedPointLastCompartiment);
            
            // Code before I used a recursive method:

            //// Debug.Log("228\ntablePointThree:\n" + StringPointThree(tablePointThree) + "\n\n\nactualPoint: " + actualPoint.toString() + "\npointedPoint: " + pointedPoint.toString());
            // int countToAdd = actualPoint.getCompartiment().getCount();
            // find the # (the compartiment that is the biggest in the list of the smallest but is smaller than pointedPoint.getCompartiment(). if pointedPoint.getCompartiment() is the biggest, put the last compartiment next to it. and # will be  pointedPoint.getCompartiment()
            // // Compartiment pointedPointLastCompartiment = FindLastCompartiment(pointedPoint.getCompartiment());
            // // if (actualPoint.getCompartiment().getNext() == null) 
            // // {
            // //     
            // //     actualPoint.getCompartiment().setNext(pointedPointLastCompartiment);
            // //     pointedPointLastCompartiment.addCount(countToAdd);
            // // }
            // // else
            // // {
                // // Compartiment smallerThanPointedPointCompartiment = FindSmallerCompatimentThan(actualPoint.getCompartiment(), pointedPointLastCompartiment.getValue()); // pointedPoint.getCompartiment()
                // // if (smallerThanPointedPointCompartiment.getNext() != null) // there is a bigger Compartiment than pointedPoint.getCompartiment()
                // // {
                // //     if (FindLastCompartiment(smallerThanPointedPointCompartiment).getValue() != pointedPointLastCompartiment.getValue())
                // //     {
                // //         //// Debug.Log("241----------------------------------------------------------------");
                // //         // Compartiment pointedPointSmallerThanBiggestActualPointCompartiment = FindSmallerCompatimentThan(pointedPoint.getCompartiment(), pointedPoint.getCompartiment().getValue()); // enlever count from compartiments that are after pointedPoint compartiment
                // //         countToAdd = pointedPointLastCompartiment.getCount(); //pointedPoint.getCompartiment()
                // //         smallerThanPointedPointCompartiment = FindLastCompartiment(smallerThanPointedPointCompartiment);
                // //         pointedPointLastCompartiment.setNext(smallerThanPointedPointCompartiment); // pointedPoint.getCompartiment()
                // //         // actualPoint.getCompartiment().setNext(smallerThanPointedPointCompartiment);
                // //         smallerThanPointedPointCompartiment.addCount(countToAdd);
                // //     }
                // // }
                // // else // pointedPoint.getCompartiment() is the biggest compartiment
                // // {
                // //     countToAdd = smallerThanPointedPointCompartiment.getCount(); // +
                // //     smallerThanPointedPointCompartiment.setNext(pointedPointLastCompartiment); // pointedPoint.getCompartiment()
                // //     // actualPoint.getCompartiment().setNext(pointedPoint.getCompartiment());
                // //     pointedPointLastCompartiment.addCount(countToAdd); // pointedPoint.getCompartiment()
                // // }
            // // }
            //// Debug.Log("255\ntablePointThree:\n" + StringPointThree(tablePointThree));
        }
        // #.getCompartiment().addCount(actualPoint.getCompartiment().getCount());
        // actualPoint.setCompartiment(#);
        // Debug.Log("204\n" + StringPointThree(tablePointThree) + "\n\n\n" + actualPoint.toString() + "\n\n\n" + pointedPoint.toString());
    }

    private PointThree[,] UpdatePointThreeTable(PointThree[,] tablePointThree, int actualDirection, int row, int column)
    {
        PointThree actualPoint = tablePointThree[row, column];
        (int x, int y) = directions[actualDirection];
        PointThree pointedPoint = tablePointThree[row + x, column + y];
        UpdatePointThreeCompartiment(actualPoint, pointedPoint, tablePointThree);
        actualPoint.setConnection();
        pointedPoint.setConnection();
        actualPoint.setDirections(actualDirection);
        pointedPoint.setDirections(InverseDirection(actualDirection)); // 1 <--> 3 ; 0 <--> 2 // make it better
        return tablePointThree;
    }

    private int[,] UpdateTableThree(int[,] tableTryThree, int actualDirection, int row, int column)
    {
        row = row * 2 + 1;
        column = column * 2 + 1;
        tableTryThree[row, column] = 1;
        (int x, int y) = directions[actualDirection];
        tableTryThree[row + x, column + y] = 1;
        tableTryThree[row + 2 * x, column + 2 * y] = 1;
        return tableTryThree;
    }

    private int[,] CreateMazeListThree()
    {
        int[,] tableTryThree = BuildListOfZeroAndOne(size);
        PointThree[,] tablePointThree = BuildListOfPointThrees(size);
        int length = tablePointThree.GetLength(0);
        int remainingConnections = (int)Math.Pow(length, 2);
        // int fiftyPercentConnections = (int)Math.Pow(length, 2) / 2; // if 100, go out of bound at 219
        int PointThreeRow = UnityEngine.Random.Range(0, length);
        int PointThreeColumn = UnityEngine.Random.Range(0, length);
        int actualDirection = 0;
        Compartiment lastCompartiment = tablePointThree[length - 1, length - 1].getCompartiment();
        int numOfCompartiments = lastCompartiment.getValue();
        
        // connect every point one or two times
        tableTryThree = ConnectEveryPoint(tableTryThree, tablePointThree);
        // Debug.Log("261\ntablePointThree:\n" + StringPointThree(tablePointThree));
        
        while (!lastCompartiment.isEqual())
        {
            //# random pointThree (position in the table)
            actualDirection = -1;
            while (actualDirection == -1)
            {
                PointThreeRow = UnityEngine.Random.Range(0, length);
                PointThreeColumn = UnityEngine.Random.Range(0, length);
                if (!tablePointThree[PointThreeRow, PointThreeColumn].canConnect())
                {
                    actualDirection = -1;
                }
                else
                {
                    //# choose direction
                    actualDirection = ChoosePointThreeDirection(tablePointThree[PointThreeRow, PointThreeColumn], tablePointThree, PointThreeRow, PointThreeColumn); 
                }
            }
            //# remainingConnextions --; PointThree.setConnection(); two time, if the boolean (connexion) of the PointThree is true.  
            remainingConnections -= NumberOfNewConnections(tablePointThree, actualDirection, PointThreeRow, PointThreeColumn);
            //# change tablePointThree - change the two points direction table 
            tablePointThree = UpdatePointThreeTable(tablePointThree, actualDirection, PointThreeRow, PointThreeColumn);
            //# change tableTryThree
            tableTryThree = UpdateTableThree(tableTryThree, actualDirection, PointThreeRow, PointThreeColumn);
        }
    
        // Node<PointThree> listNotConnected = new Node<PointThree>(new PointThree(0));
        // tableTryThree = PointThreeNotConnected(tableTryThree, tablePointThree, listNotConnected);
        // listNotConnected = listNotConnected.getNext();
        // Debug.Log("261\ntablePointThree:\n" + StringPointThree(tablePointThree)); // + "\nlistNotConnected: " + StringNodes(listNotConnected));
        return tableTryThree;
    }

    public int[,] ConnectEveryPoint(int[,] table, PointThree[,] tablePointThree)
    {
        int tableLength = tablePointThree.GetLength(0);
        int actualDirection = 0;
        for (int row = 0; row < tableLength; row++)
        {
            for (int column = 0; column < tableLength; column++)
            {
                if (!tablePointThree[row, column].getConnection())
                {
                    actualDirection = ChoosePointThreeDirection(tablePointThree[row, column], tablePointThree, row, column);
                    tablePointThree = UpdatePointThreeTable(tablePointThree, actualDirection, row, column);
                    table = UpdateTableThree(table, actualDirection, row, column);
                }
            }
        }
        return table;
    }

    //public int[,] PointThreeNotConnected(int[,] table, PointThree[,] tablePointThree, Node<PointThree> first)
    //{
    //    Node<PointThree> listNotConnected = first;
    //    int tableLength = tablePointThree.GetLength(0);
    //    int actualDirection = 0;
    //    for (int row = 0; row < tableLength; row++)
    //    {
    //        for (int column = 0; column < tableLength; column++)
    //        {
    //            if (!tablePointThree[row, column].getConnection())
    //            {
    //                if (UnityEngine.Random.Range(1, 100) > 0) // for the testing part, it is always true.
    //                {
    //                    actualDirection = ChoosePointThreeDirection(tablePointThree[row, column], tablePointThree, row, column);
    //                    tablePointThree = UpdatePointThreeTable(tablePointThree, actualDirection, row, column);
    //                    table = UpdateTableThree(table, actualDirection, row, column);
    //                }
    //                else
    //                {
    //                    listNotConnected.setNext(new Node<PointThree>(tablePointThree[row, column]));
    //                    listNotConnected = listNotConnected.getNext();
    //                }
    //            }
    //        }
    //    }
    //    return table;
    //}

    // private string StringNodes(Node<PointThree> firstNode)
    // {
    //     if (firstNode != null)
    //     {
    //         return "null";
    //     }
    //     Node<PointThree> actualNode = firstNode;
    //     string str = "||";
    //     while (actualNode != null)
    //     {
    //         str += " " + actualNode.getValue().toString() + " |";
    //         actualNode = actualNode.getNext();
    //     }
    //     str += "|";
    //     return str;
    // }
       
    // private string StringList(int[,] table)
    // {
    //     string str = "[";
    //     for (int row = 0; row < table.GetLength(0); row++)
    //     {
    //         for (int column = 0; column < table.GetLength(0); column++)
    //         {
    //             if (column != table.GetLength(0) - 1)
    //             {
    //                 str += table[row, column] + "   ,   ";
    //             }
    //             else
    //             {
    //                 str += table[row, column];
    //             }
    //         }
    //         if (row != table.GetLength(0) - 1)
    //         {
    //             str += "\n";
    //         }
    //     }
    //     str += "]";
    //     return str;
    // }

    // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// instantiating the maze in Unity. Already in previous commits
    private void InstantiateMaze(int[,] table)
    {
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
        }
        else
        {
            prefab_place = 4;
        }
        int initial_place = ((-size / 2) * square_size) + (square_size / 2) - ((square_size / 2) * (size % 2));
        int place_x = initial_place + row * square_size;  // vertical = x
        int place_z = initial_place + column * square_size;  // horizontale = z
        GameObject square = Instantiate(squares[prefab_place], new Vector3(place_x, 0, place_z), Quaternion.identity);
        square.transform.SetParent(GameObject.FindWithTag("Map").transform);
        square.transform.Rotate(0, rotation, 0);
    }
}
