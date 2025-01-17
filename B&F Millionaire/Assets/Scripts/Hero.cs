using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hero : MonoBehaviour
{
    public int balance = 0;
    public TextMeshProUGUI moneyText;

    [SerializeField] private float speed = 1f;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    private void Start()
    {
        UpdateMoneyDisplay();
    }

    public void AddMoney(int amount)
    {
        balance += amount;
        UpdateMoneyDisplay(); 
    }
    private void UpdateMoneyDisplay()
    {
        moneyText.text = balance.ToString(); 
    }
    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        State = States.idle;
        // Передвижение
        if (Input.GetButton("Horizontal"))
            HorizontalMove();
        if (Input.GetButton("Vertical"))
            VerticalMove();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            State = States.interact;
        if (Input.GetKeyDown(KeyCode.Space)) //проверка изменений , заглушка
        {
            AddMoney(10);
        }
    }

    private void HorizontalMove()
    {
        State = States.move;
        Vector2 dir = transform.right * Input.GetAxis("Horizontal");
        Vector2 trans_pos = (Vector2)transform.position;
        transform.position = Vector2.MoveTowards(transform.position, trans_pos + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;
    }

    private void VerticalMove()
    {
        State = States.move;
        Vector2 dir = transform.up * Input.GetAxis("Vertical");
        Vector2 trans_pos = (Vector2)transform.position;
        transform.position = Vector3.MoveTowards(transform.position, trans_pos + dir, speed * Time.deltaTime);
    }
}

public enum States
{
    idle,
    move,
    interact
}
