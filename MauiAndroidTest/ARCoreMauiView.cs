using MauiView = Microsoft.Maui.Controls.View;

namespace MauiAndroidTest;

public class ARCoreMauiView : MauiView
{
#if ANDROID
    public ARCoreView? NativeARView { get; internal set; }
#endif
}