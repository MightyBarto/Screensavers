using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using MightyB.core;
using MightyB.UI;

namespace MightyB.lightning
{
    internal sealed class LightningSaver : IScreenSaver
    {
        #region --- Properties ---------------------------------------------------------------------------

        #region RandomSeed
        public int RandomSeed { get; set; }
        #endregion RandomSeed

        #region TickSpeed
        private int _TickSpeed;
        public int TickSpeed {
            set
            {
                this._TickSpeed = value;
            }
        }
        #endregion TickSpeed

        #region View
        private IScreenSaverView _View;
        public IScreenSaverView View
        {
            get
            {
                return this._View;
            }
            set 
            { 
                this._View = value; 
                if(value == null || value.Model == this)
                    return;
                
                value.Model = this;
            }
        }
        #endregion View

        #region Bolts
        private List<Bolt> _Bolts = new List<Bolt>(); 
        #endregion Bolts

        private int _SnapToBottom = 70;
        private int _SplitChance = 6;
        private Bolt _TopBolt;
        private Random _Generator;

        private int _BoltSpawnRate = 30;
        private int _Tick = 50;
        private bool _Stopped;

        private object _LockObject = new object();

        #endregion --- Properties ------------------------------------------------------------------------

        #region --- Methods ------------------------------------------------------------------------------

        #region GenerateBolt
        private Bolt GenerateBolt(Bolt parent, BranchType type, Rectangle bounds)
        {
            lock (this._LockObject)
            {
                var newBolt = new Bolt(parent);
             
                int startX, startY;                             

                //generate start coords

                if (parent == null)
                {
                    startY = 0;
                    startX = this._Generator.Next(bounds.X,bounds.X + bounds.Width);                                                        
                    newBolt.Color = Color.FromArgb(this._Generator.Next(255), this._Generator.Next(255), this._Generator.Next(255)); ;
                }
                else
                {
                    //base all settings on parent
                    startY = parent.EndPosition.Y;
                    startX = parent.EndPosition.X;              
                }

                var endY = this._Generator.Next(bounds.Y, bounds.Y + bounds.Height);
                var endX = this._Generator.Next(bounds.X, bounds.X + bounds.Width);

                if (endY > this._View.Size.Height - this._SnapToBottom)
                    endY = this._View.Size.Height;
                
                newBolt.StartPosition = new Point(startX, startY);
                newBolt.EndPosition = new Point(endX, endY);

                //generate child bolts if necessary
                if (newBolt.EndPosition.Y >= this._View.Size.Height) return newBolt;
                var side = this._Generator.Next(3);

                switch (side)
                {
                    case 0:
                        newBolt.LeftChild = this.GenerateBolt(newBolt,
                            BranchType.Left,
                            new Rectangle(bounds.X, endY, endX - bounds.X, this._View.Size.Height - endY));
                        break;
                    case 1:
                        newBolt.RightChild = this.GenerateBolt(newBolt,
                            BranchType.Right,
                            new Rectangle(endX, endY, bounds.X + bounds.Width - endX, this._View.Size.Height - endY));
                        break;
                    case 2:
                        newBolt.LeftChild = this.GenerateBolt(newBolt,
                            BranchType.Left,
                            new Rectangle(bounds.X, endY, endX - bounds.X, this._View.Size.Height - endY));
                        newBolt.RightChild = this.GenerateBolt(newBolt,
                            BranchType.Right,
                            new Rectangle(endX, endY, bounds.X + bounds.Width - endX, this._View.Size.Height - endY));
                        break;
                }
                return newBolt;
            }
        }
        #endregion GenerateBolt

        #region Run
        public void Run()
        {
            this._Generator = new Random(this.RandomSeed);

            while (!this._Stopped)
            {
                //decide whether to generate a new bolt
                var spawn = this._Generator.Next(this._BoltSpawnRate) == 0;

                if (spawn)
                {
                    lock (this._Bolts)
                    {
                        //add a new bolt
                        var b = this.GenerateBolt(null, BranchType.None, new Rectangle(0, 0, this._View.Size.Width, this._View.Size.Height));

                        //generate a lifespan
                        b.GenerateLifeSpan();

                        //add bolt to collection
                        this._Bolts.Add(b);                        
                    }
                }

                this._View.DrawModel();
                
                try
                {
                    if (!this._Stopped)
                        Thread.Sleep(this._Tick);

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }                
            }
        }
        #endregion Run

        #region Start
        public void Start()
        {
            this._Stopped = false;
            new Thread(this.Run).Start();
        }
        #endregion Start

        #region Stop
        public void Stop()
        {
            this._Stopped = true;
            this._Bolts.Clear();
        }
        #endregion Stop

        #region Draw
        public void Draw(System.Drawing.Graphics g)
        {
            for (var i=0;i<this._Bolts.Count;++i)
            {
                var b = this._Bolts[i];
                b.Draw(g);
                if (b.IsBurnedOut)
                    this._Bolts.Remove(b);
            }           
        }
        #endregion Draw

        #region ToString
        public override string ToString()
        {
            return "Lightning";
        }
        #endregion ToString

        #endregion --- Methods -----------------------------------------------------------------
    }
}
