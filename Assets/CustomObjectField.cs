namespace DefaultNamespace
{
    using UnityEngine;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;
    using UnityEditor;
    public class CustomObjectField : ObjectField
    {
        private Image _thumbnailImage;

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