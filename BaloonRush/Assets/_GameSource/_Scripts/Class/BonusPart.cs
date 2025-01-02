using UnityEngine;

public class BonusPart : MonoBehaviour
{
    #region Variables

    [SerializeField] int coin;
    [SerializeField] Color myColor;

    MeshRenderer renderer;

    #endregion

    #region MonoBehaviour Callbacks

    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && BonusLine.Instance.isReady)
        {
            Destroy(GetComponent<BoxCollider>());
            GameObject fx = Instantiate(GameManager.Instance.coinFx,transform.position - Vector3.forward,Quaternion.identity);
            Paint();
            Destroy(this);
        }
    }

    #endregion

    #region Other Methods

    void Paint()
    {
        renderer.material.color = myColor;
        GameManager.Instance.SetBonus(coin);
    }

    #endregion
}
