using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PauseTokenSource
{
    public partial class FormMain : Form
    {
        PauseTokenSource pts;
        CancellationTokenSource cts;

        public FormMain()
        {
            InitializeComponent();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnPause.Enabled = true;
            btnStop.Enabled = true;
            rbResult.Clear();
            cts = new CancellationTokenSource();
            pts = new PauseTokenSource();
            CancellationToken tokenSource = cts.Token;
            await SomeMethodAsync(tokenSource);
        }

        public async Task SomeMethodAsync(CancellationToken ct)
        {

            for (int i = 0; i <= 100; i++)
            {
                this.BeginInvoke(new Action(() =>
                {
                    rbResult.Text += "i = " + i + "\r\n";
                    progressBar.Value = i;
                    lbl_status.Text = i + "%";
                    if (i == 100)
                    {
                        MessageBox.Show("Finish");
                        btnStart.Enabled = true;
                        btnPause.Enabled = false;
                        btnStop.Enabled = false;
                    }
                }));

                if (ct.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(200);
                await pts.WaitWhilePausedAsync();
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            pts.IsPaused = !pts.IsPaused;
            if (pts.IsPaused)
            {
                btnPause.Text = "Resume";
            }
            else
            {
                btnPause.Text = "Pause";
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            cts.Cancel();
            btnStart.Enabled = true;
            btnPause.Enabled = false;
            btnStop.Enabled = false;
        }
    }
}
