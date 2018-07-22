using System;
using System.Collections.Generic;
using BeatThat.CollectionsExt;
using BeatThat.Notifications;
using BeatThat.Pools;
using BeatThat.Properties;
using BeatThat.SafeRefs;
using BeatThat.DependencyInjection;
using BeatThat.TransformPathExt;
using UnityEngine;
using UnityEngine.Events;

namespace BeatThat.Bindings
{
    /// <summary>
    /// Base class for a <c>Component</c> that will bind one or more listeners.
    /// Provides a guarantee all things bound will be unbound when this component unbinds, 
    /// and automatically unbinds if this component's GameObject is destroyed.
    /// </summary>
    public abstract class BindingBehaviour : MonoBehaviour, HasBinding, DependencyInjectionEventHandler
	{
		
		/// <summary>
		/// Base implementation sets isBound property.
		/// overriding implementations should call base.Bind() as last step
		/// </summary>
		public void Bind()
		{
            if(this.isBound) {
                return;
            }

            if(!InjectDependencies.On (this)) {
                return;
            }

            BindWithDependencies();
			
		}

        private void BindWithDependencies()
        {
            if(this.isBound) {
                return;
            }

            BindAll();
            this.safeBinding.BindTo(this);
            this.isBound = true;
        }


        virtual public void OnDependencyInjectionWaitingForServicesReady() {}

        virtual public void OnWillInjectDependencies() {}
        virtual public void OnDidInjectDependencies()
        {
            if(!this.isBound) {
                BindWithDependencies();
            }
        }

        public void Rebind()
        {
            Unbind();
            Bind();
        }
		
		/// <summary>
		/// Base implementation sets isBound property false.
		/// overriding implementations should call base.Bind() as last step
		/// </summary>
	 	public void Unbind()
		{
			if(this.isBound) {
				UnbindAll();
				this.isBound = false;
				this.safeBinding.Invalidate();
				UnbindAllAttached();
				UnregisterAllNotifications();
			}
		}
		
		/// <summary>
		/// Put your controller's custom Bind code here.
		/// </summary>
		virtual protected void BindAll() {}
		
		/// <summary>
		/// Put your controller's custom Unbind code here.
		/// </summary>
		virtual protected void UnbindAll() {}

		/// <summary>
		/// Register a Notification that will be automatically be unregistered when this presenter is unbound
		/// </summary>
		protected NotificationBinding Bind(string type, Action callback)
		{
			var b = NotificationBus.Add(type, callback, this);
			GetNotifications(true).Add(b);
			return b;
		}

		/// <summary>
		/// Register a Notification that will be automatically be unregistered when this presenter is unbound
		/// </summary>
		protected NotificationBinding Bind<T>(string type, Action<T> callback)
		{
			var b = NotificationBus.Add<T>(type, callback, this);
			GetNotifications(true).Add(b);
			return b;
		}
			
		protected void Attach(Binding b)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(b);
		}

