#define DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HexGrid : MonoBehaviour //DONE
{

    public static HexGrid Instance;

    private float xOffset = 0.725f;

    [Header("Hex Grid Editor")]
    public Vector2Int Size = new Vector2Int(8, 9); //game area 
    public float HexActivationInterval = 0.01f; //for a interval when activating hex

    [Header("Hex Editor")]
    public Vector3 HexScale = new Vector3(1, 1, 1); //scale can be change depending on 
    public Color[] HexColors = new Color[5] { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan };
    public int BombScore = 1000;
    
    public bool StartByCheckingExplosions = false;

    [Header("Sprites")]
    public Sprite HexSprite;
    public Sprite BombSprite;


    [HideInInspector]
    public HexGridPoint[] HexGridPoints; //public number of hexes's points
    [HideInInspector]
    public Hex[] Hexes;
    [System.NonSerialized]
    public HexGridJunction[,] HexGridJunctions;
    [System.NonSerialized]
    public Selector Selector;
    [System.NonSerialized]
    private Vector3 LastClickedPosition;

    [System.NonSerialized]
    public bool GameReady = false; // make this enum

    [System.NonSerialized]
    public bool ExplosionFound = false;

    [System.NonSerialized]
    public int BombCounter;

    [System.NonSerialized]
    public float TimeActivated = -1;

    private void Awake()
    {
        HexGrid.Instance = this;
        //AudioSource = gameObject.GetComponent<AudioSource>();

        //if (Bomb.Exploded)
            //AudioSource.PlayOneShot(AC_BombExplosion);
    }

    public void GenerateHexGrid()
    {
        //Clean up old generated grid if it exists
        RemoveHexGrid();
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            HexGrid.Instance = this;
        }
#endif

        //Set up Grid origin based on Grid Size
        transform.position = new Vector3((-((Size.x - 1) * HexScale.x * xOffset) / 2), (-((Size.y) * HexScale.y) / 2));

        //Initialize HEX array;
        Hexes = new Hex[Size.x * Size.y];

        //Populate HEX Object Pool
        for (int i = 0; i < Hexes.Length; i++)
            Hex.CreateNew(i);

        //Initialize GridPoint array;
        HexGridPoints = new HexGridPoint[Size.x * Size.y];

        //Instantiate HexGridPoints based on Grid Size
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                HexGridPoints[x + (y * Size.x)] = new HexGridPoint(this, x, y);
                if (!Application.isPlaying)
                    Hexes[x + (y * Size.x)].Activate(HexGridPoints[x + (y * Size.x)], true);
            }
        }
    }

    public void RemoveHexGrid()
    {
        if (Hexes != null)
        {
            for (int i = 0; i < Hexes.Length; i++)
            {
                if (Hexes[i] != null)
                    DestroyImmediate(Hexes[i].gameObject);
            }
            Hexes = null;
        }
        HexGridPoints = null;
    }

    

    void Start()
    {
        //Prepare two dimentional GridPoint Array for easy access
        HexGridPoint.All = new HexGridPoint[Size.x, Size.y];
        for (int i = 0; i < HexGridPoints.Length; i++)
            HexGridPoint.All[HexGridPoints[i].X, HexGridPoints[i].Y] = HexGridPoints[i];

        //Initialize GridJunctions
        HexGridJunctions = new HexGridJunction[(Size.x - 1) * 2, (Size.y - 1)];
        for (int x = 0; x < HexGridJunctions.GetLength(0); x++)
            for (int y = 0; y < HexGridJunctions.GetLength(1); y++)
                HexGridJunctions[x, y] = gameObject.AddComponent<HexGridJunction>();

        //Deactivate All Pieces but preserve their GridPoints and invoke activation.
        for (int i = 0; i < Hexes.Length; i++)
        {
            Hexes[i].Deactivate(true);
            Hexes[i].ActivateInSeconds(HexActivationInterval * (i + 1));
        }

        if (StartByCheckingExplosions)
            ExplosionFound = true;
    }

    // Update is called once per frame
    private void Update()
    {
        //This prevents player interaction when we don't need it.
        //Example: before all pieces falls into place at the start.
        if (!GameReady)
        {
            bool allActivated = true;
            for (int i = 0; i < Hexes.Length; i++)
            {
                if (Hexes[i].gameObject.activeInHierarchy && !Hexes[i].IsActivated)
                {
                    allActivated = false;
                    break;
                }
            }
            foreach (Bomb bomb in Bomb.All)
            {
                if (bomb == null)
                    continue;

                if (bomb.gameObject.activeInHierarchy && !bomb.IsActivated)
                {
                    allActivated = false;
                    break;
                }
            }
            if (allActivated)
            {
                //If there was an explosion earlier, do not enable the game immedietly, check for repeating explosions due to new pieces being added in and shifts.
                if (ExplosionFound)
                    ExplosionFound = CheckForExplosion();
                else
                    GameReady = true;
            }
            return;
        }

        Bomb.CheckFuses();

        //Ignore Input if mouse is hovering on top of the header.
        if (Input.mousePosition.y < Screen.height - 100)
        {
            if (Input.GetMouseButtonDown(0))
            { 
                if (Selector.SelectedHexGridJunction == null)
                    Selector.Activate(Input.mousePosition);
                LastClickedPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector3 delta = Input.mousePosition - LastClickedPosition;
                if (delta.magnitude > 100.0f)
                {
                    #region DEBUG
#if UNITY_EDITOR && DEBUG
                    if (Selector.gameObject.activeInHierarchy)
                        Debug.Log(Input.mousePosition.x + " | " + LastClickedPosition.x);
#endif
                    #endregion
                    if (Selector.gameObject.activeInHierarchy)
                    {
                        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                        {
                            if (Input.mousePosition.x > LastClickedPosition.x)
                                Selector.RotateClockwise();
                            else
                                Selector.RotateCounterClockwise();
                        }
                        else
                        {
                            if (Input.mousePosition.y > LastClickedPosition.y)
                                Selector.RotateClockwise();
                            else
                                Selector.RotateCounterClockwise();
                        }
                    }
                }
                else
                {
                    Selector.Activate(Input.mousePosition);
                    LastClickedPosition = Input.mousePosition;
                }
            }
        }
    }

    private void ShiftHexGridPoints(ref int[] numRemoved, ref int[] lastRemoved)
    {

        for (int x = 0; x < HexGridPoint.All.GetLength(0); x++)
        {
            if (numRemoved[x] < 1)
                continue;

            for (int y = 1 + lastRemoved[x] - numRemoved[x]; y < HexGridPoint.All.GetLength(1); y++)
            {
                if (y < HexGridPoint.All.GetLength(1) - numRemoved[x])
                {
                    HexGridPoint.All[x, y].Hex = HexGridPoint.All[x, y + numRemoved[x]].Hex;
                    //this is set to false until piece falls to its new gridpoint. It will because true once it does automaticly
                    HexGridPoint.All[x, y].Hex.IsActivated = false;
                    //this is set to current time so that fall to place can be lerped.
                    HexGridPoint.All[x, y].Hex.TimeActivated = Time.time;
                }
                else
                {
                    if (BombCounter < Menu.Instance.Score / BombScore)
                    {
                        Bomb.CreateNew(HexGridPoint.All[x, y]);
                        BombCounter++;
                    }
                    else
                        Hex.ActivatePooled(HexGridPoint.All[x, y]);
                }
            }
        }
    }

    public bool CheckForExplosion(HexGridJunction selectedHexGridJunction = null)
    {
        //Assign values for default loop
        int xStart = 0;
        int yStart = 0;
        int xLength = HexGridJunctions.GetLength(0);
        int yLength = HexGridJunctions.GetLength(1);

 
        for (int y = yStart; y < yLength; y++)
        {
            for (int x = xStart; x < xLength; x++)
            {
                if (HexGridJunctions[x, y].HexGridPoints[0].Hex.ColorIndex == HexGridJunctions[x, y].HexGridPoints[1].Hex.ColorIndex
                    && HexGridJunctions[x, y].HexGridPoints[0].Hex.ColorIndex == HexGridJunctions[x, y].HexGridPoints[2].Hex.ColorIndex)
                {
                    GameReady = false;
                    int colorIndex = HexGridJunctions[x, y].HexGridPoints[0].Hex.ColorIndex;

                    #region DEBUG
#if DEBUG
                    Debug.Log("Explosion found at " + x + "," + y);
#endif
                    #endregion
                    int[] numRemoved = new int[HexGridPoint.All.GetLength(0)];
                    int[] lastRemoved = new int[HexGridPoint.All.GetLength(0)];
                    for (int i = 0; i < HexGridJunctions[x, y].HexGridPoints.Length; i++)
                    {
                        numRemoved[HexGridJunctions[x, y].HexGridPoints[i].X]++;
                        lastRemoved[HexGridJunctions[x, y].HexGridPoints[i].X] = HexGridJunctions[x, y].HexGridPoints[i].Y;
                        HexGridJunctions[x, y].HexGridPoints[i].Hex.Deactivate();
                    }

                    //Also need to check surrounding pieces to detect more than 3
                    HexGridPoint neighbor;
                    HexGridPoint hexGridPointA = HexGridJunctions[x, y].HexGridPoints[0];
                    HexGridPoint hexGridPointB = HexGridJunctions[x, y].HexGridPoints[1];
                    HexGridPoint hexGridPointC = HexGridJunctions[x, y].HexGridPoints[2];

                    if (HexGridPoint.GetCommonNeighbor(hexGridPointA, hexGridPointB, hexGridPointC, out neighbor))
                    {
                        if (neighbor != null && neighbor.Hex != null && neighbor.Hex.IsActivated && neighbor.Hex.ColorIndex == colorIndex)
                        {
                            numRemoved[neighbor.X]++;
                            lastRemoved[neighbor.X] = neighbor.Y > lastRemoved[neighbor.X] ? neighbor.Y : lastRemoved[neighbor.X];
                            neighbor.Hex.Deactivate();
                        }
                    }

                    hexGridPointA = HexGridJunctions[x, y].HexGridPoints[1];
                    hexGridPointB = HexGridJunctions[x, y].HexGridPoints[2];
                    hexGridPointC = HexGridJunctions[x, y].HexGridPoints[0];

                    if (HexGridPoint.GetCommonNeighbor(hexGridPointA, hexGridPointB, hexGridPointC, out neighbor))
                    {
                        if (neighbor != null && neighbor.Hex != null && neighbor.Hex.IsActivated && neighbor.Hex.ColorIndex == colorIndex)
                        {
                            numRemoved[neighbor.X]++;
                            lastRemoved[neighbor.X] = neighbor.Y > lastRemoved[neighbor.X] ? neighbor.Y : lastRemoved[neighbor.X];
                            neighbor.Hex.Deactivate();
                        }
                    }

                    hexGridPointA = HexGridJunctions[x, y].HexGridPoints[2];
                    hexGridPointB = HexGridJunctions[x, y].HexGridPoints[0];
                    hexGridPointC = HexGridJunctions[x, y].HexGridPoints[1];

                    if (HexGridPoint.GetCommonNeighbor(hexGridPointA, hexGridPointB, hexGridPointC, out neighbor))
                    {
                        if (neighbor != null && neighbor.Hex != null && neighbor.Hex.IsActivated && neighbor.Hex.ColorIndex == colorIndex)
                        {
                            numRemoved[neighbor.X]++;
                            lastRemoved[neighbor.X] = neighbor.Y > lastRemoved[neighbor.X] ? neighbor.Y : lastRemoved[neighbor.X];
                            neighbor.Hex.Deactivate();
                        }
                    }

                    ShiftHexGridPoints(ref numRemoved, ref lastRemoved);

                    //Add the score
                    int totalRemoved = 0;
                    for (int i = 0; i < numRemoved.Length; i++)
                        totalRemoved += numRemoved[i];
                    Menu.Instance.Score += totalRemoved * 5;

                    //Play Audio
                    //AudioSource.PlayOneShot(AC_PieceExplosion);

                    return true;
                }
            }
        }
        return false;
    }
}
