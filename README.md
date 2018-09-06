<a name="readme"></a>When you have components that add listeners to other objects--global or otherwise--then it's important (and also tedious) to guarantee that ALL of those listener bindings get cleaned up with the listener component 'unbinds', say is deactivated or destroyed.

This package contains base classes for both MonoBehaviours and StateMachineBehaviour that provide methods to bind UnityEvents, Actions, Notifications, etc. and have those bindings auto unbind on a common call.


## Install

From your unity project folder:

    npm init --force # only if you don't yet have a package.json file
    npm install --save beatthat/binding-behaviours

The package and all its dependencies will be installed under Assets/Plugins/packages.

In case it helps, a quick video of the above: https://youtu.be/Uss_yOiLNw8

## Usage

```csharp
public class Foo : BindingBehaviour
{
    override protected void BindAll()
    {
        Bind("some-notification-type", () => {
            // this will unbind when Foo::Unbind is called
        });

        Bind<Bar>("some-notification-type-2", (bar) => {
            // you can also bind to a notification that wants to send a param
        });

        Bind<Bar2>((UnityEvent<Bar2>)this.bar2, (bar2) => {
            // also works with Unity events
        });

        Bind<Bar3>((Action<Bar3>)this.bar3, (bar3) => {
            // also works with csharp events
        });
    }

    void OnDestroy()
    {
        // all of the above bindings automatically unbind on destroy
        // or when Foo::Unbind is called explicitly
    }
}
```
