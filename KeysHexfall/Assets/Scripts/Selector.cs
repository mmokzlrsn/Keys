using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour //DONE
{

    private float rotationX = 0.375f;
    private float rotationY = 0.5f;
    //i divided the selector into pieces 
    [Header("Selector Pieces")]
    public GameObject tripleHex; //game object that has sprite renderer fused triple hexes
    public GameObject middleCircle; //when selecting a triplehex to rotate, middlecircle will make it more visualize
    public GameObject[] hexes; //child objects of selector, this will help to acces chosen triplehexes colors 

    public SpriteRenderer[] bombSpriteRenderers; //if we choose bomb special event will happen so its good to make it seperated

    private SpriteRenderer[] hexSpriteRenderers; //main color accesser

    private int selectorHexNumber = 3; //there will be always 3 hexes in selector

    public Color[] hexColors; //this will store colors of selected hexes

    [System.NonSerialized]
    public HexGridJunction SelectedHexGridJunction; //this will hold the selected hex trio

    public void GetSpriteRenderers(SpriteRenderer[] spriteRenderers, GameObject[] hexes) //gets the sprite of the selected hexes
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i] = hexes[i].GetComponent<SpriteRenderer>();
        }
    }

    public void GetBombSpriteRenderers(SpriteRenderer[] bombSpriteRenderers, GameObject[] hexes)
    {
        for (int i = 0; i < bombSpriteRenderers.Length; i++)
        {
            bombSpriteRenderers[i] = hexes[i].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        HexGrid.Instance.Selector = this;


        hexSpriteRenderers = new SpriteRenderer[selectorHexNumber];

        GetSpriteRenderers(hexSpriteRenderers, hexes);
        GetBombSpriteRenderers(bombSpriteRenderers, hexes);

        middleCircle.transform.parent = null; //prevent circle diminished
        middleCircle.transform.localScale = new Vector3(1, 1, 1) * HexGrid.Instance.HexScale.x;
        transform.localScale = HexGrid.Instance.HexScale; // if scale of hexes are changed in future, circle will adjust its size according to the scale

        Deactivate();
    }


    public void Deactivate()
    {
        gameObject.SetActive(false);
        middleCircle.SetActive(false);
    }

    public void Reactivate()
    {
        Activate(Camera.allCameras[0].WorldToScreenPoint(transform.position));
    }

    public void Activate(Vector2 screenPoint) //touch position returns vector 2 
    {
        Activate(new Vector3(screenPoint.x, screenPoint.y, 0));
    }

    public void Activate(Vector3 screenPoint) //mouseposition returns vector 3
    {
        if (HexGrid.Instance == null)
            return;

        Vector3 worldPoint = Camera.allCameras[0].ScreenToWorldPoint(screenPoint);

        //Find the closest GridJunction
        HexGridJunction closest = null;
        float closestDistance = float.MaxValue;
        for (int x = 0; x < HexGrid.Instance.HexGridJunctions.GetLength(0); x++)
        {
            for (int y = 0; y < HexGrid.Instance.HexGridJunctions.GetLength(1); y++)
            {
                float distance = Vector3.Distance(HexGrid.Instance.HexGridJunctions[x, y].WorldPosition, worldPoint);

                if (distance > closestDistance)
                    continue;

                closest = HexGrid.Instance.HexGridJunctions[x, y];
                closestDistance = distance;
            }
        }

        SelectedHexGridJunction = closest;

        //Set gameobject active
        gameObject.SetActive(true);
        middleCircle.SetActive(true);

        //Disable bombs and enable hexagons
        Disable(bombSpriteRenderers);
        Enable(hexSpriteRenderers);

        SetPosition(); //set circle and position of selector in world pos

        //Rotate Bg and reposition pieces based on oddness
        RotateTripleHex();

        //Assign Colors
        AssignColors();
        
        /* 
         * hexColors[0] = SelectedHexGridJunction.HexGridPoints[0].Hex.Color;
        hexColors[1] = SelectedHexGridJunction.HexGridPoints[1].Hex.Color;
        hexColors[2] = SelectedHexGridJunction.HexGridPoints[2].Hex.Color;
        */
        //Check if any of the pieces are actually bombs
        AnyBomb();

    }

    private void AnyBomb()
    {
        for (int i = 0; i < hexSpriteRenderers.Length; i++)
        {
            if (SelectedHexGridJunction.HexGridPoints[i].Hex is Bomb)
            {
                hexSpriteRenderers[i].enabled = false;
                bombSpriteRenderers[i].enabled = true;
                bombSpriteRenderers[i].color = hexSpriteRenderers[i].color;
            }
        }

    }

    private void RotateTripleHex() //dont rotate Z axis
    {
        

        tripleHex.transform.localRotation = SelectedHexGridJunction.IsOdd ? Quaternion.Euler(Vector3.zero) : Quaternion.Euler(Vector3.up * 180);
        hexes[0].transform.localPosition = SelectedHexGridJunction.IsOdd ? new Vector3(-rotationX, 0) : new Vector3(rotationX, 0);
        hexes[1].transform.localPosition = SelectedHexGridJunction.IsOdd ? new Vector3(rotationX, -rotationY) : new Vector3(-rotationX, -rotationY);
        hexes[2].transform.localPosition = SelectedHexGridJunction.IsOdd ? new Vector3(rotationX, rotationY) : new Vector3(-rotationX, rotationY);

    }

    private void AssignColors()
    {
        for (int i = 0; i < hexColors.Length; i++)
            hexColors[i] = SelectedHexGridJunction.HexGridPoints[i].Hex.Color;

    }

    public void Disable(SpriteRenderer[] spriteRenderers) // at the start it will be used by bombs 
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].enabled = false;
        }

    }

    public void Enable(SpriteRenderer[] spriteRenderers)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].enabled = true;
        }
    }

    public void SetPosition()
    {
        middleCircle.transform.position = transform.position = SelectedHexGridJunction.WorldPosition;
    }

    private IEnumerator RotateEnumerator;
    public void RotateCounterClockwise() => StartCoroutine(RotateEnumerator = Rotate(1f));
    public void RotateClockwise() => StartCoroutine(RotateEnumerator = Rotate(-1f));

    private IEnumerator Rotate(float dir)
    {
        /*Play Audio
        if (dir < 0)
            HexGrid.Instance.AudioSource.PlayOneShot(HexGrid.Instance.AC_PieceClockwise);
        else
            HexGrid.Instance.AudioSource.PlayOneShot(HexGrid.Instance.AC_PieceCounterClockwise);
        */


        //if its bomb, hide textmash active after move

        HideHexShowBomb();
        

        //convert into enum //prevent player input ??
        HexGrid.Instance.GameReady = false;

        //there are 3 rotations so it will be divided to 3
        //with this i can use lerp 
        float rotationTime = 1f / 3;

        for (int i = 0; i < 3; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, dir * 120 * (i + 1));
            Quaternion startRotation = Quaternion.Euler(0, 0, dir * 120f * i); //rotation 360 / 3 
            float startTime = Time.time;
            while (Time.time < startTime + rotationTime)
            {
                transform.rotation = Quaternion.Lerp(startRotation, rotation, (1f / rotationTime) * (Time.time - startTime)); //LERP
                yield return null;
            }
            transform.rotation = rotation;

            //Todo: Actually switch pieces here
            if (dir > 0)
                SelectedHexGridJunction.SwitchPiecesClockwise();
            else
                SelectedHexGridJunction.SwitchPiecesCounterClockwise();

            if (HexGrid.Instance.ExplosionFound = HexGrid.Instance.CheckForExplosion(SelectedHexGridJunction))
            {
                transform.rotation = Quaternion.identity;
                Deactivate();
                break;
            }

            yield return null;
        }

        //Increase NumMoves if explosion occurred otherwise Set GameReady state back to true so player can make new moves.
        if (HexGrid.Instance.ExplosionFound)
            Menu.Instance.NumMoves++;
        else
        {
            HexGrid.Instance.GameReady = true;

            //enable TextMeshRenderers back
            IsBomb();
        }
    }

    public void IsBomb()
    {
        for(int i = 0; i < 3; i++ )
        {
            if (SelectedHexGridJunction.HexGridPoints[i].Hex is Bomb)
            {
                Bomb bomb = (Bomb)SelectedHexGridJunction.HexGridPoints[i].Hex;
                bomb.TextMesh.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }

            
    }

    private void HideHexShowBomb()
    {
        for (int i = 0; i < bombSpriteRenderers.Length; i++)
        {
            if (SelectedHexGridJunction.HexGridPoints[i].Hex is Bomb)
            {
                Bomb bomb = (Bomb)SelectedHexGridJunction.HexGridPoints[i].Hex;
                bomb.TextMesh.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }

        }

    }


}
