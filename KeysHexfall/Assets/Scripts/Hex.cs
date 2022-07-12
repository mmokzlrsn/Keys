using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{

    public Color Color
    #region Property
    {
        get => SpriteRenderer.color;
        set
        {
            for (int i = 0; i < HexGridPoint.HexGrid.HexColors.Length; i++)
            {
                if (HexGridPoint.HexGrid.HexColors[i].Equals(value))
                {
                    colorIndex = i;
                    SpriteRenderer.color = HexGridPoint.HexGrid.HexColors[i];
                    break;
                }
            }
        }
    }
    #endregion
    [SerializeField]
    private int colorIndex;
    public int ColorIndex => colorIndex;

    public int Index = -1;

    [SerializeField]
    private HexGridPoint hexGridPoint;
    public HexGridPoint HexGridPoint
    #region Property
    {
        get => hexGridPoint;
        set
        {
            LastHexGridPoint = hexGridPoint;
            hexGridPoint = value;
        }
    }

    public Vector3 HexGridPos => HexGridPoint.LocalPosition;
    public Vector3 HexGridPosWorld => HexGridPoint.HexGrid.transform.TransformPoint(HexGridPoint.LocalPosition);
    public Vector3 HexGridPosStart => HexGridPoint.LocalStartPosition;
    public Vector3 HexGridPosStartWorld => HexGridPoint.HexGrid.transform.TransformPoint(HexGridPoint.LocalStartPosition);

    [System.NonSerialized]
    public float TimeActivated = -1;

    private HexGridPoint LastHexGridPoint = null;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer => spriteRenderer == null ? spriteRenderer = GetComponent<SpriteRenderer>() : spriteRenderer;

    public static Hex CreateNew(int index)
    {
        Hex hex = CreateNewSprite(index).AddComponent<Hex>();
        hex.SpriteRenderer.sprite = HexGrid.Instance.HexSprite;
        HexGrid.Instance.Hexes[index] = hex;
        hex.Index = index;
        return hex;

    }

    protected static GameObject CreateNewSprite(int i = -1)
    {
        GameObject gameObject = new GameObject("Hex" + (i < 0 ? "" : i.ToString()));
        gameObject.SetActive(false);
        gameObject.transform.parent = HexGrid.Instance.transform;
        gameObject.transform.localScale = HexGrid.Instance.HexScale;

        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        return gameObject;
    }

    public bool Activate()
    {
        return Activate(null);
    }

    public bool Activate(HexGridPoint hexGridPoint, bool randomizeColor = false)
    {
        gameObject.SetActive(true);
        TimeActivated = Time.time;
        if (HexGridPoint == null)
        {
            hexGridPoint.Hex = this;
            //GridPoint = gridPoint; //Propert takes care of this
        }
        transform.localPosition = HexGridPosStart;
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            TimeActivated = -1;
            transform.localPosition = HexGridPos;
        }
#endif
        if (randomizeColor)
            Color = HexGrid.Instance.HexColors[Random.Range(0, HexGrid.Instance.HexColors.Length)];

        return true;
    }

}
