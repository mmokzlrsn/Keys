using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class HexGridPoint //DONE
{
    public static HexGridPoint[,] All; //This 2d array will only work in runtime bcs 2d arrays are not serializable
    [SerializeField]
    private int x;
    public int X => x;
    [SerializeField]
    private int y;
    public int Y => y;

    private float xOffset = 0.725f;

    

    public HexGrid HexGrid;
    [SerializeField]
    private Hex hex;
    public Hex Hex
    #region Property
    {
        get => hex;
        set
        {
            if (value != null)
                value.HexGridPoint = this;
            hex = value;
        }
    }
    #endregion

    [SerializeField]
    private Vector3 localPosition; //starting position will be 0,0 left bottom corner
    public Vector3 LocalPosition => localPosition; //I did this because readonly fields are not serialized
    public Vector3 WorldPosition => HexGrid.Instance.transform.TransformPoint(localPosition);
    [SerializeField]
    private Vector3 localStartPosition;
    public Vector3 LocalStartPosition => localStartPosition;


    public HexGridPoint(HexGrid hexGrid, int x, int y)
    {
        this.HexGrid = hexGrid;
        this.x = x;
        this.y = y;

        if (IsOdd) //position is not same if it is Odd 
        {
            localPosition = new Vector3(x * HexGrid.HexScale.x * xOffset, y * HexGrid.HexScale.y);
            localStartPosition = new Vector3(x * HexGrid.HexScale.x * xOffset, HexGrid.HexScale.y + LocalPosition.y);
        }
        else
        {
            localPosition = new Vector3(x * HexGrid.HexScale.x * xOffset, (y * HexGrid.HexScale.y) - (HexGrid.HexScale.y / 2));
            localStartPosition = new Vector3(x * HexGrid.HexScale.x * xOffset, HexGrid.HexScale.y + LocalPosition.y - (HexGrid.HexScale.y / 2));
        }
    }

    public static bool GetCommonNeighbor(HexGridPoint a, HexGridPoint b, HexGridPoint excluding, out HexGridPoint neighbor) //math stuff getting their positions
    {
        neighbor = null;

        //If a and b isn't neighbors this is futile!
        if ((Mathf.Abs(a.X - b.X) != 1 && Mathf.Abs(a.Y - b.Y) > 1) || (Mathf.Abs(a.Y - b.Y) != 1 && Mathf.Abs(a.X - b.X) > 1))
            return false;

        HexGridPoint temp = null;
        if (a.Y == b.Y)
        {
            if (a.IsOdd)
            {

                if (a.Y > 0)
                {
                    temp = All[a.X, a.Y - 1];
                    if (temp != excluding)
                    {
                        neighbor = temp;
                        return true;
                    }
                }

                if (b.Y < HexGrid.Instance.Size.y - 1)
                {
                    temp = All[b.X, b.Y + 1];
                    if (temp != excluding)
                    {
                        neighbor = temp;
                        return true;
                    }
                }
            }
            else
            {
                if (a.Y < HexGrid.Instance.Size.y - 1)
                {
                    temp = All[a.X, a.Y + 1];
                    if (temp != excluding)
                    {
                        neighbor = temp;
                        return true;
                    }
                }

                if (b.Y > 0)
                {
                    temp = All[b.X, b.Y - 1];
                    if (temp != excluding)
                    {
                        neighbor = temp;
                        return true;
                    }
                }
            }
        }
        else if (a.X == b.X)
        {
            int lowestY = a.Y < b.Y ? a.Y : b.Y;
            if (a.x > 0)
            {
                temp = All[a.X - 1, lowestY];
                if (temp != excluding)
                {
                    neighbor = temp;
                    return true;
                }
            }

            if (b.X < HexGrid.Instance.Size.x - 1)
            {
                temp = All[b.X + 1, lowestY];
                if (temp != excluding)
                {
                    neighbor = temp;
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsOdd => X % 2 > 0 ? true : false;
}
