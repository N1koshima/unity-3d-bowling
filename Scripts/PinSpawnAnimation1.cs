using UnityEngine;
using System.Collections;

public class PinSpawnAnimation1 : MonoBehaviour
{
    private Vector3 targetPos;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetPos = transform.position;

        transform.position = targetPos + Vector3.down * 2f;
        rb.isKinematic = true;

        StartCoroutine(SpawnAnim());
    }

    IEnumerator SpawnAnim()
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            transform.position = Vector3.Lerp(targetPos + Vector3.down * 2f, targetPos, t);

            yield return null;
        }

        rb.isKinematic = false;
    }
}
