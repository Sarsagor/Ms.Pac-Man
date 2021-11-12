using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    #region Event

    public event CanvasHelper.CanvasScoreHendler ScoreSet;
    public delegate void EnemyHendler();

    #endregion

    #region Mortal
    
    public int IsDead { get; private set; }
    public bool IsMortal { get; private set; }

    private bool _isDead;
    private int _wayOutLayer = 11;
    private int _enemyLayer = 9;

    #endregion

    #region Moving

    private Rigidbody2D _enemyRB2;
    private GameObject _player;

    [SerializeField] private Animator _animEnemy;

    private Vector2 _direction;
    private Vector2 _offset;
    private Vector2 _home = new Vector2(0, 0.407f);

    private const float _compens = 0.0625f;
    private float _speed = 0.2f;    

    private bool _horizontal = true;
    private bool _vertical;
    private bool _offsetB;

    #endregion

    #region Start

    [SerializeField] private int _timeToStart = 0;

    private Vector2 _startPos;
    private bool _onStart;

    #endregion

    private void Awake()
    {
        IsDead = 0;
    }

    private void Start()
    {
        _enemyRB2 = gameObject.GetComponent<Rigidbody2D>();

        GetPlayerComponent();

        StartCoroutine(WhenToStart(_timeToStart));
    }

    private void FixedUpdate()
    {
        if (_offsetB)
            _offsetB = RefOffsetMoving();
        else
            RefMoveToPlayer();
    }

    #region Begin

    private void GetPlayerComponent()
    {
        _player = GameObject.Find("Player");
        _player.GetComponent<Player>().SetMortal += () => StartCoroutine(SetTimeMortal());
        _player.GetComponent<Player>().ResetPos += PlayerDie;
    }

    private IEnumerator WhenToStart(int seconds)
    {
        _onStart = true;
        _startPos = transform.position;

        RefDirectionSet(_player.transform.position);
        RefMoveToPlayer();

        yield return new WaitForSeconds(seconds);

        _onStart = false;
        _speed = 0.7f;
        transform.position = new Vector2(0, 1.025f);
        _direction = Random.Range(0, 2) == 0 ? new Vector2(-1, 0) : new Vector2(1, 0);

        _horizontal = !_horizontal;
        _vertical = !_vertical;
        _offset = transform.position;
    }

    #endregion

    #region Moving

    #region Moving/Move
    private void RefMoveToPlayer()
    {
        _animEnemy.SetFloat("Horizontal", _direction.x);
        _animEnemy.SetFloat("Vertical", _direction.y);
        _enemyRB2.MovePosition((Vector2)transform.position + (new Vector2(_direction.x, _direction.y) * _speed * Time.deltaTime));
    }

    #endregion

    #region Moving/Direction

    private void RefDirectionSet(Vector2 moveTo)
    {
        _horizontal = !_horizontal;
        _vertical = !_vertical;

        int r = Random.Range(0, 2) == 1 ? 1 : -1;
        _direction = _isDead ? (moveTo - (Vector2)transform.position) : (moveTo - (Vector2)transform.position) * r;
        _direction = _horizontal ? new Vector2(_direction.x, _direction.y = 0) : new Vector2(_direction.x = 0, _direction.y);

        _direction.x = _direction.x == 0 ? _direction.x : _direction.x > 0 ? _direction.x = 1f : _direction.x = -1f;
        _direction.y = _direction.y == 0 ? _direction.y : _direction.y > 0 ? _direction.y = 1f : _direction.y = -1f;
    }

    #endregion

    #region Moving/Offset

    private void RefOffSet()
    {
        if (_direction.y == 0)
            _offset = new Vector2(transform.position.x - _compens * _direction.x, transform.position.y);
        else
            _offset = new Vector2(transform.position.x, transform.position.y - _compens * _direction.y);

        _offsetB = true;
    }

    private bool RefOffsetMoving()
    {
        if (transform.position == (Vector3)_offset)
            return false;
        else
            transform.position = Vector2.MoveTowards(transform.position, _offset, Time.deltaTime * _speed);

        return true;
    }

    #endregion

    #endregion

    #region Death

    #region Death/Enemy

    private IEnumerator SetTimeMortal()
    {
        IsMortal = true;
        _animEnemy.SetBool("Mortal", IsMortal);

        yield return new WaitForSeconds(10);

        IsMortal = false;
        _animEnemy.SetBool("Mortal", IsMortal);
    }

    private void TimeToDie()
    {
        _animEnemy.SetBool("Dead", _isDead);
        gameObject.layer = _wayOutLayer;
        IsDead++;
    }

    #endregion

    #region Death/Player

    private void PlayerDie()
    {
        _isDead = false;
        IsMortal = false;

        _animEnemy.SetBool("Mortal", IsMortal);
        _animEnemy.SetBool("Dead", _isDead);

        StopAllCoroutines();

        gameObject.layer = _enemyLayer;

        IsDead = 0;

        _horizontal = true;
        _vertical = false;
        
        transform.position = _startPos;
        StartCoroutine(WhenToStart(_timeToStart));
    }

    #endregion

    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && IsMortal)
        {
            ScoreSet?.Invoke(100);
            _isDead = true;
            TimeToDie();
        }

        if (!collision.gameObject.CompareTag("Enemy"))
            RefOffSet();
        else
        {
            _direction *= -1;
            return;
        }

        if (!_onStart)
            RefDirectionSet(_player.transform.position);
        else if (_isDead)
            RefDirectionSet(_home);
        else
            _direction.y *= -1;
    }
}
