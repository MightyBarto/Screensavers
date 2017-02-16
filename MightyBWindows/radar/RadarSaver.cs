using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MightyB.core;
using MightyB.UI;

namespace MightyB.Radar
{
    internal class RadarSaver : IScreenSaver
    {
        #region --- Properties ---------------------------------------------------------------------------

 //       private IEnumerable<Image> _Images; 

        private bool _Stopped;       
        private readonly Color _Color = Color.Green;
        private Point _Centre;
        private const int _Thickness = 3;
        private Pen _Pen;

        #region RandomSeed
        public int RandomSeed { get; set; }
        #endregion RandomSeed

        #region TickSpeed
        private int _TickSpeed = 50;
        public int TickSpeed
        {
            set { this._TickSpeed = value; }
        }
        #endregion TickSpeed

        #region View
        private IScreenSaverView _View;
        public IScreenSaverView View
        {
            get { return this._View; }
            set 
            { 
                this._View = value;
                if (value.Model != this)
                    value.Model = this;
            }
        }
        #endregion View

        #region PulseLine
        private RadarPulse _PulseLine;
        public RadarPulse PulseLine
        {
            get { return this._PulseLine ?? (this._PulseLine = new RadarPulse()); }
        }
        #endregion PulseLine

        #region Detectables
        private IList<Detectable> _Detectables = new List<Detectable>();
        #endregion Detectables

        #endregion --- Properties ------------------------------------------------------------------------

        #region --- Methods ------------------------------------------------------------------------------

        #region Draw
        public void Draw(Graphics graphics)
        {
            try
            {
                this.Spawn();

                this.PulseLine.Draw(graphics);
                this.PulseLine.Increment();

                foreach (var detectable in this._Detectables)
                {
                    if (this.PulseLine.CanDetect(detectable))
                    {
                        detectable.Draw(graphics);
                    }
                }              
            }
            catch { }
        }
        #endregion Draw

        #region LoadImages

        #endregion LoadImages

        #region Run
        private void Run()
        {            
            while (!this._Stopped)
            {
                if(this.View == null)
                {
                    this._Stopped = true;
                    continue;
                }

                this.View.DrawModel();

                Thread.Sleep(this._TickSpeed);
            }
        }
        #endregion Run

        #region Start
        public void Start()
        {
            //var dir = new System.IO.DirectoryInfo(@"D:\temp\Images");
            //if (dir.Exists)
            //{

            //}
            
            //this._Images = (from f in dir.EnumerateFiles()
            //                let name = f.Name.ToLower()
            //                where name.EndsWith(".jpg") ||
            //                      name.EndsWith(".jpeg") ||
            //                      name.EndsWith(".png")
            //                select Image.FromFile(f.FullName));

            this._Pen = new Pen(this._Color, _Thickness); 
            this._Centre = new Point(this._View.Size.Width / 2, this._View.Size.Height / 2);

            this.PulseLine.Center = this._Centre;
            this.PulseLine.InnerOffSet = 20;

            var size = this.GetTotalViewPortSize();
            var length = (int)Math.Sqrt((Math.Pow(size.Width, 2) + Math.Pow(size.Height, 2))) / 2; //needs to be only half the size of the screen
            this.PulseLine.Length = length;
           
            this._Stopped = false;
            new Thread(this.Run).Start();                      
        }
        #endregion Start

        #region Stop
        public void Stop()
        {
            this._Stopped = true;
        }
        #endregion Stop 

        #region GetTotalViewPortSize
        private Size GetTotalViewPortSize()
        {
            var minX = 0;
            var minY = 0;
            var maxX = 0;
            var maxY = 0;

            foreach(var screen in Screen.AllScreens)
            {
                if (screen.Bounds.X < minX)
                    minX = screen.Bounds.X;
           
                if(screen.Bounds.Y < minY)
                    minY = screen.Bounds.Y;

                if (screen.Bounds.Right > maxX)
                    maxX = screen.Bounds.Right;

                if (screen.Bounds.Bottom > maxY)
                    maxY = screen.Bounds.Bottom;
            }

            var width = maxX - minX;
            var height = maxY - minY;

            return new Size(width, height);
        }
        #endregion GetTotalViewPortSize

        #region Spawn
        private void Spawn()
        {
            //remove this test 
            if (this._Detectables.Count > 0)
                return;

            var p = new Point(100, 100);

            var ufo = new Detectable
            {
                Bounds = new Rectangle(0, 0, this._View.Size.Width, this._View.Size.Height),
                Location = p
            };

            this._Detectables.Add(ufo);
        }
        #endregion Spawn

        #endregion --- Methods ---------------------------------------------------------------------------
    }
}
