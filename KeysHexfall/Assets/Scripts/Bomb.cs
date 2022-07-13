using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Hex //DONE
{

    public TextMesh TextMesh;

    public static List<Bomb> All = new List<Bomb>();

    public string Text
    #region Property
    {
        get => TextMesh.text;
        set => TextMesh.text = value;
    }
    #endregion
    [System.NonSerialized]
    public int CreatedAtMove;
    [System.NonSerialized]
    public int Countdown;
    public int RemainingMoves => Countdown - (Menu.Instance.NumMoves - CreatedAtMove);

    private static bool exploded;
    public static bool Exploded
    #region Property
    {
        get
        {
            bool returnVal = exploded;
            exploded = false;
            return returnVal;
        }
        set => exploded = value;
    }
    #endregion

    #region Helper Functions
    public static Bomb CreateNew(HexGridPoint hexGridPoint)
    {
        Bomb bomb = Hex.CreateNewSprite().AddComponent<Bomb>();
        bomb.gameObject.name = "bomb";
        //Both x and y are assigned PieceScale.X because the bomb is round, we don't want to squish it by assigning different values
        bomb.transform.localScale = new Vector3(HexGrid.Instance.HexScale.x, HexGrid.Instance.HexScale.x);
        bomb.gameObject.SetActive(true);
        Bomb.All.Add(bomb);

        bomb.SpriteRenderer.sprite = HexGrid.Instance.BombSprite;
        bomb.SpriteRenderer.sortingOrder = 1;

        //text editing
        GameObject temp = new GameObject("textMesh");
        temp.transform.parent = bomb.transform;
        bomb.TextMesh = temp.AddComponent<TextMesh>();
        bomb.TextMesh.alignment = TextAlignment.Center;
        bomb.TextMesh.anchor = TextAnchor.MiddleCenter;
        bomb.TextMesh.color = Color.white;
        bomb.TextMesh.characterSize = 0.5f;
        bomb.TextMesh.gameObject.GetComponent<MeshRenderer>().sortingOrder = 9;

        hexGridPoint.Hex = bomb;
        bomb.Color = HexGrid.Instance.HexColors[Random.Range(0, HexGrid.Instance.HexColors.Length)];
        bomb.TimeActivated = Time.time;
        bomb.transform.localPosition = bomb.HexGridPosStart;
        bomb.CreatedAtMove = Menu.Instance.NumMoves;
        bomb.Countdown = Random.Range(8, 12);
        bomb.Tick(); //Do this once to display text.


        return bomb;
    }
    #endregion

     

    public static void TickAllBombs()
    {
        foreach (Bomb bomb in Bomb.All)
        {
            if (bomb == null)
            {
                //Todo: Fix this
                Bomb.All.Remove(bomb);
                continue;
            }

            bomb.Tick();
        }
    }

    private void Tick()
    {
        TextMesh.gameObject.GetComponent<MeshRenderer>().enabled = true;
        TextMesh.text = RemainingMoves.ToString();
    }

    public static void CheckFuses()
    {
        foreach (Bomb bomb in Bomb.All)
        {
            if (bomb == null)
            {
                //Todo: Fix this
                Bomb.All.Remove(bomb);
                continue;
            }

            if (bomb.RemainingMoves <= 0)
            {
                Menu.Instance.Restart();
                Bomb.Exploded = true;
                return;
            }
        }
    }
}
