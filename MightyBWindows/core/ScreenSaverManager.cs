using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MightyB.core
{
    internal static class ScreenSaverManager
    {
        internal static IScreenSaver GetScreenSaver(string name)
        {
            switch (name)
            {
                case "spotlight":
                    return new Spotlight.SpotlightSaver();
                case "lightning":
                    return new lightning.LightningSaver();
                case "radar":
                    return new Radar.RadarSaver();
                    break;
            }

            return null;
        }

    }
}
