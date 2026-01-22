using UnityEngine;

namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Configuration class for UI layer rendering priority.
    /// Allows dynamic adjustment of layer sort orders at runtime.
    /// </summary>
    [CreateAssetMenu(fileName = "UiLayerConfig", menuName = "Immojoy/Lite Framework/UI Layer Config")]
    public class ImmoUiLayerConfig : ScriptableObject
    {
        [Header("UI Layer Sort Orders")]
        [Tooltip("Sort order for Background layer (lowest priority)")]
        [SerializeField] private int m_BackgroundSortOrder = 0;
        
        [Tooltip("Sort order for Normal layer")]
        [SerializeField] private int m_NormalSortOrder = 100;
        
        [Tooltip("Sort order for Popup layer")]
        [SerializeField] private int m_PopupSortOrder = 200;
        
        [Tooltip("Sort order for Top layer")]
        [SerializeField] private int m_TopSortOrder = 300;
        
        [Tooltip("Sort order for System layer (highest priority)")]
        [SerializeField] private int m_SystemSortOrder = 400;

        public int GetSortOrder(UiLayer layer)
        {
            return layer switch
            {
                UiLayer.Background => m_BackgroundSortOrder,
                UiLayer.Normal => m_NormalSortOrder,
                UiLayer.Popup => m_PopupSortOrder,
                UiLayer.Top => m_TopSortOrder,
                UiLayer.System => m_SystemSortOrder,
                _ => 0
            };
        }

        /// <summary>
        /// Validates that sort orders are in ascending order.
        /// </summary>
        private void OnValidate()
        {
            if (m_BackgroundSortOrder >= m_NormalSortOrder ||
                m_NormalSortOrder >= m_PopupSortOrder ||
                m_PopupSortOrder >= m_TopSortOrder ||
                m_TopSortOrder >= m_SystemSortOrder)
            {
                Debug.LogWarning("UI Layer sort orders should be in ascending order: Background < Normal < Popup < Top < System");
            }
        }
    }
}
