using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CanvasHelper : MonoBehaviour
{
    #region Score

    public delegate void CanvasScoreHendler(int score);
    public static int scorePoint { private set; get; }

    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private GameObject gameOver;    

    #endregion

    #region Scripts

    private Player _player;
    private Enemy _enemy;

    #endregion

    [SerializeField] private GameObject[] lifes;

    private void Awake()
    {
        ScriptsRegister();
        ScoreSetMethodSet();
    }    

    private void FixedUpdate()
    {
        LiveControl();
        VictoryCount();
    }

    private void ScriptsRegister()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
    }

    #region ScoreSet

    private void ScoreSetMethodSet()
    {
        _player.ScoreSet += ScoreSetting;
        _enemy.ScoreSet += ScoreSetting;
    }

    private void ScoreSetting(int n)
    {
        score.text = "Score: " + (scorePoint += n);
    }

    #endregion    

    #region Endgame Menu

    private void LiveControl()
    {
        if (_player.Life >= 0)
        {
            for (int i = 0; i < _player.Life; i++)
            {
                lifes[i].SetActive(true);
            }

            for (int j = _player.Life; j < lifes.Length; j++)
            {
                lifes[j].SetActive(false);
            }

            gameOver.SetActive(false); 
            Time.timeScale = 1f;
        }
        else
        {
            gameOver.SetActive(true);
            Time.timeScale = 0f;
        }            
    }
    private void VictoryCount()
    {
        if (_enemy.IsDead >= 4 || _player.DotsCount >= 285)
        {
            gameOver.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            gameOver.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    #endregion
}
