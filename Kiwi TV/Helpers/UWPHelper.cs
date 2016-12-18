using Windows.System.Profile;

namespace Kiwi_TV.Helpers
{
    /// <summary>
    /// A helper to determine the current device type
    /// </summary>
    class UWPHelper
    {
        public static DeviceFormFactorType GetDeviceFormFactorType()
        {
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Phone":
                    return DeviceFormFactorType.Phone;
                case "Windows.Mobile":
                    return DeviceFormFactorType.Phone;
                case "Windows.Desktop":
                    return DeviceFormFactorType.Desktop;
                case "Windows.Universal":
                    return DeviceFormFactorType.IoT;
                case "Windows.Team":
                    return DeviceFormFactorType.SurfaceHub;
                case "Windows.Xbox":
                    return DeviceFormFactorType.Xbox;
                default:
                    return DeviceFormFactorType.Other;
            }
        }
    }

    public enum DeviceFormFactorType
    {
        Phone,
        Desktop,
        IoT,
        SurfaceHub,
        Xbox,
        Other
    }
}