namespace tfd
{
    using System.Drawing;
    using System.Windows.Forms;

    public partial class AppForm : Form
    {
        protected override CreateParams CreateParams
        {
            //https://www.csharp411.com/hide-form-from-alttab/
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= EnvConfig.tfd_AppFormExStyle;
                return cp;
            }
        }

        private readonly ThreeFingerDragManager tfDragManager;

        public AppForm()
        {
            this.InitializeComponent();
            this.Load += (s, e) => this.Size = new Size(EnvConfig.tfd_AppFormWidth, EnvConfig.tfd_AppFormHeight);
            this.WindowState = EnvConfig.tfd_AppFormWindowState;
            this.FormBorderStyle = EnvConfig.tfd_AppFormWindowBorderStyle;
            this.ShowInTaskbar = EnvConfig.tfd_AppFormShowInTaskbar;
            this.Opacity = EnvConfig.tfd_AppFormOpacity;

            if (EnvConfig.tfd_EnableThreeFingerDrag)
            {
                this.tfDragManager = new ThreeFingerDragManager();
                bool registeredTrackpad = TrackpadHelper.RegisterTrackpad(this.Handle);
                Logger.Instance.Info($"registered trackpad={registeredTrackpad}");
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == win32.WM_INPUT)
            {
                TrackpadContact[] contacts = TrackpadHelper.ParseInput(m.LParam);
                this.tfDragManager?.ProcessTouch(contacts);
            }

            base.WndProc(ref m);
        }
    }
}