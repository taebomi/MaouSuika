using System.Collections;
using UnityEngine;

namespace SOSG.Stage
{
    public class GashaponShooterCooldownTimer : MonoBehaviour
    {
        [SerializeField] private VoidEventSO shootEventSO;

        [SerializeField] private BoolVariableSO isCooldownVarSO;

        private WaitForSeconds _waitForCooldown;
    
        private const float ShootCooldown = 0.5f;

        private void Awake()
        {
            shootEventSO.OnEventRaised += OnShot;
        
            isCooldownVarSO.Initialize(false);
        
            _waitForCooldown = new WaitForSeconds(ShootCooldown);
        }

        private void OnDestroy()
        {
            shootEventSO.OnEventRaised -= OnShot;
        }

        private void OnShot()
        {
            StartCoroutine(CheckCooldown());
        }

        private IEnumerator CheckCooldown()
        {
            isCooldownVarSO.Set(true);
            yield return _waitForCooldown;
            isCooldownVarSO.Set(false);
        }
    }
}