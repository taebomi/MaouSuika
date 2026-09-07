using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SoulOrbQueueView : MonoBehaviour
    {
        [SerializeField] private SoulOrbQueueSlot[] slots;

        private SoulOrbQueueViewModel _vm;

        private IDisposable _subscription;

        public void Setup(SoulOrbQueueViewModel vm)
        {
            _vm = vm;

            _subscription?.Dispose();
            _subscription = _vm.RedrawRequested.Subscribe(_ => Redraw());
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }

        private void Redraw()
        {
            for (var i = 0; i < _vm.PreviewCount; i++)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].Set(_vm.GetSlotData(i));
            }

            for (var i = _vm.PreviewCount; i < slots.Length; i++)
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }
}