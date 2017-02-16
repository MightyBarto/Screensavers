using System;
using System.Drawing;
using MightyB.core;

namespace MightyB.Spotlight
{
    internal class Ray : IDrawable
    {
        #region --- Constants ----------------------------------------------------------------------------
        private const int _MAX_LIFESPAN = 50;
        private const int _MIN_LIFESPAN = 5;
        private const int _MAX_SHIFT = 20;
        private const int _MAX_THICKNESS = 8;
        #endregion --- Constants -------------------------------------------------------------------------

        #region --- Class Constructor --------------------------------------------------------------------
        public Ray(Ray parent)
        {
            this.Parent = parent;            
        }
        #endregion --- Class Constructor -----------------------------------------------------------------

        #region --- Properties ---------------------------------------------------------------------------

        #region Parent
        public Ray Parent { get; set; }
        #endregion Parent

        #region LeftChild
        public Ray LeftChild { get; set; }
        #endregion LeftChild

        #region RightChild
        public Ray RightChild { get; set; }
        #endregion RightChild

        #region StartPosition
        private Point _StartPosition;
        public Point StartPosition
        {
            get {return this.Parent == null ? this._StartPosition : this.Parent.StartPosition;}
            set { this._StartPosition = value; }
        }
        #endregion StartPosition

        #region EndPosition
        public Point EndPosition { get; set; }
        #endregion EndPosition

        #region Color
        private Color _Color = Color.Black;
        public Color Color
        {
            get { return this.Parent == null ? this._Color : this.Parent.Color; }
            set { this._Color = value; }
        }
        #endregion Color

        #region IsBurnedOut
        internal bool IsBurnedOut{ get { return this.Age >= this.LifeSpan; } }
        #endregion IsBurnedOut

        #region LifeSpan
        private int _LifeSpan = 10;
        public int LifeSpan
        {
            get { return this.Parent == null ? this._LifeSpan : this.Parent.LifeSpan; }
            set 
            {
                var bolt = this.Parent ?? this;
                bolt._LifeSpan = value;
            }
        }
        #endregion LifeSpan

        #region Thickness
        private int _Thickness = 2;
        public int Thickness
        {
            get { return this.Parent == null ? this._Thickness : this.Parent.Thickness; }
            set { this._Thickness = value; }
        }
        #endregion Thickness

        #region Age
        private int _Age;
        public int Age
        {
            get { return this.Parent == null ? this._Age : this.Parent.Age; }
            set { this._Age = value; }
        }
        #endregion Age

        #region SkipRate
        private int _SkipRate = 5;
        #endregion SkipRate

        #region ShouldSkip
        private bool _ShouldSkip;
        public bool ShouldSkip
        {
            get { return this.Parent == null ? this._ShouldSkip : this.Parent.ShouldSkip; }
            set { this._ShouldSkip = value; }
        }
        #endregion ShouldSkip

        #region Shifter
        private Random _Shifter = new Random();
        #endregion Shifter

        #endregion --- Properties ------------------------------------------------------------------------

        #region --- Methods ------------------------------------------------------------------------------

        #region Draw
        public void Draw(System.Drawing.Graphics graphics)
        {
            if (graphics == null) return;

            int x1, y1, x2, y2;

            //randomly shift a little at every endpoint
            var endXShift = this._Shifter.Next(_MAX_SHIFT);
            var endYShift = this._Shifter.Next(_MAX_SHIFT);
            var directionX = this._Shifter.Next(2);
            var directionY = this._Shifter.Next(2);

            var x = this.EndPosition.X; 
            var y = this.EndPosition.Y;

            x += directionX == 1 ? endXShift : -1 * endXShift;

            this.EndPosition = new Point(x, y);

            if (this.Parent == null)
            {
                this.Thickness = this._Shifter.Next(_MAX_THICKNESS);
                endXShift = this._Shifter.Next(_MAX_SHIFT);
                directionX = this._Shifter.Next(2);

                x = this.StartPosition.X;
                y = this.StartPosition.Y;

                x += directionX == 1 ? endXShift : -1 * endXShift;

                this.StartPosition = new Point(x, y);
            }

            if (this.LeftChild != null && this.RightChild != null)
            {
                x = this.EndPosition.X;
                y = this.EndPosition.Y;

                y += directionY == 1 ? endYShift : -1 * endYShift;

                this.EndPosition = new Point(x, y);
            }

            x1 = this.StartPosition.X;
            y1 = this.StartPosition.Y;

            x2 = this.EndPosition.X; 
            y2 = this.EndPosition.Y;
            
            //randomly skip a tick
            if (this.Parent == null)
                this.ShouldSkip = this._Shifter.Next(this._SkipRate) == 0;

            if (!this.ShouldSkip)
            {
                var pen = new Pen(this.Color, this.Thickness);
                graphics.DrawLine(pen, x1, y1, x2, y2);
            }

            if (this.LeftChild != null)
            {
                this.LeftChild.Draw(graphics);
            }

            if (this.RightChild != null)
            {
                this.RightChild.Draw(graphics);
            }

            if (this.Parent == null)
                this.Age++;
        }
        #endregion Draw

        #region GenerateLifeSpan
        public void GenerateLifeSpan()
        {
            this.LifeSpan = new Random().Next(_MIN_LIFESPAN, _MAX_LIFESPAN);
        }
        #endregion GenerateLifeSpan

        #region GenerateX
        public int GenerateX(int max)
        {

            int parentX; //,parentY;
            var generator = new Random();

            parentX = this.StartPosition.X;
            //parentY = getStartPosition().y;

            if (this.Parent != null)
            {

                if (this.Parent.LeftChild == this)                
                    //generate point to the left
                    return generator.Next(parentX);                
                                
                return generator.Next(max - parentX) + parentX;                
            }
          
            return 0;
            

        }
        #endregion GenerateX        

        #endregion --- Methods ---------------------------------------------------------------------------
    }
}
