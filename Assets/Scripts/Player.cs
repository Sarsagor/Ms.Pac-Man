using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{
    #region Events

    public event CanvasHelper.CanvasScoreHendler ScoreSet;
    public event Enemy.EnemyHendler SetMortal;
    public event Enemy.EnemyHendler ResetPos;

    #endregion

    #region Life

    public int Life { private set; get; }

    private bool _isDead;
    private int _scoreForLife;
    private const int _pointForScore = 1000;

    #endregion

    #region Move
    [SerializeField] private Animator _playerAnim;

    private Vector2 _moveDir;
    private Vector2 _playerMove;
    private Vector2 _startPos = new Vector2(0, -1.485f);
    private const float _teleportX = -2.311f;

    #endregion
    
    public int DotsCount { private set; get; }

    private void Awake()
    {
        Life = 2;
        Time.timeScale = 1f;
        _scoreForLife = _pointForScore;
    }

    void Update()
    {
        if (!_isDead)
            Move();

        LiveForScore();
    }

    #region Life & Death

    private void LiveForScore()
    {
        if (CanvasHelper.scorePoint >= _scoreForLife)
        {
            if (Life <= 4)
                Life++;

            _scoreForLife += _pointForScore;
        }
    }

    private IEnumerator TimeToDie()
    {
        _isDead = true;
        _playerAnim.SetBool("isDead", _isDead);

        yield return new WaitForSeconds(1);

        Life--;
        _isDead = false;
        _moveDir = Vector2.zero;
        _playerAnim.SetBool("isDead", _isDead);
        transform.position = _startPos;
    }

    #endregion    

    #region Moving
    private void Move()
    {
        _playerMove = DirectionMove() * Time.deltaTime;
        transform.position += (Vector3)_playerMove;
    }

    private Vector2 DirectionMove()    
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            _moveDir = Vector2.left;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            _moveDir = Vector2.right;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            _moveDir = Vector2.up;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            _moveDir = Vector2.down;

        RefAnimationMove(_moveDir);
        return _moveDir;
    }    

    private void RefAnimationMove(Vector2 move)
    {
        _playerAnim.SetFloat("Horizontal", move.x);
        _playerAnim.SetFloat("Vertical", move.y);
    }    

    #endregion

    #region Trigger & Collision

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleport"))
        {
            transform.position = collision.gameObject.name == "teleportR" ?
                new Vector2(-_teleportX, transform.position.y) : new Vector2(_teleportX, transform.position.y);
            return;
        }

        if (collision.CompareTag("big"))
        {
            ScoreSet?.Invoke(30);
            SetMortal?.Invoke();
        }

        if (collision.CompareTag("little"))
            ScoreSet?.Invoke(20);

        DotsCount++;
        collision.gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !collision.gameObject.GetComponent<Enemy>().IsMortal)
        {
            StartCoroutine(TimeToDie());
            ResetPos?.Invoke();
        }
    }

    #endregion
}
