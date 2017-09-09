using UnityEngine.Events;
using System;

namespace BeatThat
{
	public class UnityEventBinding : Binding, IDisposable, Poolable
	{
//		public UnityEventBinding(UnityEvent evt, UnityAction callback)
//		{
//			this.unityEvent = evt;
//			this.callback = callback;
//		}

		#region Poolable implementation
		public void OnReturnedToPool ()
		{
			Reset();
		}
		#endregion

		#region IDisposable implementation
		public void Dispose ()
		{
			if(this.isBound) {
				Unbind();
			}
			StaticObjectPool<UnityEventBinding>.Return(this);
		}
		#endregion

		public void Bind(UnityEvent evt, UnityAction callback)
		{
			this.unityEvent = evt;
			this.callback = callback;
			this.unityEvent.AddListener(this.callback);
			this.isBound = true;
		}

		public bool isBound { get; private set; }

		public void Unbind()
		{
			if(!this.isBound) {
				return;
			}
			this.unityEvent.RemoveListener(this.callback);
			this.isBound = false;
		}

		public void Reset()
		{
			if(this.isBound) {
				Unbind();
			}
			this.unityEvent = null;
			this.callback = null;
		}

		public UnityEvent unityEvent { get; private set; }
		public UnityAction callback { get; private set; }

	}

	public class UnityEventBinding<T> : Binding, IDisposable, Poolable
	{
		#region Poolable implementation
		public void OnReturnedToPool ()
		{
			Reset();
		}
		#endregion

		#region IDisposable implementation
		public void Dispose ()
		{
			if(this.isBound) {
				Unbind();
			}
			StaticObjectPool<UnityEventBinding<T>>.Return(this);
		}
		#endregion

		public void Bind(UnityEvent<T> evt, UnityAction<T> callback)
		{
			this.unityEvent = evt;
			this.callback = callback;
			this.unityEvent.AddListener(this.callback);
			this.isBound = true;
		}

		public bool isBound { get; private set; }

		public void Unbind()
		{
			if(!this.isBound) {
				return;
			}
			this.unityEvent.RemoveListener(this.callback);
			this.isBound = false;
		}

		public void Reset()
		{
			if(this.isBound) {
				Unbind();
			}
			this.unityEvent = null;
			this.callback = null;
		}

		public UnityEvent<T> unityEvent { get; private set; }
		public UnityAction<T> callback { get; private set; }

	}

}
