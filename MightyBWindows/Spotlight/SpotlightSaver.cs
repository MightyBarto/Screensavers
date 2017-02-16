using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using MightyB.core;
using MightyB.UI;

namespace MightyB.Spotlight
{
    internal sealed class SpotlightSaver : IScreenSaver
    {

        #region --- Properties ---------------------------------------------------------------------------

        #region RandomSeed
        private int _RandomSeed;
        public int RandomSeed
        {
            get
            {
                return this._RandomSeed;
            }
            set
            {
                this._RandomSeed = value;
                this._Generator = new Random(value);
            }
        }
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
        private List<Ray> _Rays = new List<Ray>(); 
        #endregion Bolts

        private int _SnapToBottom = 30;
        private int _SplitChance = 6;
        private Ray _TopBolt;
        private Random _Generator;

        private int _BoltSpawnRate = 30;
        private int _Tick = 50;
        private bool _Stopped;

        private object _LockObject = new object();

        #endregion --- Properties ------------------------------------------------------------------------

        #region --- Methods ------------------------------------------------------------------------------

        #region GenerateBolt
        private Ray GenerateBolt(Ray parent, BranchType type)
        {
            lock (this._LockObject)
            {
                var newBolt = new Ray(parent);
                var canvasSize = this._View.Size;

                int startX, startY, endX, endY; //,heightReached;
                int seed;              

                //generate start coords

                if (parent == null)
                {
                    startY = 0;

                    seed = canvasSize.Width;
                    if (seed <= 0) seed = 1;

                    startX = this._Generator.Next(seed);

                    //generate random color  
                    // color = Color.FromArgb(this._Generator.Next(255),this._Generator.Next(255),this._Generator.Next(255));

                    newBolt.Color = Color.FromArgb(this._Generator.Next(255), this._Generator.Next(255), this._Generator.Next(255)); ;

                    seed = canvasSize.Width;
                    if (seed <= 0) seed = 1;

                    endX = this._Generator.Next(seed);

                    seed = canvasSize.Height;
                    if (seed <= 0) seed = 1;

                    endY = this._Generator.Next(seed);

                }
                else
                {
                    //base all settings on parent
                    startY = parent.EndPosition.Y;
                    startX = parent.EndPosition.X;

                    //color  = parent.Color;

                    seed = canvasSize.Height - startY;
                    if (seed <= 0) seed = 1;

                    endY = this._Generator.Next(seed) + startY;

                    if (type == BranchType.Left)
                    {
                        seed = startX;
                        if (seed <= 0) seed = 1;

                        endX = this._Generator.Next(seed);

                    }
                    else if (type == BranchType.Right)
                    {
                        seed = canvasSize.Width - startX;
                        if (seed <= 0) seed = 1;

                        endX = this._Generator.Next(seed) + startX;
                    }
                    else
                    {
                        //should never be reached
                        seed = startX;
                        if (seed <= 0) seed = 1;

                        endX = this._Generator.Next(seed);
                    }

                }

                newBolt.StartPosition = new Point(startX, startY);
                newBolt.EndPosition = new Point(endX, endY);

                if (newBolt.EndPosition.Y > canvasSize.Height - this._SnapToBottom)
                {
                    var x = newBolt.EndPosition.X;
                    var y = canvasSize.Height;
                    newBolt.EndPosition = new Point(x, y);
                }

                //generate child bolts if necessary
                if (newBolt.EndPosition.Y < canvasSize.Height)
                {
                    //recurse

                    var choice = this._Generator.Next(this._SplitChance);

                    if (choice == 0)
                    {
                        newBolt.LeftChild = this.GenerateBolt(newBolt, BranchType.Left);
                        newBolt.RightChild = this.GenerateBolt(newBolt, BranchType.Right);
                    }
                    else
                    {
                        var side = this._Generator.Next(2);

                        if (side == 1)
                            newBolt.LeftChild = this.GenerateBolt(newBolt, BranchType.Left);
                        else
                            newBolt.RightChild = this.GenerateBolt(newBolt, BranchType.Right);
                    }
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
                    lock (this._Rays)
                    {
                        //add a new bolt
                        var b = this.GenerateBolt(null, BranchType.None);

                        //generate a lifespan
                        b.GenerateLifeSpan();

                        //add bolt to collection
                        this._Rays.Add(b);
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
            this._Rays.Clear();
        }
        #endregion Stop

        #region Draw
        public void Draw(System.Drawing.Graphics g)
        {
            for (var i=0;i<this._Rays.Count;++i)
            {
                var b = this._Rays[i];
                b.Draw(g);
                if (b.IsBurnedOut)
                    this._Rays.Remove(b);
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
