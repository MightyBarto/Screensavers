using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MightyB.Radar
{
    internal class RadarPulse
    {
        #region --- Constants ----------------------------------------------------------------------------

        private const decimal __Increment = 1.0M; //0.4M;
        private const int __DetectRange = 5;
        private const int __Thickness = 4;

        #endregion --- Constants -------------------------------------------------------------------------

        #region --- Properties ---------------------------------------------------------------------------

        #region Angle
        public decimal Angle { get; set; }
        #endregion Angle

        #region Center
        public Point Center { get; set; }
        #endregion Center

        #region Length
        public int Length { get; set; }
        #endregion Length

        #region Start
        public int InnerOffSet { get; set; }
        #endregion Start

        #region Pen
        private readonly Pen _Pen = new Pen(Color.Green, __Thickness);
        #endregion Pen

        #region StartPoint
        public Point StartPoint
        {
            get
            {
                var radians = Math.PI * (double)this.Angle / 180.0;

                var sinStart = Math.Sin(radians) * this.InnerOffSet;
                var cosStart = Math.Cos(radians) * this.InnerOffSet;

                var radarStartX = (int)sinStart;
                var radarStartY = (int)cosStart;

                return new Point(radarStartX, radarStartY);
            }
        }
        #endregion StartPoint

        #region EndPoint
        public Point EndPoint
        {
            get
            {
                var radians = Math.PI * (double)this.Angle / 180.0;

                var sin = Math.Sin(radians) * this.Length;
                var cos = Math.Cos(radians) * this.Length;

                var x = (int)sin;
                var y = (int)cos;

                return new Point(x, y);
            }
        }
        #endregion EndPoint

        #endregion --- Properties ------------------------------------------------------------------------

        #region --- Methods ------------------------------------------------------------------------------

        #region Draw
        public void Draw(Graphics graphics)
        {           
            var radarStart = this.StartPoint;
            var radarEnd = this.EndPoint;

            graphics.DrawLine(this._Pen,
                              radarStart.X + this.Center.X,
                              (radarStart.Y * -1) + this.Center.Y,
                              radarEnd.X + this.Center.X,
                              (radarEnd.Y * -1) + this.Center.Y);
        }
        #endregion Draw

        #region Increment
        public void Increment()
        {
            this.Angle += __Increment;
            this.Angle %= 360M;
        }
        #endregion Increment

        #region GetDistance
        public double GetDistance(Point p1, Point p2)
        {
            return Math.Sqrt(
                        Math.Pow(p1.X - p2.X, 2)
                        +
                        Math.Pow(p1.Y - p2.Y, 2));
        }

        private double GetDistance(Detectable detectable)
        {
            var startPoint = this.StartPoint;
            var endPoint = this.EndPoint;

            var x0 = startPoint.X;
            var y0 = startPoint.Y;
            var x1 = endPoint.X;
            var y1 = endPoint.Y;
            var xp = detectable.Location.X;
            var yp = detectable.Location.Y;

            var lambda = (((x1 - x0) * (xp - x0)) + ((y1 - y0) * (yp - y0)))
                            /
                            (Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2));

            var dist = Math.Sqrt(Math.Pow(xp - x0 - (lambda * (x1 - x0)), 2)
                                    + Math.Pow(yp - y0 - (lambda * (y1 - y0)), 2));
            return dist;
        }
        #endregion GetDistance

        #region CanDetect
        public bool CanDetect(Detectable detectable)
        {
            var dist = this.GetDistance(detectable);

            if (dist > __DetectRange ||
                !this.SameQuadrant(this.EndPoint,detectable.Location) ||
                this.GetDistance(detectable.Location,new Point(0,0)) > this.Length)
                return false;

            return true;
        }
        #endregion CanDetect

        #region SameQuadrant
        private bool SameQuadrant(Point p1, Point p2)
        {

            //if the product of two numbers is positive, then they 
            //have the same sign
            return p1.X * p2.X > 0 && p1.Y * p2.Y > 0;

        }
        #endregion SameQuadrant

        #endregion --- Methods ---------------------------------------------------------------------------
    }
}
