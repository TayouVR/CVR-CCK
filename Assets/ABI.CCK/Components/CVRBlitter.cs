using UnityEngine;

public class CVRBlitter : MonoBehaviour
{
#pragma warning disable CS0414
    [SerializeField] RenderTexture originTexture = null;
    [SerializeField] RenderTexture destinationTexture = null;
    [SerializeField] Material blitMaterial = null;
#pragma warning restore CS0414
    public bool clearEveryFrame;
}
