using Microsoft.Maui.ApplicationModel;

namespace MauiAndroidTest.Services;


public static class PermissionService 
{
    /*
     * Taaskasutatav permissionite küsimise helper
     * Kuidas kasutada: 
     * EnsurePermissionAsync<Permissions.permission>()
     */
    public static async Task<bool> EnsurePermissionAsync<TPermission>()
        where TPermission : Permissions.BasePermission, new()
    {
        var status = await Permissions.CheckStatusAsync<TPermission>();

        if (status == PermissionStatus.Granted)
            return true;

        status = await Permissions.RequestAsync<TPermission>();

        return status == PermissionStatus.Granted;
    }
}