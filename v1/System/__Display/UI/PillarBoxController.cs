using UnityEngine;

namespace SOSG.System.Display
{
    public class PillarBoxController : MonoBehaviour
    {
        [SerializeField] private RectTransform leftPillarBox, rightPillarBox;

        public void SetUp()
        {
            DisplayData.PillarBoxEnabled += SetPillarBox;
            SetPillarBox(DisplayData.IsPillarBoxOn);
        }

        public void TearDown()
        {
            DisplayData.PillarBoxEnabled -= SetPillarBox;
        }

        private void SetPillarBox(bool isOn)
        {
            if (isOn is false)
            {
                gameObject.SetActive(false);
                return;
            }

            leftPillarBox.offsetMax = new Vector2(DisplayData.PillarBoxWidth, 0f);
            rightPillarBox.offsetMin = new Vector2(-DisplayData.PillarBoxWidth, 0f);
            gameObject.SetActive(true);
        }
    }
}