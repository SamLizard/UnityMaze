using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildFromList : MonoBehaviour
{
    // public readonly int size = 3;
    private readonly int square_size = 10;

    private readonly Tuple<int, int>[] directions = new Tuple<int, int>[4] {
        new Tuple<int, int>(0, -1),
        new Tuple<int, int>(-1, 0),
        new Tuple<int, int>(0, 1),
        new Tuple<int, int>(1, 0)
    };

    public GameObject[] squares;
    void Start()
    {
        int[,] table = new int[3, 3] { { 0, 1, 1 },
                                       { 1, 1, 0 }, 
                                       { 0, 1, 0 } };

        InstantiateMaze(table);
    }

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
                    if (!IsOutOfBounds(table.GetLength(0), row + x, column + y) && table[row + x, column + y] != 0)
                    {
                        num_of_item[i] = 0;
                    }
                }

                InstantiateSquare(row, column, table.GetLength(0), num_of_item);
            }
        }
    }

    private bool IsOutOfBounds(int size, int row, int column)
    {
        return row >= size
            || row < 0
            || column >= size
            || column < 0;
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
            if ((i == 0 && walls[i] == 1 && walls[3] == 1) ||(i > 0 && walls[i] == 1 && walls[i-1] == 1))
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
                            rotation = 90;
                            if (walls[3] == 0)
                            {
                                rotation += 90;
                            }
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
