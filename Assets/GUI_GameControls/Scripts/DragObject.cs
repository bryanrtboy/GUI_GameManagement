//Author: Bryan Leister
//Date: April 2014
//Description:
//
//This can be used as a stand-in for the Player, when you click your mouse on screen, the sphere is dragged around.
//Useful for designing Kinect or Leap interaces when you don't have access to the Kinect or Leap.
//
//Instructions: We assume the colliders are static colliders without a rigidbody, so we require that this object has
//a rigidbody and we set it to isKinematic by default. Detecting collisions requires that one of the objects
//has a rigid body attached.
//
//Attach this script to something with a collider and we will then assign it the tag of Player if it is not already. The
//triggering object should test for colliders tagged 'Player'.
//
//For simplicity sake, we do not move the Z value of this object, it needs to be in line with what you want to trigger.
//

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class DragObject : MonoBehaviour
{

		public string 		m_defaultTag = "Player";				//Usually triggering objects check what tag triggered it, for a Player stand-in use 'Player'
		
		private Transform 	m_transform;
		private float 		m_zOffset = 0;
		private Camera 		m_mainCamera;

		void Start ()
		{

				m_mainCamera = FindCamera ();
				m_transform = this.transform;
				if (m_transform.tag != "Player") {
						Debug.Log ("The " + this.name + " was not tagged " + m_defaultTag + ", we think it needs this if we are using it to Trigger things. Correcting it now.");
						m_transform.tag = m_defaultTag;
				}

				this.rigidbody.isKinematic = true;				//Setting the rigidbody to isKinematic since we don't need physics.
				this.rigidbody.useGravity = false;

				m_zOffset = m_mainCamera.transform.position.z - m_transform.position.z;

		}



		void Update ()
		{
				// Make sure the user pressed the mouse down
				if (!Input.GetMouseButton (0))
						return;

				m_transform.position = m_mainCamera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, -m_zOffset));
		}

		Camera FindCamera ()
		{
				if (camera)
						return camera;
				else
						return Camera.main;
		}
}
