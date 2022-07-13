using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class HexGridPoint : MonoBehaviour
{

    [SerializeField]
    private int x;
    public int X => x;
    [SerializeField]
    private int y;
    public int Y => y;

    private float xOffset = 0.725f;

    public static HexGridPoint[,] All; //This 2d array will only work in runtime bcs 2d arrays are not serializable

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public HexGridPoint(HexGrid hexGrid, int x, int y)
    {
        this.HexGrid = hexGrid;
        this.x = x;
        this.y = y;

        if (IsOdd)
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

    public bool IsOdd => X % 2 > 0 ? true : false;
}
