using UnityEngine;

public class FuelCan : MonoBehaviour
{

    public Transform balonModel;
    Vector3 pos = new Vector3(0f, 0.5f, 0f);

    private void Start()
    {
        Instantiate(balonModel, transform.position - pos, Quaternion.identity, this.transform);
    }   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AI"))
        {
            Destroy(GetComponent<BoxCollider>());
            other.GetComponent<Character>().GetFuel();
            Destroy(gameObject);
        }

        if (other.CompareTag("Player"))
        {
            FindObjectOfType<PlayerControl>().PlusSpawner();
        }
    }
}
