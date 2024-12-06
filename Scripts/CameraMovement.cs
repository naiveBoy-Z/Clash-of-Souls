using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Tốc độ di chuyển của camera

    void Update()
    {
        // Lấy giá trị di chuyển theo trục X và Y
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D hoặc trái/phải
        float moveY = Input.GetAxisRaw("Vertical");   // W/S hoặc lên/xuống

        // Tính toán vị trí mới
        Vector3 moveDirection = new Vector3(moveX, moveY, 0); // Z = 0 để giữ camera ở cùng một mặt phẳng
        transform.position += moveDirection * moveSpeed * Time.deltaTime; // Thay đổi vị trí
    }
}
