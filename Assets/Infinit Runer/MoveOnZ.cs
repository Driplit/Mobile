using UnityEngine;

public class MoveOnZ : MonoBehaviour
{
    [SerializeField] float speedOfRoad;

    void Update()
    {
        transform.position += new Vector3(0, 0, -speedOfRoad) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
    }
}
