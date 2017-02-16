using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MightyB.core;
using MightyB.UI;

namespace MightyB
{
    public partial class ScreenSaverForm : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        #region --- Class Contructor ---------------------------------------------------------------------
        public ScreenSaverForm()
        {
            this.InitializeComponent();
            this.InitializeForm();
        }

        public ScreenSaverForm(IntPtr ptr) : this()
        {
            // Set the preview window as the parent of this window
            SetParent(this.Handle, ptr);

            // Make this a child window so it will close when the parent dialog closes
            // GWL_STYLE = -16, WS_CHILD = 0x40000000
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));

            // Place our window inside the parent
            Rectangle parentRect;
            GetClientRect(ptr, out parentRect);
            this.Size = parentRect.Size;
            this.Location = new Point(0, 0);

            this.IsPreview = true;
        }

        #endregion --- Class Constructor -----------------------------------------------------------------

        #region --- Properties ---------------------------------------------------------------------------

        private IScreenSaver _Saver;
        private Point _MouseLocation = new Point();

        #region IsPreview
        public bool IsPreview { get; set; }
        #endregion IsPreview

        #region RandomSeed
        public int RandomSeed { get; set; }
        #endregion RandomSeed

        #endregion --- Properties ------------------------------------------------------------------------

        #region --- Event Handlers -----------------------------------------------------------------------

        #region OnDisposed
        private void OnDisposed(object sender, EventArgs e)
        {
            if (this._Saver != null)
                this._Saver.Stop();
        }
        #endregion OnDisposed

        #region OnLoad
        private void OnLoad(object sender, EventArgs e)
        {
#if !DEBUG
            if (!this.IsPreview)
            {
                Cursor.Hide();
                this.TopMost = true;
                this.Focus();
            }
#endif

            this._Saver = ScreenSaverManager.GetScreenSaver("lightning");
            if (this._Saver == null)
                return;

            var screenSaverControl = new ScreenSaverControl {Parent = this, Model = this._Saver, Dock = DockStyle.Fill};
#if !DEBUG
            if (!this.IsPreview)
            {
                screenSaverControl.MouseClick += this.OnMouseClick;
                screenSaverControl.MouseMove += this.OnMouseMove;
                screenSaverControl.KeyPress += this.OnKeyPress;
            }
#endif

            screenSaverControl.Start();            
        }
        #endregion OnLoad

        #region OnMouseClick
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (this.IsPreview) 
                return;

            Application.Exit();
        }
        #endregion OnMouseClick

        #region OnMouseMove
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsPreview)
                return;

            if (!this._MouseLocation.IsEmpty)
            {
                // Terminate if mouse is moved a significant distance
                if (Math.Abs(this._MouseLocation.X - e.X) > 5 ||
                    Math.Abs(this._MouseLocation.Y - e.Y) > 5)
                    Application.Exit();
            }

            // Update current mouse location
            this._MouseLocation = e.Location;
        }
        #endregion OnMouseMove

        #region OnKeyPress
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.IsPreview)
                return;

            Application.Exit();
        }
        #endregion OnKeyPress

        #endregion --- Event Handlers --------------------------------------------------------------------

        #region --- Methods ------------------------------------------------------------------------------

        #region InitializeForm
        private void InitializeForm()
        {
#if !DEBUG
            this.FormBorderStyle = FormBorderStyle.None;  //remove all decorations
            this.StartPosition = FormStartPosition.Manual;
#endif

            //wire events
            this.Load += this.OnLoad;
            this.Disposed += this.OnDisposed;

#if !DEBUG
            this.KeyPress += this.OnKeyPress;
            this.MouseMove += this.OnMouseMove;
            this.MouseClick += this.OnMouseClick;
#endif
        }
        #endregion InitializeForm

        #region OnFormClosed
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            if (!this.IsPreview)
                Application.Exit();
        }
        #endregion OnFormClosed

        #endregion --- Methods ---------------------------------------------------------------------------


    }
}
