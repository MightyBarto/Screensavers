using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MightyB.UI;

namespace MightyB.core
{
    internal interface IScreenSaver : IDrawable
    {
        void Start();
        void Stop();
        int TickSpeed { set; }
        int RandomSeed { get; set; }
        IScreenSaverView View { get; set; }
    }
}
