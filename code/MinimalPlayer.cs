﻿using Sandbox;
using System;
using System.Linq;

namespace MinimalExample
{
	partial class MinimalPlayer : Player
	{
		[Net, Local]
		public double food { get; set; }= 50.0;
		//double food = 50.0;
		public bool IsAlive = true;
		public override void Respawn()
		{
			IsAlive = true;
			
			SetModel( "models/citizen/citizen.vmdl" );

			//
			// Use WalkController for movement (you can make your own PlayerController for 100% control)
			//
			Controller = new WalkController();

			//
			// Use StandardPlayerAnimator  (you can make your own PlayerAnimator for 100% control)
			//
			Animator = new StandardPlayerAnimator();

			//
			// Use ThirdPersonCamera (you can make your own Camera for 100% control)
			//
			Camera = new FirstPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			base.Respawn();
		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			//
			// If you have active children (like a weapon etc) you should call this to 
			// simulate those too.
			//
			SimulateActiveChild( cl, ActiveChild );

			//
			// If we're running serverside and Attack1 was just pressed, spawn a ragdoll
			//
			if (IsAlive == true) 
			{
				if ( IsServer && Input.Pressed( InputButton.Attack1 ) )
				{
					var ragdoll = new ModelEntity();
					ragdoll.SetModel( "models/glizzy.vmdl" );  
					ragdoll.Position = EyePos + EyeRot.Forward * 40;
					ragdoll.Rotation = Rotation.LookAt( Vector3.Random.Normal );
					ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
					ragdoll.PhysicsGroup.Velocity = EyeRot.Forward * 5000;
					PlaySound( "fard" );
				}
				else if (Input.Pressed( InputButton.Attack1 ) && IsAlive == true){
					PlaySound( "fard" );
				}
			}
			

			if ( food <= 0 || food >= 100 )
			{
				base.OnKilled();

				EnableDrawing = false;

				food = 50;

				IsAlive = false;

				PlaySound( "death" );
			}

			food = food - 0.05;
		}


		public override void OnKilled()
		{
			base.OnKilled();
			//BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone( lastDamage.HitboxIndex ) );
			EnableDrawing = false;
			
			IsAlive = false;
			
			food = 50;
		}

		public override void StartTouch( Entity other )
		{
			//this is what happens when hit by a glizzy
			food = food + 5.0;
			//base.Respawn();
			other.Delete();
			PlaySound( "amongus" );
		}

	}
}
