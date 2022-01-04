using System;

public class Point
{
	public int row;
	public int column;
	public int value;

	public Point(int row, int column, int value)
	{
		this.row = row;
		this.column = column;
		this.value = value;
	}

	public Point(int row, int column)
	{
		this.row = row;
		this.column = column;
		this.value = 0;
	}

	public int getRow()
    {
		return this.row;
    }

	public int getColumn()
    {
		return this.column;
    }

	public int getValue()
    {
		return this.value;
    }

	public bool isValue(int value)
    {
		return this.value == value;
    }

	public void setValue(int value)
    {
		this.value = value;
    }

	public string toString()
    {
		return "[(" + this.row + ", " + this.column + "), " + this.value + "]";
    }
}