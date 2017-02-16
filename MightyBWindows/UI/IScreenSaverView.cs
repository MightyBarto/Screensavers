using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MightyB.core;

namespace MightyB.UI
{
    internal interface IScreenSaverView
    {
        void Start();
        void Stop();
        IScreenSaver Model { get; set; }
        Size Size { get; set; }
        void DrawModel();
        Component Component { get; }
    }
}
