using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Move : MonoBehaviour
{
    internal void SetMove(int move)
    {
        GetComponent<TMP_Text>().text = "Move " + move;
    }
}
