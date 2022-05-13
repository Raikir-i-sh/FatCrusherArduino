	using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParallaxLayer : MonoBehaviour
{
	public float parallaxFactor;
	
	void Start()
	{
		
		if(parallaxFactor < 0){
		// if layer is closer to camera than the player.
		}
	}
	

	public void Move(float deltaX, float deltaY)
	{
		Vector3 newPos = transform.localPosition;
		newPos.x -= deltaX * parallaxFactor;
		newPos.y -= deltaY * parallaxFactor;
		transform.localPosition = newPos;
		
		
	}
}