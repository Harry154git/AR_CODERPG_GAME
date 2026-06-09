using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public bool isGameOver = false;
    public float moveSpeed = 5f;
    public float rotationSpeed = 300f;
    public float gridSize = 1f;

    private bool isMoving = false;

    // --- FUNGSI AKSI PLAYER ---

    public IEnumerator MoveForward()
    {
        if (isMoving || isGameOver) yield break;

        if (!IsPathBlocked())
        {
            Vector3 targetPos = transform.position + transform.forward * gridSize;
            yield return StartCoroutine(MoveToPosition(targetPos));
        }
        else
        {
            Debug.Log("Jalan di depan terhalang!");
            yield return new WaitForSeconds(0.5f); // Jeda jika nabrak
        }
    }

    public IEnumerator TurnLeft()
    {
        if (isMoving || isGameOver) yield break;
        yield return StartCoroutine(RotateToAngle(-90f));
    }

    public IEnumerator TurnRight()
    {
        if (isMoving || isGameOver) yield break;
        yield return StartCoroutine(RotateToAngle(90f));
    }

    public IEnumerator Attack()
    {
        if (isMoving || isGameOver) yield break;
        Debug.Log("Player menebas ke depan!");

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        yield return new WaitForSeconds(0.4f);
        
        RaycastHit hit;
        // Laser ditaruh di dada (Otomatis menyesuaikan skala raksasa/kecil)
        Vector3 rayStart = transform.position + Vector3.up * (transform.lossyScale.y * 0.5f); 
        
        if (Physics.Raycast(rayStart, transform.forward, out hit, gridSize))
        {
            if (hit.collider.CompareTag("EnemyLevel1"))
            {
                Enemy_Level_1 enemyScript = hit.collider.GetComponent<Enemy_Level_1>();
                if (enemyScript != null) enemyScript.TakeDamage();
            }
        }
        yield return new WaitForSeconds(0.6f); // Jeda animasi serang
    }

    // --- FUNGSI PENDUKUNG (LOGIKA GERAK & SENSOR) ---

    private bool IsPathBlocked()
    {
        // --- 1. Cek Tembok / Rintangan di Depan ---
        RaycastHit hitForward;
        // Mata Depan dinaikkan setinggi dada agar tidak menggaruk ubin
        Vector3 rayStartForward = transform.position + Vector3.up * (transform.lossyScale.y * 0.5f);
        
        if (Physics.Raycast(rayStartForward, transform.forward, out hitForward, gridSize))
        {
            if (hitForward.collider.CompareTag("Obstacle") || hitForward.collider.CompareTag("EnemyLevel1"))
            {
                return true; // Terhalang kotak/tembok/musuh
            }
        }

        // --- 2. Cek Lantai (Upper Floor) di Petak Tujuan ---
        Vector3 targetPos = transform.position + transform.forward * gridSize;
        // Sensor bawah ditembakkan dari atas kepala jauh ke bawah agar pasti kena lantai
        Vector3 rayStartDown = targetPos + Vector3.up * (transform.lossyScale.y * 2f); 
        RaycastHit hitDown;

        // Tembak laser sejauh 5x tinggi badan
        if (Physics.Raycast(rayStartDown, Vector3.down, out hitDown, transform.lossyScale.y * 5f))
        {
            if (!hitDown.collider.CompareTag("Floor") && !hitDown.collider.CompareTag("Goal"))
            {
                return true; 
            }
        }
        else
        {
            return true; // Jurang kosong
        }

        return false;
    }

    private IEnumerator MoveToPosition(Vector3 targetPos)
    {
        isMoving = true;
        
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }

        // Menyesuaikan speed agar Playernya tidak jalan super lambat saat ukurannya raksasa
        float currentSpeed = moveSpeed;
        if (gridSize > 1) currentSpeed = moveSpeed * (gridSize / 2f);

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;

        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private IEnumerator RotateToAngle(float angle)
    {
        isMoving = true;
        Quaternion targetRot = transform.rotation * Quaternion.Euler(0, angle, 0);
        while (Quaternion.Angle(transform.rotation, targetRot) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = targetRot;
        isMoving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            Debug.Log("LEVEL SELESAI! PEMAIN MENANG! 🎉");
            isGameOver = true;
            
            if (animator != null)
            {
                animator.SetTrigger("Win"); 
            }
        }
    }
}