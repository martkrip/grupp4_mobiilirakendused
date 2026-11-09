
using Android.App;
using Google.AR.Core;

namespace MauiAndroidTest.Platforms.Android.AR;

public class ARService
{
    private Session? arSession;

    public bool IsInitialized => arSession != null;

    public void Initialize(Activity activity)
    {
        if (arSession != null)
            return;

        try
        {
            arSession = new Session(activity);
        }
        catch (Exception ex)
        {
            throw new Exception(
                "Could not initialize ARCore: " + ex.Message);
        }
    }

    public void Pause()
    {
        arSession?.Pause();
    }

    public void Resume()
    {
        arSession?.Resume();
    }

    public void Dispose()
    {
        arSession?.Dispose();
        arSession = null;
    }
}
