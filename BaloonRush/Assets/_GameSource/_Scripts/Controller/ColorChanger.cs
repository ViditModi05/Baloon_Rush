using UnityEngine;

public class ColorChanger : MonoBehaviour
{

    public Color[] colors;
    private MeshRenderer balonColor;

    private void Start()
    {
        balonColor = GetComponent<MeshRenderer>();
        int x = Random.Range(0, 6);
        balonColor.materials[0].color = colors[x];
    }

}