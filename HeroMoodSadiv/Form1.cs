using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using Newtonsoft.Json;
using Microsoft.ProjectOxford.Face;

namespace HeroMoodSadiv
{
    public partial class Form1 : Form
    {
        private bool ffmpegEnded = true;
        private Process frameing = new Process();
        FaceServiceClient fSC = new FaceServiceClient("14a7562cf9a54dccacb26ca90f309f65");


        //private List<faceSmile> fs;
        faceSmile ff = new faceSmile();

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (ffmpegEnded)
            {
                frameing.StartInfo.FileName = @"C:\ffmpeg\bin\ffmpeg.exe";
                frameing.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                frameing.StartInfo.Arguments = "-i \"" + textBox1.Text + "\" -vf fps=1/60 img%d.png";

                ffmpegEnded = false;

                frameing.EnableRaisingEvents = true;
                frameing.Exited += new EventHandler(AsyncFrames);
                frameing.Start();
            }

            int f = 2;
            while (!ffmpegEnded)
            {
                if (File.Exists($"img{f}.png"))
                {
                    pictureBox1.Image = Image.FromFile($"img{f - 1}.png");
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Refresh();
                    f++;

                    await Search($"img{f - 1}.png");
                }
            }

            Guid[] gHero = await Group();

            //

        }


        private async Task<Guid[]> Group()
        {
            var group = await fSC.GroupAsync(ff.faceId.ToArray());
            return group.Groups[0];
        }


        private async Task Search(string q)
        {

            //FaceServiceClient fSC = new FaceServiceClient("14a7562cf9a54dccacb26ca90f309f65");
            List<FaceAttributeType> faceA = new List<FaceAttributeType>();
            faceA.Add(FaceAttributeType.Age);
            faceA.Add(FaceAttributeType.Gender);
            faceA.Add(FaceAttributeType.Smile);

            using (Stream s = File.OpenRead(q))
            {
                var faces = await fSC.DetectAsync(s, true, false, faceA);
                foreach (var face in faces)
                {
                    
                    ff.faceId.Add(face.FaceId);
                    ff.smile.Add(face.FaceAttributes.Smile);

                }
            }
            
        }


        private void AsyncFrames(object sender, System.EventArgs e)
        {
            ffmpegEnded = true;
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    textBox1.Text = file;
                }
                catch (IOException)
                {
                }


            }
        }
    }
}
