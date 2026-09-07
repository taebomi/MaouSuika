using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class UnitVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer bodyRenderer;
        [SerializeField] private GameObject shadow;

        private Transform _bodyTransform;

        private FlashEffect _flashEffect;

        private UnitSpriteAnimation _animation;
        private float _elapsed;
        private int _frame;

        public int Frame => _frame;
        public float NormalizedTime => _animation.Duration > 0f ? _elapsed / _animation.Duration : 0f;
        public bool IsFinished => !_animation.Loop && _elapsed >= _animation.Duration;

        private void Awake()
        {
            _bodyTransform = bodyRenderer.transform;
            _flashEffect = new FlashEffect(bodyRenderer);
        }

        public void Play(UnitSpriteAnimation ani)
        {
            _animation = ani;
            ApplyFlipX(ani.FlipX);

            _elapsed = 0f;
            _frame = -1;
            Apply(0);
        }

        public void Tick(float deltaTime)
        {
            if (_animation.Frames is not { Length: > 0 }) return;

            _elapsed += deltaTime;
            if (_animation.Loop && _animation.Duration > 0f) _elapsed %= _animation.Duration;
            Apply(FrameAt(_elapsed));
        }

        public void ResetStates()
        {
            _animation = default;
            _elapsed = 0f;
            _frame = -1;
            bodyRenderer.sprite = null;

            SetBodyOffsetY(0f);
            ClearFlash();
            SetColor(Color.white);
        }

        public void Flash()
        {
            _flashEffect.Play();
        }

        public void ClearFlash()
        {
            _flashEffect.Clear();
        }

        public void SetColor(Color color)
        {
            bodyRenderer.color = color;
        }

        public void SetShadowVisible(bool visible)
        {
            shadow.SetActive(visible);
        }

        public void SetSortingLayerID(int id)
        {
            bodyRenderer.sortingLayerID = id;
        }

        public void SetSortingOrder(int order)
        {
            bodyRenderer.sortingOrder = order;
        }

        public void SetSorting(int id, int order)
        {
            SetSortingLayerID(id);
            bodyRenderer.sortingOrder = order;
        }

        public void SetBodyOffsetY(float y)
        {
            var pos = _bodyTransform.localPosition;
            pos.y = y;
            _bodyTransform.localPosition = pos;
        }

        private int FrameAt(float elapsed)
        {
            var frame = Mathf.FloorToInt(elapsed * _animation.FPS);
            var count = _animation.Frames.Length;

            return _animation.Loop ? frame % count : Mathf.Clamp(frame, 0, count - 1);
        }

        private void Apply(int frame)
        {
            if (_frame == frame) return;

            _frame = frame;
            bodyRenderer.sprite = _animation.Frames[frame];
        }

        private void ApplyFlipX(bool flipX)
        {
            var scale = transform.localScale;
            var absX = Mathf.Abs(scale.x);
            scale.x = flipX ? -absX : absX;
            transform.localScale = scale;
        }
    }
}