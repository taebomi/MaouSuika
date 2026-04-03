namespace SOSG.System.UI
{
    public class ModalOverlay : BaseUI
    {
        protected override void AwakeAfter()
        {
            SetActive(false);
        }

        public void SetActive(bool value)
        {
            Canvas.enabled = value;
        }
    }
}