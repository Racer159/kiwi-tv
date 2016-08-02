using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

namespace Kiwi_TV.Helpers
{
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