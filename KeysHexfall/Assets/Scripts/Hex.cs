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

    public static Queue<Hex> Unused = new Queue<Hex>();

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
    #endregion
    public Vector3 HexGridPos => HexGridPoint.LocalPosition;
    public Vector3 HexGridPosWorld => HexGridPoint.HexGrid.transform.TransformPoint(HexGridPoint.LocalPosition);
    public Vector3 HexGridPosStart => HexGridPoint.LocalStartPosition;
    public Vector3 HexGridPosStartWorld => HexGridPoint.HexGrid.transform.TransformPoint(HexGridPoint.LocalStartPosition);

    [System.NonSerialized]
    public bool IsActivated;

    [System.NonSerialized]
    public float TimeActivated = -1;

    [System.NonSerialized]
    private HexGridPoint LastHexGridPoint = null;

    private float DeltaActivation => Time.time - TimeActivated;
     
  

    protected virtual void Update()
    {
        if (HexGridPoint == null)
            return;
        if (IsActivated)
        {
            transform.localPosition = HexGridPos;
            return;
        }

        if (LastHexGridPoint == null)
        {
            if (DeltaActivation < 1.0f)
                transform.localPosition = Vector3.Lerp(HexGridPosStart, HexGridPos, DeltaActivation);
            else
            {
                transform.localPosition = HexGridPos;
                IsActivated = true;
            }
        }
        else
        {
            float t = (1.0f / HexGrid.Instance.Size.y) * (LastHexGridPoint.Y - HexGridPoint.Y);
            if (DeltaActivation < t)
                transform.localPosition = Vector3.Lerp(LastHexGridPoint.LocalPosition, HexGridPos, (1.0f / t) * DeltaActivation);
            else
            {
                transform.localPosition = HexGridPos;
                IsActivated = true;
            }
        }
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

    public void Deactivate(bool preserveGridPoint = false)
    {
        gameObject.SetActive(false);
        TimeActivated = -1f;
        IsActivated = false;

        if (!preserveGridPoint)
        {
            HexGridPoint.Hex = null;
            HexGridPoint = null;
            LastHexGridPoint = null;

            if (this is Bomb)
            {
                Bomb bomb = (Bomb)this;
                if (Bomb.All.Contains(bomb))
                    Bomb.All.Remove(bomb);
                Destroy(gameObject);
            }
            else
                Hex.Unused.Enqueue(this);
        }
    }

    public static bool ActivatePooled(HexGridPoint hexGridPoint)
    {
        if (Unused.Count < 1)
            return false;

        return Unused.Dequeue().Activate(hexGridPoint, true);
    }

    public void ActivateInSeconds(float time)
    {
        Invoke("Activate", time);
    }

}


//DONE