using System.Collections;
using UnityEngine;

public class Enemy_Level_1 : MonoBehaviour
{
    public Animator animator; // Wadah untuk Animator katak
    public float deathDelay = 1.0f; // Waktu tunggu sampai animasi mati selesai

    public Transform playerTarget; // Tarik objek Player ke sini lewat Inspector
    public float gridSize = 1f; // Sesuaikan dengan ukuran 1 ubin di Unity
    public float rotationSpeed = 300f; // Kecepatan muter kodok

    void Start()
    {
        
    }

    void Update()
    {
        if (playerTarget == null) return;

        Vector3 directionToPlayer = playerTarget.position - transform.position;
        directionToPlayer.y = 0; 

        float distanceX = Mathf.Abs(directionToPlayer.x);
        float distanceZ = Mathf.Abs(directionToPlayer.z);

        // --- PERUBAHAN DI SINI ---
        // Membuat angka toleransi otomatis menyesuaikan gridSize (misal 20% dari grid)
        float tolerance = gridSize * 0.2f; 

        bool isSejajarX = (distanceX <= gridSize + tolerance && distanceX >= gridSize - tolerance) && (distanceZ < tolerance);
        bool isSejajarZ = (distanceZ <= gridSize + tolerance && distanceZ >= gridSize - tolerance) && (distanceX < tolerance);

        if (isSejajarX || isSejajarZ)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage()
    {
        Debug.Log("Katak terkena serangan!");
        
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }
}