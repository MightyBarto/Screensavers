using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MightyB.core;

namespace MightyB.UI
{
    internal partial class ScreenSaverControl : UserControl, IScreenSaverView
    {
        public ScreenSaverControl()
        {
            InitializeComponent();
            this.InitializeControl();
        }

        #region --- Properties ---------------------------------------------------------------------------

        private Graphics _Graphics;
        private BufferedGraphics _Buffer;

        #endregion --- Properties ------------------------------------------------------------------------

        #region --- Methods ------------------------------------------------------------------------------

        #region InitializeControl
        private void InitializeControl()
        {                      
            this.BackColor = Color.Black;
        }
        #endregion InitializeControl

        #region Start
        public void Start()
        {
            this._Graphics = this.CreateGraphics();
            this._Buffer = BufferedGraphicsManager.Current.Allocate(this._Graphics, this.Bounds);

            if (this.Model == null)
                return;

            this.Model.Start();
        }
        #endregion Start

        #region Stop
        public void Stop()
        {
            if (this.Model == null)
                return;

            this.Model.Stop();
        }
        #endregion Stop

        #region Model
        private IScreenSaver _Model;
        public IScreenSaver Model
        {
            get { return this._Model; }
            set
            {
                this._Model = value;
                if (value == null || value.View == this)
                    return;

                value.View = this;
            }
        }
        #endregion Model

        #region DrawModel
        public void DrawModel()
        {
            try
            {
                if (this.IsDisposed)
                    return;

                //var buffer = this._BufferContext.Allocate(this._Graphics, this.Bounds);
                this._Buffer.Graphics.Clear(Color.Black);

                if (this.Model == null)
                    return;

                this.Model.Draw(this._Buffer.Graphics);

                this._Buffer.Render(this._Graphics);
            }
            catch{}
        }
        #endregion DrawModel

        #region Component
        public Component Component
        {
            get { return this; }
        }
        #endregion Component

        #endregion --- Methods ---------------------------------------------------------------------------
    }
}
