using UnityEngine;
using UnityEngine.Serialization;

#pragma warning disable

namespace ABI.CCK.Components
{
    public class CVRGIMaterialUpdater : MonoBehaviour
    {
        [SerializeField] bool updateEveryFrame;
        private static Renderer m_Renderer { get; set; }

        private Renderer Renderer
        {
            get
            {
                if (!m_Renderer)
                {
                    m_Renderer = GetComponent<Renderer>();
                }
                return m_Renderer;
            }
        }

        private void Update()
        {
            if (Renderer == null || !updateEveryFrame) return;
            Renderer.UpdateGIMaterials();
        }
    
    }
}