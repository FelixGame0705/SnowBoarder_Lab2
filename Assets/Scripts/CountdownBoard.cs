using System.Collections;
using UnityEngine;

public class CountdownBoard : MonoBehaviour
{
    [SerializeField]
    private float countdownTime = 30f; // Thời gian đếm ngược mặc định là 5 giây

    void Start()
    {
        // Bắt đầu coroutine đếm ngược khi script được kích hoạt
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        // Đợi theo thời gian đã định
        yield return new WaitForSeconds(countdownTime);

        // Sau khi đợi xong, disable game object
        gameObject.SetActive(false);

        Debug.Log("Object has been disabled after " + countdownTime + " seconds");
    }
}
