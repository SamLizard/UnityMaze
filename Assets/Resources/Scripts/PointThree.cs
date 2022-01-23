using System;

public class PointThree
{
    private bool[] directions;
    private bool connection; 
    private Compartiment compartiment;
    private int remainingDirections;

    public PointThree(bool[] directions, bool connection, int compartiment)
    {
        this.directions = directions;
        this.connection = connection;
        this.compartiment = new Compartiment(compartiment);
        this.remainingDirections = CountRemainingDirections();
    }

    //public PointThree(bool[] directions, int compartimentNumber)
    //{
    //    this.directions = directions;
    //    this.connection = false;
    //    this.compartiment = new Compartiment(compartimentNumber);
    //    this.remainingDirections = CountRemainingDirections();
    //}

    public PointThree(int compartimentNumber)
    {
        this.directions = new bool[4] { true, true, true, true};
        this.connection = false;
        this.compartiment = new Compartiment(compartimentNumber);
        this.remainingDirections = 3;
    }

    private int CountRemainingDirections()
    {
        int directions = 4;
        for (int i = 0; i < 4; i++)
        {
            if (!this.directions[i])
            {
                directions--;
            }
        }
        return directions;
    }

    //public int getRemainingDirections()
    //{
    //    return this.remainingDirections;
    //}

    public bool canConnect()
    {
        return this.remainingDirections > 0;
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
        this.remainingDirections--;
    }

    public bool[] getDirections()
    {
        return this.directions;
    }

    public Compartiment getCompartiment()
    {
        return this.compartiment;
    }

    //public int getCompartimentCount()
    //{
    //    return this.compartiment.getCount();
    //}

    public void setCompartiment(Compartiment compartiment)
    {
        this.compartiment = compartiment;
    }

    //public void increaseCompartiment(int toAdd)
    //{
    //    this.compartiment.addCount(toAdd);
    //}

    //public void increaseCompartimentByCompartiment(Compartiment compartiment)
    //{
    //    this.compartiment.addCount(compartiment.getCount());
    //}

    public string toString()
    {
        return "[(" + this.directions[0] + ", " + this.directions[1] + ", " + this.directions[2] + ", " + this.directions[3] + ") " + this.connection + ", " + this.remainingDirections + ", " + this.compartiment.toString() + "]";
    }
}