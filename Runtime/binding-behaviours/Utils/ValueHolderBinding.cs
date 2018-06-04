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
	public class ValueHolderBinding<T> : Binding, IDisposable, Poolable
		where T : class
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
			StaticObjectPool<ValueHolderBinding<T>>.Return(this);
		}
		#endregion

		public void Bind(IHasValue<T> valHolder, T val)
		{
			this.valueHolder = valHolder;
			this.value = val;

			if(valHolder != null) {
				valHolder.value = val;
			}

			this.isBound = true;
		}

		public bool isBound { get; private set; }

		public void Unbind()
		{
			if(!this.isBound) {
				return;
			}

			var holder = this.valueHolder;
			var val = this.value;

			if(holder != null && object.ReferenceEquals(holder.value, val)) {
				holder.value = null;
			}

			this.valueHolder = null;
			this.value = null;

			this.isBound = false;
		}

		public void Reset()
		{
			if(this.isBound) {
				Unbind();
			}
			this.valueHolder = null;
			this.value = null;
		}

		private IHasValue<T> valueHolder { get { return m_valueHolder.value; } set { m_valueHolder = new SafeRef<IHasValue<T>>(value); } }
		private T value { get { return m_value.value; } set { m_value = new SafeRef<T>(value); } }

		private SafeRef<IHasValue<T>> m_valueHolder; 
		private SafeRef<T> m_value;

	}


}
