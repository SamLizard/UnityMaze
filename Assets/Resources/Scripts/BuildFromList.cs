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
        // int[,] tableTryThree = BuildListOfZeroAndOne(size);                          // 1. already in 3.
        // PointThree[,] tablePointThree = BuildListOfPointThrees(size);                // 2. already in 3.
        int[,] tableTryThree = CreateMazeListThree();                                   // 3.
        InstantiateMaze(tableTryThree);                                                 // 4.
        //# build list of zero and one (table[,]) // became 1
        //# build list in two dimensions of something that contain the possible directions (list of 5 booleans - the fifth is to know if there is at least one direction that is true), and a boolean (did the point already made a connexion?). // became 2.
        //# create maze (make a count of the number of points that didn't already made a connexion (that stops running when it is equal to 0). // became 3.
        //# print maze      // it is 4.
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
        table[1, 0] = number;
        table[size, size + 1] = number;
        for (int i = 0; i < size + 2; i++)
        {
            for (int j = 0; j < size + 2; j++)
            {
                if (table[i, j] == 0)
                {
                    int value = 0;
                    if (i % 2 == 1 && j % 2 == 1)
                    {
                        value = number;
                    }
                    table[i, j] = value;
                }
            }
        }
        return table;
    }

    private PointThree[,] BuildListOfPointThrees(int size)
    {
        // 5 -> 3; 7 -> 4; 9 -> 5
        int tableLength = (int)Math.Ceiling(size / 2.0);
        PointThree[,] table = new PointThree[tableLength, tableLength];
        for (int i = 0; i < tableLength; i++)
        {
            for (int j = 0; j < tableLength; j++)
            {
                if (i == 0 || j == 0 || i == tableLength - 1 || j == tableLength - 1)
                {
                    table[i, j] = new PointThree(new bool[5] { j != 0, i != 0, j != tableLength - 1, i != tableLength - 1, true }); // MakeDirectionsThree(i, j, tableLength)
                }
                else
                {
                    table[i, j] = new PointThree();
                }
            }
        }
        Debug.Log(StringPointThree(table));
        return table;
    }

    private string StringPointThree(PointThree[,] table)
    {
        int tableLength = (int)Math.Ceiling(size / 2.0);
        string str = "|";
        for (int i = 0; i < tableLength; i++)
        {
            for (int j = 0; j < tableLength; j++)
            {
                str += "| " + table[i, j].toString() + " ";
            }
            str += "\n";
        }
        str += "|";
        return str;
    }

    private int ChoosePointThreeDirection(PointThree actualPoint) //, int row, int column, PointThree[,] tablePointThree, int[,] tableTryThree, int remainingConnexions
    {
        int directionTable = UnityEngine.Random.Range(0, 23);
        bool[] pointDirections = actualPoint.getDirections();
        for (int i = 0; i < 4; i++)
        {
            if (pointDirections[direction[directionTable, i]])
            {
                return direction[directionTable, i];
            }
        }
        // Debug.Log("298\n" + actualPoint.toString()); // + "\nrow: " + row + ". Column: " + column + "\ntableTryThree: " + StringList(tableTryThree) + "\ntablePointThree:\n" + StringPointThree(tablePointThree) + "\nremainingConnections: " + remainingConnexions);
        return -1; // it never has to come here. check before if the fifth boolean in the pointThree directions table is true.
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
        // Debug.Log("315\nactualPoint: " + tablePointThree[row, column].toString() + "\npointedPoint: " + tablePointThree[row + x, column + y].toString() + "\ncount: " + count);
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

    private PointThree[,] UpdatePointThreeTable(PointThree[,] tablePointThree, int actualDirection, int row, int column)
    {
        PointThree actualPoint = tablePointThree[row, column];
        (int x, int y) = directions[actualDirection];
        PointThree pointedPoint = tablePointThree[row + x, column + y];
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
        // int count = 15; // just for the testing part
        int PointThreeRow = UnityEngine.Random.Range(0, length);
        int PointThreeColumn = UnityEngine.Random.Range(0, length);
        int actualDirection = 0;
        while (remainingConnections > 0) // && count > 0
        {
            // count--;
            //# random pointThree (position in the table)
            while (!tablePointThree[PointThreeRow, PointThreeColumn].getLastDirection()) // && count > 0
            {
                // count--;
                PointThreeRow = UnityEngine.Random.Range(0, length);
                PointThreeColumn = UnityEngine.Random.Range(0, length);
                // Debug.Log("376\nPointThreeRow: " + PointThreeRow + "\nPointThreeColumn: " + PointThreeColumn); // + "\nCount: " + count
            }
            // Debug.Log("301\nRow: " + PointThreeRow + "\nColumn: " + PointThreeColumn + "\nPointThree: " + tablePointThree[PointThreeRow, PointThreeColumn].toString());

            //# choose direction
            actualDirection = ChoosePointThreeDirection(tablePointThree[PointThreeRow, PointThreeColumn]); //, PointThreeRow, PointThreeColumn, tablePointThree, tableTryThree, remainingConnections
            //# remainingConnextions --; PointThree.setConnection(); two time, if the boolean (connexion) of the PointThree is true.  
            remainingConnections -= NumberOfNewConnections(tablePointThree, actualDirection, PointThreeRow, PointThreeColumn);
            //# change tablePointThree - change the two points direction table 
            tablePointThree = UpdatePointThreeTable(tablePointThree, actualDirection, PointThreeRow, PointThreeColumn);
            //# change tableTryThree
            tableTryThree = UpdateTableThree(tableTryThree, actualDirection, PointThreeRow, PointThreeColumn);
        }
        return tableTryThree;
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
