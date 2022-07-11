using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    internal void SetScore(int score)
    {
        GetComponent<TMP_Text>().text = "Score " + score;

    }
}
