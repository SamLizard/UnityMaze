using System;

public class PointThree
{
    public bool[] directions;
    public bool connection;

    // delete this method?
    public PointThree(bool[] directions, bool connexion) //  add an int that count how much directions are true (and put only 4 booleans in directions instead of 5)
    {
        this.directions = directions;
        this.connection = connexion;
    }

    public PointThree(bool[] directions)
    {
        this.directions = directions;
        this.connection = false;
    }

    public PointThree()
    {
        this.directions = new bool[5] { true, true, true, true, true };
        this.connection = false;
    }

    public bool canConnect()
    {
        return this.directions[4]; // 4 -> directions.length - 1
    }

    public void setConnection()
    {
        this.connection = true;
    }

    public bool getConnection()
    {
        return this.connection;
    }

    public void setDirections(int place)
    {
        this.directions[place] = false;
        bool onlyFalse = false;
        for (int i = 0; i < 4; i++)
        {
            if (this.directions[i])
            {
                onlyFalse = true;
                break;
            }
        }
        if (!onlyFalse)
        {
            directions[4] = false;
        }
    }

    public bool[] getDirections()
    {
        return this.directions;
    }

    public bool getLastDirection()
    {
        return this.directions[4];
    }

    public string toString()
    {
        return "[(" + this.directions[0] + ", " + this.directions[1] + ", " + this.directions[2] + ", " + this.directions[3] + ", " + this.directions[4] + ") " + this.connection + "]";
    }
}