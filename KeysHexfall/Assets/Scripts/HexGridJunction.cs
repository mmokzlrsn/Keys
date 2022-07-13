using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HexGridJunction
{
    public HexGrid HexGrid;

    public HexGridPoint[] HexGridPoints = new HexGridPoint[3];

    [SerializeField]
    private int x;
    public int X => x;
    [SerializeField]
    private int y;
    public int Y => y;

    [SerializeField]
    private Vector3 localPosition;
    public Vector3 LocalPosition => localPosition;
    public Vector3 WorldPosition => HexGrid.Instance.transform.TransformPoint(localPosition);

    [HideInInspector] //maybe?
    public bool IsOdd => X % 2 > 0 ? true : false;

    public HexGridJunction(HexGrid hexGrid, int x, int y)
    {
        HexGrid = hexGrid;
        this.x = x;
        this.y = y;

        int gridX = x / 2;
        if (IsOdd) //math stuff
        {
            HexGridPoints[0] = HexGridPoint.All[gridX, gridX % 2 > 0 ? y : y + 1];
            HexGridPoints[1] = HexGridPoint.All[gridX + 1, y];
            HexGridPoints[2] = HexGridPoint.All[gridX + 1, y + 1];
        }
        else
        {
            HexGridPoints[0] = HexGridPoint.All[gridX + 1, gridX % 2 > 0 ? y + 1 : y];
            HexGridPoints[1] = HexGridPoint.All[gridX, y];
            HexGridPoints[2] = HexGridPoint.All[gridX, y + 1];
        }

        localPosition = new Vector3((HexGridPoints[0].LocalPosition.x + HexGridPoints[1].LocalPosition.x) / 2, HexGridPoints[0].LocalPosition.y);
    }

    public void SwitchPiecesClockwise() //tried to combine them but i think this is the best version
    {
        Hex[] hexes = new Hex[] { HexGridPoints[0].Hex, HexGridPoints[1].Hex, HexGridPoints[2].Hex };
        if (IsOdd)
        {
            HexGridPoints[0].Hex = hexes[2];
            HexGridPoints[1].Hex = hexes[0];
            HexGridPoints[2].Hex = hexes[1];
        }
        else
        {
            HexGridPoints[2].Hex = hexes[0];
            HexGridPoints[0].Hex = hexes[1];
            HexGridPoints[1].Hex = hexes[2];
        }
    }

    public void SwitchPiecesCounterClockwise()
    {
        Hex[] hexes = new Hex[] { HexGridPoints[0].Hex, HexGridPoints[1].Hex, HexGridPoints[2].Hex };
        if (IsOdd)
        {
            HexGridPoints[2].Hex = hexes[0];
            HexGridPoints[0].Hex = hexes[1];
            HexGridPoints[1].Hex = hexes[2];
        }
        else
        {
            HexGridPoints[0].Hex = hexes[2];
            HexGridPoints[1].Hex = hexes[0];
            HexGridPoints[2].Hex = hexes[1];
        }
    }

    //DONE
}
