using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public bool isGameOver = false;
    public float moveSpeed = 5f;
    public float rotationSpeed = 300f;
    public float gridSize = 1f; // Sesuaikan dengan jarak antar petak di Unity

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
        // Menembakkan raycast dari sedikit di atas lantai agar kena badan musuh
        Vector3 rayStart = transform.position + Vector3.up * 0.2f; 
        
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
        Vector3 rayStartForward = transform.position + Vector3.up * 0.2f;
        
        if (Physics.Raycast(rayStartForward, transform.forward, out hitForward, gridSize))
        {
            if (hitForward.collider.CompareTag("Obstacle") || hitForward.collider.CompareTag("EnemyLevel1"))
            {
                return true; // Terhalang kotak/tembok/musuh
            }
        }

        // --- 2. Cek Lantai (Upper Floor) di Petak Tujuan ---
        Vector3 targetPos = transform.position + transform.forward * gridSize;
        Vector3 rayStartDown = targetPos + Vector3.up * 0.5f; // Posisi sensor di atas target petak
        RaycastHit hitDown;

        // Menembakkan sinar ke bawah (Vector3.down) sejauh 1 unit
        if (Physics.Raycast(rayStartDown, Vector3.down, out hitDown, 1f))
        {
            // Jika yang terdeteksi di bawah BUKAN Upper Floor, maka block!
            if (!hitDown.collider.CompareTag("Floor") && !hitDown.collider.CompareTag("Goal"))
            {
                return true; 
            }
        }
        else
        {
            // Jika tidak menabrak apa pun di bawahnya (berarti jurang kosong)
            return true;
        }

        return false; // Depan kosong, dan bawahnya ada lantai aman
    }

    private IEnumerator MoveToPosition(Vector3 targetPos)
    {
        isMoving = true;
        
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
        }

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
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
        // Cek apakah benda yang disentuh punya Tag "Goal"
        if (other.CompareTag("Goal"))
        {
            Debug.Log("LEVEL SELESAI! PEMAIN MENANG! 🎉");
            
            // 1. Ubah status game agar perintah lain dihentikan
            isGameOver = true;
            
            // 2. Putar animasi JOGET!
            if (animator != null)
            {
                // Memanggil trigger "Win" yang kita buat di Unity
                animator.SetTrigger("Win"); 
            }

            // Nanti di sini kita bisa memunculkan UI Panel "Victory" atau pindah level
        }
    }
}
