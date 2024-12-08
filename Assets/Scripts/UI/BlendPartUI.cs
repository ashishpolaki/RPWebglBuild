using CharacterCustomisation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BlendPartUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI partName;
        [SerializeField] private Slider slider;
        private BlendShapePartData blendShapePartData;

        #region Unity methods
        private void OnEnable()
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        private void OnDisable()
        {
            slider.onValueChanged.RemoveAllListeners();
        }
        #endregion

        private void OnSliderValueChanged(float value)
        {
          //  CharacterCustomisationManager.Instance.SetBlendShapes(blendShapePartData, value);
        }

        public void SetData(BlendShapePartData _blendShapePartData)
        {
            slider.value = 0;
            blendShapePartData = _blendShapePartData;
            partName.text = blendShapePartData.name;
        }
    }
}