		/// <summary>
		/// Attach HasBindings (usually sub presenters) so that when this presenter Unbinds, all attached HasBindings unbind automatically.
		/// </summary>
		/// <param name="h1">H1.</param>
		protected void Attach(HasBinding h1)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding);
		}

		protected void Attach(HasBinding h1, HasBinding h2)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding);
			bindings.Add(h2.binding);
		}

		protected void Attach(HasBinding h1, HasBinding h2, HasBinding h3)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding); bindings.Add(h2.binding); bindings.Add(h3.binding);
		}

		protected void Attach(HasBinding h1, HasBinding h2, HasBinding h3, HasBinding h4)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding); bindings.Add(h2.binding); bindings.Add(h3.binding); bindings.Add(h4.binding);
		}

		protected void Attach(HasBinding h1, HasBinding h2, HasBinding h3, HasBinding h4, HasBinding h5)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding); bindings.Add(h2.binding); bindings.Add(h3.binding); bindings.Add(h4.binding); bindings.Add(h5.binding);
		}

		protected void Attach(HasBinding h1, HasBinding h2, HasBinding h3, HasBinding h4, HasBinding h5, HasBinding h6)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding); bindings.Add(h2.binding); bindings.Add(h3.binding); bindings.Add(h4.binding); bindings.Add(h5.binding);
			bindings.Add(h6.binding);
		}

		protected void Attach(HasBinding h1, HasBinding h2, HasBinding h3, HasBinding h4, HasBinding h5, HasBinding h6, HasBinding h7)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding); bindings.Add(h2.binding); bindings.Add(h3.binding); bindings.Add(h4.binding); bindings.Add(h5.binding); 
			bindings.Add(h6.binding); bindings.Add(h7.binding);
		}

		protected void Attach(HasBinding h1, HasBinding h2, HasBinding h3, HasBinding h4, HasBinding h5, HasBinding h6, HasBinding h7, HasBinding h8)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding); bindings.Add(h2.binding); bindings.Add(h3.binding); bindings.Add(h4.binding); bindings.Add(h5.binding); 
			bindings.Add(h6.binding); bindings.Add(h7.binding); bindings.Add(h8.binding);
		}

		protected void Attach(HasBinding h1, HasBinding h2, HasBinding h3, HasBinding h4, HasBinding h5, HasBinding h6, HasBinding h7, HasBinding h8, HasBinding h9)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding); bindings.Add(h2.binding); bindings.Add(h3.binding); bindings.Add(h4.binding); bindings.Add(h5.binding); 
			bindings.Add(h6.binding); bindings.Add(h7.binding); bindings.Add(h8.binding); bindings.Add(h9.binding);
		}

		protected void Attach(HasBinding h1, HasBinding h2, HasBinding h3, HasBinding h4, HasBinding h5, HasBinding h6, HasBinding h7, HasBinding h8, HasBinding h9, HasBinding h10)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding); bindings.Add(h2.binding); bindings.Add(h3.binding); bindings.Add(h4.binding); bindings.Add(h5.binding); 
			bindings.Add(h6.binding); bindings.Add(h7.binding); bindings.Add(h8.binding); bindings.Add(h9.binding); bindings.Add(h10.binding);
		}

		protected void Attach(HasBinding h1, HasBinding h2, HasBinding h3, HasBinding h4, HasBinding h5, HasBinding h6, HasBinding h7, HasBinding h8, HasBinding h9, HasBinding h10,
			HasBinding h11)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding); bindings.Add(h2.binding); bindings.Add(h3.binding); bindings.Add(h4.binding); bindings.Add(h5.binding); 
			bindings.Add(h6.binding); bindings.Add(h7.binding); bindings.Add(h8.binding); bindings.Add(h9.binding); bindings.Add(h10.binding);
			bindings.Add(h11.binding); 
		}

		protected void Attach(HasBinding h1, HasBinding h2, HasBinding h3, HasBinding h4, HasBinding h5, HasBinding h6, HasBinding h7, HasBinding h8, HasBinding h9, HasBinding h10,
			HasBinding h11, HasBinding h12)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding); bindings.Add(h2.binding); bindings.Add(h3.binding); bindings.Add(h4.binding); bindings.Add(h5.binding); 
			bindings.Add(h6.binding); bindings.Add(h7.binding); bindings.Add(h8.binding); bindings.Add(h9.binding); bindings.Add(h10.binding);
			bindings.Add(h11.binding); bindings.Add(h12.binding); 
		}

		protected void Attach(HasBinding h1, HasBinding h2, HasBinding h3, HasBinding h4, HasBinding h5, HasBinding h6, HasBinding h7, HasBinding h8, HasBinding h9, HasBinding h10,
			HasBinding h11, HasBinding h12, HasBinding h13)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding); bindings.Add(h2.binding); bindings.Add(h3.binding); bindings.Add(h4.binding); bindings.Add(h5.binding); 
			bindings.Add(h6.binding); bindings.Add(h7.binding); bindings.Add(h8.binding); bindings.Add(h9.binding); bindings.Add(h10.binding);
			bindings.Add(h11.binding); bindings.Add(h12.binding); bindings.Add(h13.binding); 
		}

		protected void Attach(HasBinding h1, HasBinding h2, HasBinding h3, HasBinding h4, HasBinding h5, HasBinding h6, HasBinding h7, HasBinding h8, HasBinding h9, HasBinding h10,
			HasBinding h11, HasBinding h12, HasBinding h13, HasBinding h14)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding); bindings.Add(h2.binding); bindings.Add(h3.binding); bindings.Add(h4.binding); bindings.Add(h5.binding); 
			bindings.Add(h6.binding); bindings.Add(h7.binding); bindings.Add(h8.binding); bindings.Add(h9.binding); bindings.Add(h10.binding);
			bindings.Add(h11.binding); bindings.Add(h12.binding); bindings.Add(h13.binding); bindings.Add(h14.binding); 
		}

		protected void Attach(HasBinding h1, HasBinding h2, HasBinding h3, HasBinding h4, HasBinding h5, HasBinding h6, HasBinding h7, HasBinding h8, HasBinding h9, HasBinding h10,
			HasBinding h11, HasBinding h12, HasBinding h13, HasBinding h14, HasBinding h15)
		{
			var bindings = GetAttachedBindings(true);
			bindings.Add(h1.binding); bindings.Add(h2.binding); bindings.Add(h3.binding); bindings.Add(h4.binding); bindings.Add(h5.binding); 
			bindings.Add(h6.binding); bindings.Add(h7.binding); bindings.Add(h8.binding); bindings.Add(h9.binding); bindings.Add(h10.binding);
			bindings.Add(h11.binding); bindings.Add(h12.binding); bindings.Add(h13.binding); bindings.Add(h14.binding); bindings.Add(h15.binding);
		}

		/// <summary>
		/// Unbinds all dependent bindings added through AddDependentBinding
		/// </summary>
		protected void UnbindAllAttached()
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
		/// Unregisters any notifications that were registered via calls to AddNotification or AddNotification<T>
		/// </summary>
		protected void UnregisterAllNotifications()
		{
			if(m_notificationBindings != null) {
				m_notificationBindings.UnbindAll();
			}
		}

		public void GetAttachedBindings(ICollection<Binding> result)
		{
			if(m_attachedBindings != null) {
				result.AddRange(m_attachedBindings);
			}
		}

		protected ListPoolList<Binding> GetAttachedBindings(bool create)
		{
			if(m_attachedBindings == null && create) {
				m_attachedBindings = ListPool<Binding>.Get();
			}
			return m_attachedBindings;
		}
		
		public Binding binding
		{
			get {
				return this.safeBinding;
			}
		}
		
		private MyBinding safeBinding
		{
			get {
				if(m_binding == null) {
					m_binding = new MyBinding();
				}
				return m_binding;
			}
		}

		protected NotificationBindings GetNotifications(bool create)
		{
			if(m_notificationBindings == null && create) {
				m_notificationBindings = new NotificationBindings();
			}
			return m_notificationBindings;
		}

		/// <summary>
		/// True if Bind has been called without a subsequent Unbind.
		/// Implies events listeners may remain attached, etc.
		/// </summary>
		public bool isBound
		{
			get; protected set;
		}

		public bool isDestroyed { get; protected set; }
		
		virtual protected void OnDestroy()
		{
			this.isDestroyed = true;
			Unbind();
		}


		public void DisposeOnUnbind(IDisposable d)
		{
			var b = StaticObjectPool<DisposableBinding>.Get();
			b.Bind(d);
			Attach(b);
		}

		public void Bind<T>(IHasValue<T> hasVal, T val) where T : class
		{
			var b = StaticObjectPool<ValueHolderBinding<T>>.Get();
			b.Bind(hasVal, val);
			Attach(b);
		}

		protected void Bind(UnityEvent e, UnityAction a)
		{
			var b = StaticObjectPool<UnityEventBinding>.Get();
			b.Bind(e, a);
			Attach(b);
		}

		protected void Bind<T>(UnityEvent<T> e, UnityAction<T> a)
		{
			var b = StaticObjectPool<UnityEventBinding<T>>.Get();
			b.Bind(e, a);
			Attach(b);
		}

        class MyBinding : Binding
		{
			public void Unbind()
			{
				var owner = m_owner.value;
				if(owner != null) {
					owner.Unbind();
					m_owner.value = null;
				}
			}

			public void BindTo(BindingBehaviour p)
			{
				m_owner = new SafeRef<BindingBehaviour>(p);						
			}

			public void Invalidate()
			{
				m_owner.value = null;
			}

			public bool isBound 
			{
				get {
					var owner = m_owner.value;
					return owner != null && owner.isBound;
				}
			}

			override public string ToString()
			{
				var owner = m_owner.value;
				return "[Binding isBound=" + this.isBound + ", obj=" + (owner != null? owner.Path(): "null").ToString() + "]";
			}

			private SafeRef<BindingBehaviour> m_owner;
		}


		private ListPoolList<Binding> m_attachedBindings;
		private NotificationBindings m_notificationBindings;
		private MyBinding m_binding;
	}



}






