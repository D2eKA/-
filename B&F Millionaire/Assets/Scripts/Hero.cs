using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float interactionRadius = 1f; // ������ ��������������
    [SerializeField] private LayerMask interactableLayer;  // ���� ��� ��������, � �������� ����� �����������������
    private int maxInventorySize = 10;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    private List<string> inventory = new List<string>(); // ��������� ���������

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

    private void Update()
    {
        State = States.idle;

        // ������������
        if (Input.GetButton("Horizontal"))
            HorizontalMove();
        if (Input.GetButton("Vertical"))
            VerticalMove();

        // �������������� � ��������
        if (Input.GetKeyDown(KeyCode.E))
            Interact();
    }

    private void HorizontalMove()
    {
        State = States.move;
        Vector3 dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;
    }

    private void VerticalMove()
    {
        State = States.move;
        Vector3 dir = transform.up * Input.GetAxis("Vertical");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
    }

    public void AddToInventory(string itemName)
    {
        Debug.Log($"������� �������� ������� '{itemName}' � ���������.");

        if (inventory.Count < maxInventorySize)
        {
            inventory.Add(itemName);
            Debug.Log($"������� '{itemName}' �������� � ���������. ������� ������ ���������: {inventory.Count}");
        }
        else
        {
            Debug.Log("��������� ������, �� ���� �������� �������.");
        }
    }

    private void Interact()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactableLayer);

        if (hitColliders.Length > 0)
        {
            Debug.Log("������ ��� �������������� ������.");

            // ���������, ���� �� �� ������� ��������� IInteractable
            IInteractable interactable = hitColliders[0].GetComponent<IInteractable>();
            if (interactable != null)
            {
                Debug.Log("������ ��������� IInteractable. ��������� ��������������.");
                interactable.OnInteract();
            }
            else
            {
                Debug.Log("������ ������, �� �� ��������� IInteractable.");
            }
        }
        else
        {
            Debug.Log("��� �������� ��� �������������� � �������.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}

public enum States
{
    idle,
    move
}



public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName; // �������� ��������

    public void OnInteract()
    {
        Debug.Log($"������� ��������: {itemName}");
    }

    public string GetItemName()
    {
        return itemName;
    }
}