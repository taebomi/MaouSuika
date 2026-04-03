using System;
using FMODUnity;
using SOSG.System.Audio;
using SOSG.System.Vibration;
using UnityEngine;

namespace SOSG.Stage.Suika
{
    public class SuikaPhysicsComponent : MonoBehaviour
    {
        [Header("Component")]
        [SerializeField] private SuikaObject suika;

        [field: SerializeField] public Rigidbody2D Rb { get; private set; }
        [SerializeField] private Collider2D coll;

        [Header("etc")]
        [SerializeField] private EventReference collideSfx;

        public bool IsGrounded { get; private set; }

        public int ColliderID => coll.GetInstanceID();

        public void OnLoaded()
        {
            Rb.bodyType = RigidbodyType2D.Dynamic;
            Rb.simulated = false;

            IsGrounded = false;
        }


        public void OnShot(Vector2 vel)
        {
            Rb.simulated = true;
            Rb.linearVelocity = vel;
        }

        public void OnMerging()
        {
            Rb.bodyType = RigidbodyType2D.Kinematic;
            Rb.linearVelocity = Vector2.zero;
        }

        public void OnMerged()
        {
            Rb.bodyType = RigidbodyType2D.Dynamic;
            Rb.simulated = true;
            IsGrounded = true;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (suika.Player.Shooter.LastShotSuika == suika)
            {
                PlayCollisionEffect();
            }

            if (suika.Player.Collection.TryGetSuika(other.collider.GetInstanceID(), out var collidedSuika))
            {
                OnSuikaCollided(collidedSuika);
            }
            else if (other.gameObject.CompareTag("Ground"))
            {
                IsGrounded = true;
            }
        }

        private void OnSuikaCollided(SuikaObject collidedSuika)
        {
            if (collidedSuika.PhysicsComponent.IsGrounded)
            {
                IsGrounded = true;
            }

            if (suika.Tier != collidedSuika.Tier)
            {
                return;
            }

            if (suika.State is SuikaState.Merging || collidedSuika.State is SuikaState.Merging)
            {
                return;
            }

            suika.Player.Merger.Merge(suika, collidedSuika);
        }

        private void PlayCollisionEffect()
        {
            AudioSystemHelper.PlaySfx(collideSfx);
            VibrationEventBus.PlayConstant(0.05f, 0.1f, 0.075f);
        }
    }
}