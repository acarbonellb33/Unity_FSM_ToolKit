#if UNITY_EDITOR
namespace FSM.Utilities
{
    using UnityEngine;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;
    using UnityEditor;
    /// <summary>
    /// Custom ObjectField class to display object thumbnails.
    /// </summary>
    public class CustomObjectField : ObjectField
    {
        private readonly Image _thumbnailImage;
        /// <summary>
        /// Constructor to initialize the custom object field.
        /// </summary>
        /// <param name="label">Label for the object field.</param>
        public CustomObjectField(string label) : base(label)
        {
            _thumbnailImage = new Image();
            Add(_thumbnailImage);
            RegisterCallback<ChangeEvent<Object>>(OnValueChanged);
        }

        private void OnValueChanged(ChangeEvent<Object> evt)
        {
            UpdateThumbnail(evt.newValue);
        }

        private void UpdateThumbnail(Object obj)
        {
            Texture2D thumbnailTexture = null;
            if (obj != null)
            {
                thumbnailTexture = AssetPreview.GetAssetPreview(obj);
            }
            _thumbnailImage.image = thumbnailTexture;
        }
    }
}
#endif