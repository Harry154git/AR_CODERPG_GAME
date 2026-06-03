using System.Collections;
using UnityEngine;

public class Enemy_Level_1 : MonoBehaviour
{
    public Animator animator; // Wadah untuk Animator katak
    public float deathDelay = 1.0f; // Waktu tunggu sampai animasi mati selesai (misal 1 detik)

    public Transform playerTarget; // Tarik objek Player ke sini lewat Inspector
    public float gridSize = 1f; // Sesuaikan dengan ukuran 1 ubin di Unity kamu (biasanya 1 atau 2)
    public float rotationSpeed = 300f; // Kecepatan muter kodok

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Kalau player tidak ada, jangan lakukan apa-apa
        if (playerTarget == null) return;

        // Cari arah dari katak ke player
        Vector3 directionToPlayer = playerTarget.position - transform.position;
        directionToPlayer.y = 0; // Abaikan perbedaan tinggi (Y)

        // Ambil jarak absolut (angka positif) dari sumbu X dan Z
        float distanceX = Mathf.Abs(directionToPlayer.x);
        float distanceZ = Mathf.Abs(directionToPlayer.z);

        // Toleransi kecil (0.1f) karena angka di Unity kadang tidak bulat sempurna (misal 0.999)
        bool isSejajarX = (distanceX <= gridSize + 0.1f && distanceX >= gridSize - 0.1f) && (distanceZ < 0.1f);
        bool isSejajarZ = (distanceZ <= gridSize + 0.1f && distanceZ >= gridSize - 0.1f) && (distanceX < 0.1f);

        // Jika player ada di jarak 1 petak DAN lurus (isSejajarX atau isSejajarZ)
        if (isSejajarX || isSejajarZ)
        {
            // Hitung rotasi yang pas untuk menghadap player
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            
            // Putar katak secara mulus ke arah player
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    

    // Fungsi ini dipanggil oleh Player saat attack()
    public void TakeDamage()
    {
        Debug.Log("Katak terkena serangan!");
        
        // Memutar animasi mati
        if (animator != null)
        {
            animator.SetTrigger("Die"); // "Die" ini nanti kita buat di Unity
        }

        // Hapus katak, tapi tunggu animasinya selesai dulu
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        // Menunggu selama waktu deathDelay
        yield return new WaitForSeconds(deathDelay);
        
        // Setelah animasi selesai, baru hancurkan objeknya agar jalan terbuka
        Destroy(gameObject);
    }
}
