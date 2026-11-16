#if ANDROID

using Android.Widget;
using Microsoft.Maui.Handlers;

namespace MauiAndroidTest;

public class ARCoreMauiViewHandler : ViewHandler<ARCoreMauiView, FrameLayout>
{
    public static readonly IPropertyMapper<ARCoreMauiView, ARCoreMauiViewHandler> Mapper =
        new PropertyMapper<ARCoreMauiView, ARCoreMauiViewHandler>();

    public ARCoreMauiViewHandler()
        : base(Mapper)
    {
    }

    protected override FrameLayout CreatePlatformView()
    {
        var container = new FrameLayout(Context);

        var arView = new ARCoreView();

        container.AddView(
            arView,
            new FrameLayout.LayoutParams(
                Android.Views.ViewGroup.LayoutParams.MatchParent,
                Android.Views.ViewGroup.LayoutParams.MatchParent));

        VirtualView.NativeARView = arView;

        return container;
    }

    protected override void ConnectHandler(FrameLayout platformView)
    {
        base.ConnectHandler(platformView);
        VirtualView.NativeARView?.ResumeAR();
    }

    protected override void DisconnectHandler(FrameLayout platformView)
    {
        VirtualView.NativeARView?.PauseAR();
        base.DisconnectHandler(platformView);
    }
}

#endif