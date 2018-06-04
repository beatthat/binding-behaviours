using UnityEngine;
using BeatThat.App;
using BeatThat;
using UnityEngine.Events;
using System;
using BeatThat.Service;

namespace BeatThat
{
	public class BindingStateBehaviour : StateMachineBehaviour
	{
		public bool m_breakOnEnter;
		public bool m_breakOnExit;
		public bool m_disableUnbindOnGameObjectDisabled;

		protected Animator animator { get; private set; }	
		protected Transform transform { get { return this.gameObject != null? this.gameObject.transform: null; } }
		protected GameObject gameObject { get; private set; }

		private SafeRef<GameObjectEvents> m_gameObjectEvents;

		private void Bind(Animator animator)
		{
			DependencyInjection.InjectDependencies (this);

			this.animator = animator;
			this.gameObject = animator.gameObject;

			var goEvts = m_gameObjectEvents.value;
			if (goEvts == null) {
				goEvts = this.gameObject.AddIfMissing<GameObjectEvents> ();
				goEvts.onDisable += this.onGameObjectDisabled;
				goEvts.onDestroy += this.onGameObjectDestroyed;
				m_gameObjectEvents = new SafeRef<GameObjectEvents> ();
			}

			BindInternal();
			BindState();

			this.isBound = true;
		}

		private void OnGameObjectDestroyed(GameObject go)
		{
			Unbind ();
			ControllerDidDisable ();
		}
		private System.Action<GameObject> onGameObjectDestroyed { get { return m_onGameObjectDestroyed ?? (m_onGameObjectDestroyed = this.OnGameObjectDestroyed); } }
		private System.Action<GameObject> m_onGameObjectDestroyed;

		private void OnGameObjectDisabled(GameObject go)
		{
			if (!m_disableUnbindOnGameObjectDisabled) {
				Unbind ();
				ControllerDidDisable ();
			}
		}
		private System.Action<GameObject> onGameObjectDisabled { get { return m_onGameObjectDisabled ?? (m_onGameObjectDisabled = this.OnGameObjectDisabled); } }
		private System.Action<GameObject> m_onGameObjectDisabled;

		internal virtual void BindInternal() {}

		virtual protected void BindState() {}

		private void Unbind()
		{
			if(!this.isBound) {
				return;
			}

			this.isBound = false;

			var goEvts = m_gameObjectEvents.value;
			m_gameObjectEvents = default(SafeRef<GameObjectEvents>);
			if (goEvts != null) {
				goEvts.onDestroy -= this.onGameObjectDestroyed;
				goEvts.onDisable -= this.onGameObjectDisabled;
			}

			var b = GetBindings(false);
			if(b != null) {
				b.UnbindAll();
			}

			DisposeAttachedDisposables();
			CleanupAttachedBindings();

			UnbindState();

			this.animator = null;
			this.gameObject = null;
		}

		virtual protected void UnbindState() {}

		protected bool isBound { get; private set; }

		sealed override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.activeStateInfo = stateInfo;
			this.activeStateLayer = layerIndex;
			this.hasActiveStateInfo = true;

			if(!WillEnter(animator, stateInfo, layerIndex)) {
				return;
			}


			Bind(animator);
			DidEnter();

			#if UNITY_EDITOR
			if(m_breakOnEnter) {
				Debug.LogWarning("[" + Time.frameCount + "][" + Path() + "] " + GetType() + " break on enter");
				Debug.Break();
			}
			#endif
		}

