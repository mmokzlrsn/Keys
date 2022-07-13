using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{

    public static HexGrid Instance;

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
    private HexGridPoint[] HexGridPoints; //public number of hexes's points
    [HideInInspector]
    public Hex[] Hexes;
    [System.NonSerialized]
    public HexGridJunction[,] HexGridJunctions;
    [System.NonSerialized]
    public Selector Selector;
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

    public void GenerateGrid()
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
        transform.position = new Vector3((-((Size.x - 1) * HexScale.x * 0.725f) / 2), (-((Size.y) * HexScale.y) / 2));

        //Initialize Piece array;
        Hexes = new Hex[Size.x * Size.y];

        //Populate Piece Object Pool
        for (int i = 0; i < Hexes.Length; i++)
            Hex.CreateNew(i);

        //Initialize GridPoint array;
        HexGridPoints = new HexGridPoint[Size.x * Size.y];

        //Instantiate GridPoints based on Grid Size
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
