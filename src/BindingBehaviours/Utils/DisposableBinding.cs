using System;

namespace BeatThat
{
	/// <summary>
	/// Binds a (reference) value to a IHasValue<T> so that on Unbind, can clear the value from the value holder.
	/// This is useful for say, a controller whose view has a HasTexture property:
	/// when the controller unbinds, you want that HasTexture to clear it's reference to the Texture.
	/// Using this binding type, you can have that behaviour happen automatically when the controller unbinds, e.g.
	/// 
	/// <code>this.controller.Bind<Texture>(this.view.someHasTexture, this.model.someTexture); // this will auto unbind when the controller unbinds</code>
	/// </summary>
	public class DisposableBinding : Binding, IDisposable, Poolable
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
			StaticObjectPool<DisposableBinding>.Return(this);
		}
		#endregion

		public void Bind(IDisposable d)
		{
			this.disposable = d;
			this.isBound = true;
		}

		public bool isBound { get; private set; }

		public void Unbind()
		{
			if(!this.isBound) {
				return;
			}

			var d = this.disposable;
			if(d != null) {
				d.Dispose();
			}

			this.disposable = null;

			this.isBound = false;
		}

		public void Reset()
		{
			if(this.isBound) {
				Unbind();
			}
			this.disposable = null;
		}

		private IDisposable disposable { get { return m_disposable.value; } set { m_disposable = new SafeRef<IDisposable>(value); } }

		private SafeRef<IDisposable> m_disposable;

	}


}
