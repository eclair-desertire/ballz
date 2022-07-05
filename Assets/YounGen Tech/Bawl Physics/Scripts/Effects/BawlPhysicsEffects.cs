using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Handles the example visual effects for the ball.
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Effects/Effects Controller"), DisallowMultipleComponent]
	public class BawlPhysicsEffects : MonoBehaviour {

		Grapple grapple;
		Magnet magnet;

		CollisionDetectionBase _collisionDetection;
		public CollisionDetectionBase CollisionDetection {
			get {
				if(!_collisionDetection) _collisionDetection = GetComponent<CollisionDetectionBase>();

				return _collisionDetection;
			}
		}

		public Animator colorAnimator;

		[Tooltip("Multiply the angular velocity that is sent to the animator")]
		public float angularVelocityMultiplier = 1;

		public LineRenderer grappleLineRendererPrefab;
		List<LineRenderer> grappleLineRenderers = new List<LineRenderer>();

		public GameObject boostEffect;
		public GameObject explosionEffect;

		void Start() {
			GetAbilityComponents();
		}

		void Update() {
			if(colorAnimator) {
				colorAnimator.SetBool("Magnet", magnet ? magnet.IsUsing : false);

				if(GetComponent<Rigidbody>())
					colorAnimator.SetFloat("Speed", Mathf.Min((GetComponent<Rigidbody>().angularVelocity.magnitude / GetComponent<Rigidbody>().maxAngularVelocity) * angularVelocityMultiplier, 1));
			}

			if(grapple && grapple.IsUsing)
				for(int i = 0; i < grappleLineRenderers.Count && i < grapple.MaxGrappleTargets; i++) {
					Grapple.GrappleTarget grappleTarget = grapple.GetTarget(i);

					if(grappleTarget.Target && grappleTarget.Enabled) {
						grappleLineRenderers[i].SetPosition(0, transform.position);
						grappleLineRenderers[i].SetPosition(1, grappleTarget.WorldAttachPoint);
					}
				}
		}

		void GetAbilityComponents() {
			if(!grapple) grapple = GetComponent<Grapple>();
			if(!magnet) magnet = GetComponent<Magnet>();
		}

		public void InstantiateEffect(GameObject effect) {
			if(effect) {
				if(GetComponent<Rigidbody>().angularVelocity == Vector3.zero) return;

				Quaternion rotation = Quaternion.identity;
				Vector3 direction = GetRollDirection();

				if(direction != Vector3.zero)
					rotation = Quaternion.LookRotation(direction);

				Instantiate(effect, transform.position, rotation);
			}
		}

		public Vector3 GetRollDirection() {
			Vector3 direction = GetComponent<Rigidbody>().angularVelocity.normalized;

			if(direction == Vector3.zero || CollisionDetection.StickyNormals.Value == Vector3.zero)
				return Vector3.zero;

			Quaternion angularDirection = Quaternion.LookRotation(direction.normalized, CollisionDetection.StickyNormals.Value);
			return angularDirection * Vector3.left;
		}

		public void BoostEffect(AbilityBase ability) {
			if(boostEffect) {
				Boost boost = ability as Boost;

				if(boost && boost.IsUsing) {
					if(boost.AngularVelocityIsDirection)
						if(GetComponent<Rigidbody>().angularVelocity == Vector3.zero)
							return;

					if(boost.BoostWhileColliding && !boost.CollisionDetection.IsColliding)
						return;

					if(boost.ChargeWhileColliding && boost.ChargePower == 0)
						return;

					Quaternion rotation = Quaternion.identity;
					Vector3 direction = boost.ForwardBoost;

					if(direction != Vector3.zero)
						rotation = Quaternion.LookRotation(direction);

					GameObject effect = (GameObject)Instantiate(boostEffect, transform.position, rotation);
					ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();

					if(particleSystem) 
						StartCoroutine("SetupBoostParticles", new BoostParticlesInfo(direction, particleSystem));
				}
			}
		}

		IEnumerator SetupBoostParticles(BoostParticlesInfo info) {
			yield return new WaitForFixedUpdate();
			float velocity = GetComponent<Rigidbody>().velocity.magnitude;
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[info.ParticleSystem.maxParticles];

			int count = info.ParticleSystem.GetParticles(particles);

			for(int i = 0; i < count; i++) {
				ParticleSystem.Particle particle = particles[i];
				particle.velocity = info.Direction * velocity;
				particles[i] = particle;
			}

			info.ParticleSystem.SetParticles(particles, count);
		}


		public void GrappleCreateRope(AbilityBase ability) {
			Grapple grapple = ability as Grapple;

			if(grapple) {
				while(grappleLineRenderers.Count < grapple.MaxGrappleTargets) {
					LineRenderer lineRenderer = Instantiate(grappleLineRendererPrefab) as LineRenderer;

					lineRenderer.transform.SetParent(transform, false);
					grappleLineRenderers.Add(lineRenderer);
				}

				for(int i = 0; i < grapple.MaxGrappleTargets; i++) {
					Grapple.GrappleTarget grappleTarget = grapple.GetTarget(i);

					grappleLineRenderers[i].enabled = grappleTarget.Enabled && grappleTarget.Target;

					if(grappleLineRenderers[i].enabled) {
						grappleLineRenderers[i].SetPosition(0, transform.position);
						grappleLineRenderers[i].SetPosition(1, grappleTarget.WorldAttachPoint);
					}
				}
			}
		}

		public void GrappleDestroyRope(AbilityBase ability) {
			Grapple grapple = ability as Grapple;

			if(grapple)
				for(int i = 0; i < grappleLineRenderers.Count && i < grapple.MaxGrappleTargets; i++)
					grappleLineRenderers[i].enabled = false;
		}

		public void ExplosionEffect(AbilityBase ability) {
			if(explosionEffect)
				Instantiate(explosionEffect, ability.transform.position, ability.transform.rotation);
		}

		struct BoostParticlesInfo {
			public Vector3 Direction { get; set; }
			public ParticleSystem ParticleSystem { get; set; }

			public BoostParticlesInfo(Vector3 direction, ParticleSystem particleSystem) {
				Direction = direction;
				ParticleSystem = particleSystem;
			}
		}
	}
}