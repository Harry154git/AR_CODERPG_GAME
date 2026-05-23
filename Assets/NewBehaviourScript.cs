using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
    [Tooltip("Kecepatan rotasi objek saat layar di-swipe")]
    public float rotationSpeed = 0.5f;

    void Update()
    {
        // Memastikan ada input sentuhan dari layar smartphone
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Memeriksa apakah ada pergerakan jari yang berstatus Moved atau sedang menggeser.
            if (touch.phase == TouchPhase.Moved)
            {
                // Menangkap seberapa besar geseran jari pada sumbu X atau horizontal.
                float swipeDeltaX = touch.deltaPosition.x;

                // Merotasi objeknya. kemudian, nilai negatif (-) digunakan agar arah putaran terasa natural 
                // terhadap arah geseran jari. selanjutnya, menggunakan Space.Self untuk rotasi secara lokal.
                transform.Rotate(Vector3.up, -swipeDeltaX * rotationSpeed, Space.Self);
            }
        }
    }
}