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

    public GameObject[] squares;
    void Start()
    {
        int[,] tableNotReady = new int[5, 5] { { 1, 0, 1, 1, 1 },
                                               { 1, 0, 0, 1, 1 },
                                               { 1, 1, 0, 1, 0 },
                                               { 0, 1, 1, 1, 0 },
                                               { 1, 1, 0, 1, 1 } };
        PrintList(tableNotReady);
        tableNotReady = RemoveWallBetween(tableNotReady, 1, 1, new int[4] { 0, 0, 1, 0 });
        tableNotReady = RemoveWallBetween(tableNotReady, 5, 5, new int[4] { 1, 0, 0, 0 });
        PrintList(tableNotReady);

        InstantiateMaze(tableNotReady);
    }

    private bool IsOutOfBounds(int row, int column)
    {
        return row > size + 1
            || row < 0
            || column > size + 1
            || column < 0;
    }

    private int[,] RemoveWallBetween(int[,] table, int row, int column, int[] direction) // direction : right = first; top = second; left = third; bottom = fourth
    {
        for (int i = 0; i < directions.Length; i++)
        {
            if (direction[i] == 1)
            {
                (int x, int y) = directions[i];
                if (!IsOutOfBounds(row + 2 * x, column + 2 * y))
                {
                    int biggest = Math.Max(table[row, column], table[row + 2 * x, column + 2 * y]);
                    table[row, column] = biggest;
                    table[row + x, column + y] = biggest;
                    table[row + 2 * x, column + 2 * y] = biggest;

                }
                return table;
            }
        }
        return table;
    }

    private void PrintList(int[,] table)
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
        Debug.Log("299\n" + str);
    }


    private int[,] ChangeCasesToBiggestNumber(int[,] table, int row, int column)
    {
        Tuple<int, int, int>[] cases = new Tuple<int, int, int>[9];
        int biggestNumber = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++) // row = row + i - 1; column = column + j - 1; number = table[row + i - 1, column + j - 1]
            {
                if (!IsOutOfBounds(row + i - 1, row + j - 1))
                {
                    cases[i * 3 + j] = new Tuple<int, int, int>(row + i - 1, column + j - 1, table[row + i - 1, column + j - 1]);
                    biggestNumber = Math.Max(biggestNumber, table[row + i - 1, column + j - 1]);
                }
                else
                {
                    cases[i * 3 + j] = new Tuple<int, int, int>(row + i - 1, column + j - 1, 0);
                }
            }
        }
        table = ChangeCasesToNumber(table, cases, biggestNumber);
        return table;
    }

    private int[,] ChangeCasesToNumber(int[,] table, Tuple<int, int, int>[] cases, int number)
    {
        for (int i = 0; i < cases.Length; i++)
        {
            (int row, int column, int nothing) = cases[i];
            if (table[row, column] > 1)
            {
                table[row, column] = number;
            }
        }
        return table;
    }

    private Tuple<int, int, int>[] addNextCase(Tuple<int, int, int>[] cases, int[,] table, int row, int column, bool[] directions, int biggestNumber) // check the biggest number before calling this method. // 
    {
        if (table[row, column] > 1)
        {
            biggestNumber = Math.Max(table[row, column], biggestNumber);
            table[row, column] = biggestNumber;
            //cases. // to continue?
        }
        else
        {
            return cases;
        }
        return cases;
    }

    private int BiggestNumber(Tuple<int, int, int>[] cases)
    {
        int biggestNumber = 0;
        for (int i = 0; i < cases.Length; i++)
        {
            (int row, int column, int number) = cases[i];
            if (number > biggestNumber)
            {
                biggestNumber = number;
            }
        }
        return biggestNumber;
    }


    // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////instantiating the maze in Unity. Alreadyin previous commit
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
