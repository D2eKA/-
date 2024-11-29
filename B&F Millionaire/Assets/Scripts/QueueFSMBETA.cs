using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyerMovement : MonoBehaviour
{
    [SerializeField] private Transform table; // ����, � �������� ���� ����������
    [SerializeField] private Transform hero; // ������� ��������
    [SerializeField] private List<Transform> waypoints = new List<Transform>(); // ����� ��� ��������� ����� � ����� �����
    [SerializeField] private List<Transform> queuePositions = new List<Transform>(); // ������� ��� �������
    [SerializeField] private float interactionDistance = 2f; // ��������� ��������������
    [SerializeField] private float moveSpeed = 5f; // �������� ��������
    [SerializeField] private float stopDistance = 0.1f; // ���������� ��� ���������

    private Rigidbody2D rb;
    private Vector2 movement;
    private Transform currentTarget; // ������� ����

    private static bool isTableOccupied = false; // ����, ������������ ��������� �����
    private static Queue<BuyerMovement> waitingQueue = new Queue<BuyerMovement>(); // ������� �����������

    private int waypointIndex = 0; // ������ ������� ����� ��������
    private bool reachedWaypoints = false; // ���� ���������� ��������

    private enum BuyerState
    {
        MovingToWaypoints,
        MovingToTable,
        WaitingInQueue,
        InteractingWithTable,
        Leaving
    }

    private BuyerState currentState;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = BuyerState.MovingToWaypoints;
        SetTarget(waypoints[0]); // ���������� ��������� � ������ �����
    }

    void Update()
    {
        switch (currentState)
        {
            case BuyerState.MovingToWaypoints:
                MoveToWaypoints();
                break;
            case BuyerState.MovingToTable:
                MoveToTarget();
                break;
            case BuyerState.WaitingInQueue:
                WaitForTable();
                break;
            case BuyerState.InteractingWithTable:
                WaitForHeroInteraction();
                break;
            case BuyerState.Leaving:
                LeaveScene();
                break;
        }
    }

    void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            rb.velocity = movement;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void SetTarget(Transform target)
    {
        currentTarget = target;
        movement = Vector2.zero;
    }

    private void MoveToWaypoints()
{
    if (waypoints.Count == 0 || waypointIndex >= waypoints.Count)
    {
        // ��� ����� �������� ��������
        currentState = BuyerState.Leaving; // ���������� ������
        return;
    }

    // ������������� ������� ����� �������� ��� ����
    SetTarget(waypoints[waypointIndex]);
    MoveToTarget();

    // ���������, ������ �� ���������� ������� �����
    if (Vector2.Distance(transform.position, waypoints[waypointIndex].position) <= stopDistance)
    {
        waypointIndex++; // ��������� � ��������� ����� ��������
    }
}

    private void MoveToTarget()
    {
        if (currentTarget == null) return;

        Vector2 direction = (currentTarget.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, currentTarget.position);

        if (distance <= stopDistance)
        {
            if (currentTarget == table)
            {
                if (isTableOccupied)
                {
                    JoinQueue();
                }
                else
                {
                    isTableOccupied = true;
                    currentState = BuyerState.InteractingWithTable;
                }
            }
        }
        else
        {
            movement = direction * moveSpeed;
        }
    }

    private void JoinQueue()
    {
        if (queuePositions.Count == 0)
        {
            Debug.LogError("��� ����� ��� �������!");
            currentState = BuyerState.Leaving;
            return;
        }

        foreach (Transform queuePoint in queuePositions)
        {
            if (!IsPositionOccupied(queuePoint)) // ���������, ������ �� �����
            {
                waitingQueue.Enqueue(this); // ��������� ���������� � �������
                SetTarget(queuePoint); // ������������� ����� ������� ��� ����
                currentState = BuyerState.WaitingInQueue; // �������� ���������
                Debug.Log($"���������� ����� ������� �� �������: {queuePoint.position}");
                return;
            }
        }

        // ���� ��� ����� ������, ���������� ������
        Debug.LogWarning("������� ���������! ���������� ������.");
        currentState = BuyerState.Leaving;
    }

    private void WaitForTable()
    {
        if (!isTableOccupied && waitingQueue.Peek() == this)
        {
            waitingQueue.Dequeue();
            currentState = BuyerState.MovingToTable;
            SetTarget(table);
        }
    }

    private void WaitForHeroInteraction()
    {
        if (Vector2.Distance(transform.position, hero.position) <= interactionDistance && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("���������� �������� �������������� �� ������.");
            isTableOccupied = false;
            currentState = reachedWaypoints ? BuyerState.Leaving : BuyerState.MovingToWaypoints;

            // ������� ���������� �� ������� ����� ���������� ��������������
            if (waitingQueue.Contains(this))
            {
                waitingQueue.Dequeue(); // ������� �������� ���������� �� �������
            }

            if (waitingQueue.Count > 0)
            {
                waitingQueue.Peek().NotifyTableAvailable(); // ���������� ���������� ����������
            }
        }
    }

    private void LeaveScene()
    {
        Debug.Log("���������� ������.");
        Destroy(gameObject);
    }

    private bool IsPositionOccupied(Transform position)
    {
        foreach (GameObject buyer in GameObject.FindGameObjectsWithTag("Buyer"))
        {
            if (Vector2.Distance(buyer.transform.position, position.position) <= stopDistance)
            {
                return true; // ����� ������
            }
        }
        return false; // ����� ��������
    }


    public void NotifyTableAvailable()
    {
        if (currentState == BuyerState.WaitingInQueue)
        {
            currentState = BuyerState.MovingToTable;
            SetTarget(table);
        }
    }
}
