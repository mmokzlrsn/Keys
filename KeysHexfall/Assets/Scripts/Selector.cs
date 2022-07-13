using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    //i divided the selector into pieces 
    [Header("Selector Pieces")]
    public GameObject tripleHex; //game object that has sprite renderer fused triple hexes
    public GameObject middleCircle; //when selecting a triplehex to rotate, middlecircle will make it more visualize
    public GameObject[] hexes; //child objects of selector, this will help to acces chosen triplehexes colors 

    public SpriteRenderer[] bombSpriteRenderers; //if we choose bomb special event will happen so its good to make it seperated

    private SpriteRenderer[] hexSpriteRenderers; //main color accesser

    private int selectorHexNumber = 3; 

    public Color[] hexColors; //this will store colors of selected hexes

    [System.NonSerialized]
    public HexGridJunction SelectedHexGridJunction; //

    public void GetSpriteRenderers(SpriteRenderer[] spriteRenderers, GameObject[] hexes)
    {
        for( int i = 0; i < spriteRenderers.Length; i++ )
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
