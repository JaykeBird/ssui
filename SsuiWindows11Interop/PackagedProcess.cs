using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SolidShineUi;

/// <summary>
/// A helper class containing functionality to determine if the current process has a package identity.
/// </summary>
/// <remarks>
/// Taken from <a href="https://devblogs.microsoft.com/insidemsix/is-this-a-packaged-process/">
/// https://devblogs.microsoft.com/insidemsix/is-this-a-packaged-process/</a>
/// </remarks>
public static class PackagedProcess
{
    private const uint ERROR_INSUFFICIENT_BUFFER = 122;   // 0x007A
    private const uint APPMODEL_ERROR_NO_PACKAGE = 15700; // 0x3D54

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern uint GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

    /// <summary>
    /// Get if the current process has package identity.
    /// </summary>
    /// <returns><c>true</c> if it has package identity, <c>false</c> if it does not, or throws an error if it cannot determine</returns>
    /// <exception cref="Win32Exception">thrown if unable to determine if this has package identity or not</exception>
    public static bool IsPackagedProcess()
    {
        int n = 0;
        uint rc = GetCurrentPackageFullName(ref n, null!);
        if (rc == ERROR_INSUFFICIENT_BUFFER)
        {
            return true;
        }
        else if (rc == APPMODEL_ERROR_NO_PACKAGE)
        {
            return false;
        }
        else
        {
            throw new Win32Exception((int)rc);
        }
    }

    /// <summary>
    /// Try and get if the current process has package identity.
    /// </summary>
    /// <param name="isPackaged">
    /// output that indicates if the process has package identity;
    /// <c>true</c> indicates this has package identity; <c>false</c> indicates it does not, or unable to determine
    /// </param>
    /// <returns>
    /// a <c>uint</c> that was returned from when trying to determine if this process has package identity;
    /// if <paramref name="isPackaged"/> is <c>false</c>, this can contain an error code if its not <c>15700</c>
    /// </returns>
    public static uint TryGetIsPackagedProcess(out bool isPackaged)
    {
        int n = 0;
        uint rc = GetCurrentPackageFullName(ref n, null!);
        isPackaged = (rc == ERROR_INSUFFICIENT_BUFFER);
        return rc;
    }
}
