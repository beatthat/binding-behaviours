using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatThat
{
	public class GameObjectEvents : MonoBehaviour
	{

		public event System.Action<GameObject> onDisable;
		public event System.Action<GameObject> onDestroy;

		void OnDisable()
		{
			if (this.onDisable != null) {
				this.onDisable (this.gameObject);
			}
		}

		void OnDestroy()
		{
			if (this.onDestroy != null) {
				this.onDestroy (this.gameObject);
			}
		}
	}

}