		protected AnimatorStateInfo activeStateInfo { get; private set; }
		protected int activeStateLayer { get; private set; }
		protected bool hasActiveStateInfo { get; private set; }

		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.activeStateInfo = stateInfo;
			this.activeStateLayer = layerIndex;
			UpdateState();
		}

		/// <summary>
		/// The equivalent of StateMachineBehaviour::OnStateUpdate but minus params which should generally be avoided here.
		/// </summary>
		virtual protected void UpdateState() {}

		/// <summary>
		/// Called by OnStateEnter before Bind or DidEnter. 
		/// Provides a chance to cancel entry into the state. 
		/// If will enter decides to cancel, then it should return false.
		/// </summary>
		/// <returns><c>TRUE</c>, if the state should proceed. False if it should cancel entry.</returns>
		virtual protected bool WillEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { return true; }
		virtual protected void DidEnter() {}

		virtual protected void WillExit() {}
		virtual protected void DidExit() {}

		/// <summary>
		/// Called when the controller is disabled or destroyed. 
		/// Unbind will have already been called.
		/// </summary>
		virtual protected void ControllerDidDisable() {}

		sealed override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.activeStateInfo = stateInfo;
			this.activeStateLayer = layerIndex;
			try {
				WillExit();
			}
			catch(Exception e) {
				Debug.LogError ("[" + Time.frameCount + "][" + this.Path () 
					+ " caught an exception because in WillExit. Must proceed and try to Unbind or app can enter deeply unstable state (it might still anyway):\n"+ e.StackTrace);
			}

			Unbind();

			try {
				DidExit();
			}
			catch(Exception e) {
				Debug.LogError ("[" + Time.frameCount + "][" + this.Path () 
					+ " caught an exception in DidExit:\n"+ e.StackTrace);
			}

			this.hasActiveStateInfo = false;

			#if UNITY_EDITOR
			if(m_breakOnExit) {
				Debug.LogWarning("[" + Time.frameCount + "][" + animator.Path() + "] " + GetType() + " BREAK ON EXIT");
				Debug.Break();
			}
			#endif
		}

		/// <summary>
		/// For animator states that have motion. Sets the normalized time on the motion
		/// </summary>
		/// <param name="time">Time.</param>
		protected void SetNormalizedTime(float time)
		{
			if(this.animator == null) {
				#if BT_DEBUG_UNSTRIP
				Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + " SetNormalizedTime called while behaviour has no animator");
				#endif
				return;
			}

			if(!this.hasActiveStateInfo) {
				#if BT_DEBUG_UNSTRIP
				Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + " SetNormalizedTime called while behaviour has no active state info");
				#endif
				return;
			}

			this.animator.Play(this.activeStateInfo.shortNameHash, this.activeStateLayer, time);
		}

		/// <summary>
		/// We need to Unbind and cleanup when the underlying controller/animator is destroyed. 
		/// The only way to detect this is the OnDisable message;
		/// OnDestroy would be called only if the actual AnimatorController asset were destroyed.
		/// </summary>
		private void OnDisable()
		{
			Unbind();
			ControllerDidDisable();
		}

		/// <summary>
		/// Register a Notification that will be automatically be unregistered when this presenter is unbound
		/// </summary>
		protected NotificationBinding Bind(string type, Action callback)
		{
			var b = NotificationBus.Add(type, callback, this);
			GetBindings(true).Add(b);
			return b;
		}

		/// <summary>
		/// Register a Notification that will be automatically be unregistered when this presenter is unbound
		/// </summary>
		protected NotificationBinding Bind<T>(string type, Action<T> callback)
		{
			var b = NotificationBus.Add<T>(type, callback, this);
			GetBindings(true).Add(b);
			return b;
		}

		protected NotificationBindings GetBindings(bool create)
		{
			if(m_bindings == null && create) {
				m_bindings = new NotificationBindings();
			}
			return m_bindings;
		}

		protected UnityEventBinding Bind(UnityEvent e, UnityAction a)
		{
			var b = StaticObjectPool<UnityEventBinding>.Get();
			b.Bind(e, a);
			Attach(b);
			return b;
		}

		protected UnityEventBinding<T> Bind<T>(UnityEvent<T> e, UnityAction<T> a)
		{
			var b = StaticObjectPool<UnityEventBinding<T>>.Get();
			b.Bind(e, a);
			Attach(b);
			return b;
		}

		/// <summary>
		/// Attached IDisposables will be automatically disposed when the state exits or unbinds.
		/// Short-syntax version of DisposeOnUnbind
		/// </summary>
		protected T Attach<T>(T r) where T : IDisposable
		{
			return DisposeOnUnbind<T>(r);
		}

		/// <summary>
		/// Attached IDisposables will be automatically disposed when the state exits or unbinds
		/// </summary>
		protected T DisposeOnUnbind<T>(T r) where T : IDisposable
		{
			if(m_attachedDisposables == null) {
				m_attachedDisposables = ListPool<IDisposable>.Get();
			}

			m_attachedDisposables.Add(r);

			return r;
		}

		/// <summary>
		/// Attached will be automatically unbound when the state exits or unbinds.
		/// Short syntax version of UnbindOnUnbind.
		/// </summary>
		protected void Attach(HasBinding b)
		{
			Attach(b.binding);
		}

		/// <summary>
		/// Attached will be automatically unbound when the state exits or unbinds.
		/// </summary>
		protected void UnbindOnUnbind(HasBinding b)
		{
			Attach(b.binding);
		}
			
		/// <summary>
		/// Attached will be automatically unbound when the state exits or unbinds.
		/// Short syntax version of UnbindOnUnbind.
		/// </summary>
		protected void Attach(Binding b)
		{
			if(m_attachedBindings == null) {
				m_attachedBindings = ListPool<Binding>.Get();
			}

			m_attachedBindings.Add(b);
		}

		/// <summary>
		/// Attached will be automatically unbound when the state exits or unbinds.
		/// </summary>
		protected void UnbindOnUnbind(Binding b)
		{
			if(m_attachedBindings == null) {
				m_attachedBindings = ListPool<Binding>.Get();
			}

			m_attachedBindings.Add(b);
		}

		protected void CleanupAttachedBindings()
		{
			if(m_attachedBindings == null) {
				return;
			}

			for(int i = m_attachedBindings.Count - 1; i >= 0; i--) {

				Binding b = m_attachedBindings[i];
				m_attachedBindings.RemoveAt(i);

				if(b == null) {
					continue;
				}

				b.Unbind();

				var d = b as IDisposable;
				if(d == null) {
					continue;
				}

				d.Dispose();
			}

			ListPool<Binding>.Return(m_attachedBindings);

			m_attachedBindings = null;
		}

		/// <summary>
		/// Unbind a single Disposable that was previously bound.
		/// </summary>
		protected bool Detach(IDisposable r)
		{
			return m_attachedDisposables != null && m_attachedDisposables.Remove (r);

		}

		protected T GetComponent<T>() where T : class
		{
			return this.animator.GetComponent<T>();
		}

		protected T RequireChild<T>(ref T c) where T : class
		{
			return c ?? RequireChild (ref c);
		}

		protected T RequireChild<T>() where T : class
		{
			var c = this.animator.GetComponentInChildren<T>(true);

			if(c == null) {
				Debug.LogError("[" + Time.frameCount + "] " + GetType() + " [" + this.animator.Path() + "] is missing required child component of type " + typeof(T).Name);
			}

			return c;
		}

		protected string Path()
		{
			var a = this.transform;
			return a == null ? "[unbound]" : a.Path();

		}

		private void DisposeAttachedDisposables()
		{
			if(m_attachedDisposables == null) {
				return;
			}

			foreach(var r in m_attachedDisposables) {
				if(r == null) {
					Debug.LogError("[" + Time.frameCount + "] " + GetType() + "::DisposeAttachedDisposables found a null disposable");
					continue;
				}

				r.Dispose();
			}

			m_attachedDisposables.Clear();

			ListPool<IDisposable>.Return(m_attachedDisposables);
			m_attachedDisposables = null;
		}

		private NotificationBindings m_bindings;
		private ListPoolList<Binding> m_attachedBindings;
		private ListPoolList<IDisposable> m_attachedDisposables;
	}

	public class BindingStateBehaviour<T> : BindingStateBehaviour where T : class
	{
		protected T controller { get; private set; }

		sealed internal override void BindInternal() 
		{
			this.controller = this.animator.GetComponent<T>();

			if(this.controller == null) {
				Debug.LogWarning("[" + Time.frameCount + "][" + Path() + "] " + GetType()
					+ " failed to find controller with type " + typeof(T).Name);
			}
		}
	}
}
