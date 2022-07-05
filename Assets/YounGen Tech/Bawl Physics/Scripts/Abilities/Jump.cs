using UnityEngine;
using System.Collections;

namespace YounGenTech.BawlPhysics {
	/// <summary>
	/// Jump not once, not twice, but pretty much infinite times (jump count is customizable).
	/// </summary>
	[AddComponentMenu("YounGen Tech/Bawl Physics/Abilities/Jump"), DisallowMultipleComponent]
	public class Jump : AbilityBase {

		[Tooltip("Delays the use of this ability"), SerializeField]
		ActionDelay _actionDelay = new ActionDelay(.5f);

		[Tooltip("Stores the current surface normal if colliding. If not colliding a timer will count down and the surface normal will be set to a default normal upon reaching 0."), SerializeField]
		SurfaceStickDelay _stickyNormals = new SurfaceStickDelay(Vector3.zero, Vector3.up, .2f);

		[Tooltip("The velocity of the jump"), SerializeField]
		public float _jumpSpeed = 2;

		/// <summary>
		/// Not colliding when performing a jump will count as a jump without effecting the ball's velocity.
		/// </summary>
		[SerializeField, Tooltip("Not colliding when performing a jump will use up a jump without effecting the ball's velocity")]
		bool _countNoCollisionAsJump = true;

		[SerializeField, Tooltip("Maximum amount of jumps you can do")]
		int _maximumJumps = 1;

		[SerializeField, Tooltip("How many jumps have happened")]
		int _totalJumps;

		bool checkForJumpReset;
		bool usedLastFrame;

		#region Properties
		/// <summary>
		/// Delays the use of this ability.
		/// </summary>
		public ActionDelay ActionDelay {
			get { return _actionDelay; }
			set { _actionDelay = value; }
		}

		/// <summary>
		/// Not colliding when performing a jump will use up a jump without effecting the ball's velocity.
		/// </summary>
		public bool CountNoCollisionAsJump {
			get { return _countNoCollisionAsJump; }
			set { _countNoCollisionAsJump = value; }
		}

		/// <summary>
		/// The velocity of the jump.
		/// </summary>
		public float JumpSpeed {
			get { return _jumpSpeed; }
			set { _jumpSpeed = value; }
		}

		/// <summary>
		/// Maximum amount of jumps you can do.
		/// </summary>
		public int MaximumJumps {
			get { return _maximumJumps; }
			set { _maximumJumps = value; }
		}

		/// <summary>
		/// Stores the current surface normal if colliding. If not colliding a timer will count down and the surface normal will be set to a default normal upon reaching 0.
		/// </summary>
		public SurfaceStickDelay StickyNormals {
			get { return _stickyNormals; }
			set { _stickyNormals = value; }
		}

		/// <summary>
		/// How many jumps have happened.
		/// </summary>
		public int TotalJumps {
			get { return _totalJumps; }
			set { _totalJumps = value; }
		}
		#endregion
		
		void Start() {
			CollisionDetection.onCollided.AddListener(CollisionEvent);
		}

		protected override void Update() {
			base.Update();

			if(Rigidbody.IsSleeping()) Rigidbody.WakeUp();

			if(!IsUsing)
				ActionDelay.Update();

			if(!CollisionDetection.IsColliding && (TotalJumps == 0 || TotalJumps == MaximumJumps) && !StickyNormals.IsReady)
				StickyNormals.Update();
		}

		void CollisionEvent(Collision hit) {
			StickyNormals.Delay();

			if(!IsUsing && !usedLastFrame) 
				ResetJumps();
		}

		IEnumerator UsedLastFrame() {
			yield return null;
			usedLastFrame = false;
		}

		/// <summary>
		/// Starts and finishes the ability automatically.
		/// </summary>
		public override void StartAbility() {
			if(ActionDelay.IsReady) {
				IsUsing = true;

				PrepareJump();
				FinishAbility();
			}
		}

		public override void FinishAbility() {
			if(IsUsing) {
				DoJump();
				usedLastFrame = true;

				StopCoroutine("UsedLastFrame");
				StartCoroutine("UsedLastFrame");

                IsUsing = false;
			}
		}

		public void SetSurfaceNormals(Vector3 normals) {
			if(CollisionDetection.IsColliding && TotalJumps == 0) 
				StickyNormals.Value = normals;
		}

		void PrepareJump() {
			if(!CollisionDetection.IsColliding && TotalJumps == 0 && StickyNormals.IsReady) {
				if(CountNoCollisionAsJump)
					TotalJumps = 1;

				StickyNormals.Reset();
			}
		}

		void DoJump() {
			if(CollisionDetection.IsColliding || TotalJumps < MaximumJumps) {
				if(StickyNormals.Value != Vector3.zero && ActionDelay.IsReady) {
					Vector3 jumpDirection = StickyNormals.Value.normalized;
					Vector3 currentSpeedInNormals = Vector3.Project(Rigidbody.velocity, jumpDirection);

					Rigidbody.velocity = (Rigidbody.velocity - currentSpeedInNormals) + (jumpDirection * JumpSpeed);

					TotalJumps++;

					StickyNormals.Reset();
					ActionDelay.Delay();
				}
			}
		}

		/// <summary>
		/// Lowers the totalJumps by one so you can jump again
		/// </summary>
		public void ExtendJump() {
			TotalJumps = Mathf.Max(TotalJumps - 1, 0);
		}

		/// <summary>
		/// Sets totalJumps to 0
		/// </summary>
		public void ResetJumps() {
			TotalJumps = 0;
		}
	}
}