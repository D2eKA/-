using UnityEngine;




public class Storage : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemName = "������"; // �������� ��������
    [SerializeField] private int itemCount = 1; // ���������� ���������

    private bool isPlayerNearby = false;
    private Hero player;

    public void OnInteract()
    {
        if (isPlayerNearby && player != null)
        {
            Debug.Log($"�������� ���������� �������� '{itemName}' � ���������.");

            // ��������� ������� � ���������
            for (int i = 0; i < itemCount; i++)
            {
                player.AddToInventory(itemName);
            }
            Debug.Log($"��������� {itemCount} '{itemName}' � ���������.");

            // ����� ���� ��� ������� ��������, ����� ���������� ������ (���� ����������)
            // Destroy(gameObject);
        }
        else
        {
            Debug.Log("���������� ����������������� � ��������.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            player = collision.GetComponent<Hero>();
            Debug.Log("�������� ����� � ������ �������������� � ��������.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            player = null;
            Debug.Log("�������� ������� ������ �������������� � ��������.");
        }
    }
}
