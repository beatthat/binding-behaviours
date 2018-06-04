using BeatThat.Service;

namespace BeatThat
{
	/// <summary>
	/// Use this base class for registered services that use bindings
	/// and, more importantly, dependency injection (@see BeatThat.Service).
	/// 
	/// The [Inject] tag (and dependency-injection/service lookup in general)
	///  won't work properly until all services have been registered.
	/// 
	/// This base class calls BindingBehaviour::Bind() only *after*
	/// all services have been init.
	/// 
	/// </summary>
	public abstract class BindingService : BindingBehaviour, AutoInitService
	{
		public void InitService(Services services)
		{
			Bind();
		}
	}



}
