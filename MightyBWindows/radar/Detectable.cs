using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MightyB.core;

namespace MightyB.Radar
{
    internal class Detectable : IDrawable
    {
        #region --- Properties ---------------------------------------------------------------------------

        #region Location
        public Point Location { get; set; }
        #endregion Location

        #region Color
        protected Color Color { get; set; }
        #endregion Color

        #region Thickness
        protected Size Thickness { get; set; }
        #endregion Thickness

        #region Bounds
        public Rectangle Bounds { get; set; }
        #endregion Bounds

        #region Speed
        protected int Speed { get; set; }
        #endregion Speed

        #region Size
        protected Size Size { get; set; }
        #endregion Size

        #endregion --- Properties ------------------------------------------------------------------------

        #region --- Class Constructors -------------------------------------------------------------------
        public Detectable()
        {
            this.Location = new Point();
            this.Bounds = new Rectangle();
            this.Color = Color.Red;
            this.Size = new Size(20, 20);
            this.Thickness = new Size(4, 4);
            this.Speed = 10;
        }
        #endregion --- Class Constructors ----------------------------------------------------------------

        #region --- Methods ------------------------------------------------------------------------------

        #region Draw
        public void Draw(Graphics graphics)
        {
            var pen = new Pen(this.Color, this.Thickness.Width);
            var location = this.Translate();

            graphics.DrawEllipse(pen,
                                location.X,
                                location.Y,
                                this.Size.Height,
                                this.Size.Width);
        }
        #endregion Draw

        #region Translate
        private Point Translate()
        {
            return new Point(this.Location.X + this.Bounds.Width / 2 - this.Size.Width / 2,
                            (this.Location.Y * -1) + this.Bounds.Height / 2 - this.Size.Width / 2);
        }
        #endregion Translate

        #endregion --- Methods ---------------------------------------------------------------------------
    }
}
