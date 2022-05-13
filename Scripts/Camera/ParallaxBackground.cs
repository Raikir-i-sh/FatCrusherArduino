using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*How it works: First you have to have a gameobject with the ParallaxBackground script 
and add the layers as children to this game object and put the ParallaxLayer script on these. 
Then all you have to do is call Move(delta) where delta is the distance that the ortho camera moved.
 I made an event in the parallax camera, but you can do as you will. 
 The layers will move with parallaxFactor ( if parallaxFactor is 1 then the layer 
 will move at the same speed as the camera, if it's 0.5 then it will move 2 times slower, 2 for 2 times faster)
I prefer the second method since it's more 2d ish, but I have used both methods and both work well. 
*/
public class ParallaxBackground : MonoBehaviour
{
	public ParallaxCamera parallaxCamera;
	List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

	void Start()
	{
		if (parallaxCamera == null)
			parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();
		if (parallaxCamera != null)
			parallaxCamera.onCameraTranslate += Move;
		SetLayers();
	}

	void SetLayers()
	{
		parallaxLayers.Clear();
		for (int i = 0; i < transform.childCount; i++)
		{
			ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

			if (layer != null)
			{
				layer.name = "Layer-" + i;
				parallaxLayers.Add(layer);
			}
		}
	}
	void Move(float deltaX, float deltaY)
	{
		foreach (ParallaxLayer layer in parallaxLayers)
		{
			layer.Move(deltaX, deltaY);
		}
	}
}