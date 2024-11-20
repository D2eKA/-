using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokypatelDviz : MonoBehaviour
{
    [SerializeField] private List<Transform> TargetPoint = new List<Transform>(); // ������ ����� 
    public float moveSpeed = 5f; // �������� �������� 
    public float stopDistance = 0.1f; // ���������� ��� ��������� 

    private Rigidbody2D rb;
    private Vector2 movement;
    private int currentTargetIndex = 0; // ������ ������� ����� 

    void Start()
    {
        // �������� ��������� Rigidbody2D 
        rb = GetComponent<Rigidbody2D>();

        // ��������, ��� ������ �� ���� 
        if (TargetPoint.Count > 0)
        {
            // ������������� ������ ���� 
            SetTargetPoint(TargetPoint[currentTargetIndex]);
        }
    }

    void Update()
    {
        // ���� ������ ����� ����, ������ �� ������ 
        if (TargetPoint.Count == 0) return;

        // ��������� ����������� � ������� ����� 
        Vector2 direction = TargetPoint[currentTargetIndex].position - transform.position;

        // ���� ��� ������ � ������� �����, ��������� � ��������� 
        if (direction.magnitude <= stopDistance)
        {
            NextTarget();
        }
        else
        {
            // ����������� ����������� � ��������� �������� 
            movement = direction.normalized * moveSpeed;
        }
    }

    void FixedUpdate()
    {
        // ����������� ���� 
        if (movement != Vector2.zero)
        {
            rb.velocity = movement;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    // ������������� ����� ����� ���� 
    private void SetTargetPoint(Transform newTarget)
    {
        if (newTarget != null)
        {
            movement = Vector2.zero;
        }
    }

    // ������� � ��������� ����� � ������ 
    private void NextTarget()
    {
        currentTargetIndex++;

        // ���� �������� ����� ������, ������� ������ 
        if (currentTargetIndex >= TargetPoint.Count)
        {
            Destroy(gameObject); // ���������� ������ 
            return;
        }
        // ������������� ����� ���� 
        SetTargetPoint(TargetPoint[currentTargetIndex]);
    }
    
}