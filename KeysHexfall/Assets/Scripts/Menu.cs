using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static Menu Instance;

    public Text TextScore;
    public Text TextNumMoves;

    private int score = 0;
    public int Score
    #region Property
    {
        get => score;
        set => TextScore.text = (score = value).ToString();
    }
    #endregion
    private int numMoves = 0;
    public int NumMoves
    #region Property
    {
        get => numMoves;
        set
        {
            TextNumMoves.text = (numMoves = value).ToString();
            Bomb.TickAllBombs();
        }
    }
    #endregion

    private void Awake() => Menu.Instance = this;

    //Todo: Implement reseting static variables just in case
    public void Restart()
    {
        foreach (Bomb bomb in Bomb.All)
            Destroy(bomb.gameObject);
        Bomb.All.Clear();

        Hex.Unused.Clear();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}

