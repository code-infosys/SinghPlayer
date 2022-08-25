using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SinghPlayer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string[] files, path;
        int index = 0;
        private void btnOpenFiles_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                files = openFileDialog1.SafeFileNames;

                path = openFileDialog1.FileNames;

                for (int i = 0; i < files.Length; i++)
                {
                    listBox1.Items.Add(files[i]);
                }

                Play(path[index]);//start  
            }
        }

 

        WaveOutEvent player;
        public void Play(string fileurl)
        {
            try
            {
                player = new WaveOutEvent();
                var file = new AudioFileReader(fileurl);

                var trimmed = new OffsetSampleProvider(file);
                double fadeMilisec = 0;//take milisecoinds for faiding
                long mediaLengthSeconds = 0;

                trimmed.SkipOver = TimeSpan.FromSeconds(Convert.ToDouble(txtFromSec.Text));
                trimmed.Take = TimeSpan.FromSeconds(Convert.ToDouble(txtTakeSec.Text));
                fadeMilisec = (Convert.ToDouble(txtTakeSec.Text) * 1000) - 1300;
                mediaLengthSeconds = long.Parse(txtTakeSec.Text);

                player.PlaybackStopped += new EventHandler<StoppedEventArgs>(audioOutput_PlaybackStopped);

                var fadeOut = new DelayFadeOutSampleProvider(trimmed);//
                fadeOut.BeginFadeOut(fadeMilisec, 1300);//

                player.Init(fadeOut);

                //player.Init(trimmed); 
                player.Play();

                listBox1.SelectedIndex = index;
                listBox1.Refresh();

                //call timer
                timer1.Tick += Tick;
                CountDown(mediaLengthSeconds);
                //call timer end
            }
            catch (Exception ex)
            {
                if (index < listBox1.Items.Count)
                {
                    Play(path[index]);
                }
            }
        }

        //timer job begin
        DateTime start; //use for timer
        long s; //use for timer 

        public void CountDown(long seconds)
        {
            tBarProcessing.Minimum = 0;
            tBarProcessing.Maximum = Convert.ToInt32(seconds);

            start = DateTime.Now;
            s = seconds;
            timer1.Start();
        }
        private void Tick(object sender, EventArgs e)
        {
            Int32 count = 0;
            var remainingSeconds = s - (DateTime.Now - start).TotalSeconds;
            count = Convert.ToInt32((s - remainingSeconds));
            lblTime.Text = string.Format("{0}", Convert.ToInt32(count));
            tBarProcessing.Value = count;
            if (count == s)
            {
                timer1.Stop();
                lblTime.Text = "Done!";
                return;
            }


        }

        private void audioOutput_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            index = index + 1;
            if (index < listBox1.Items.Count)
            {
                //player.Stop();
                //player.Dispose();

                Play(path[index]);
            }
        }




    }
}
