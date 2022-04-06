using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QuickOpening.Constants;
using static QuickOpening.KeyboardManager;
using static QuickOpening.Shortcut;


namespace QuickOpening
{
    public partial class Main : Form
    {
        #region variables

        private List<QOObject> config;

        private NotifyIcon ni;

        private PictureBox temp = new PictureBox();

        private readonly string pathAutostart = $"{Environment.GetFolderPath(Environment.SpecialFolder.Startup)}\\{Application.ProductName}.lnk";

        #endregion

        public Main()
        {
            InitializeComponent();

            #region file

            if (!Directory.Exists(images)) Directory.CreateDirectory(images);

            if (!File.Exists(fileSettings))
                File.WriteAllText(fileSettings, JsonConvert.SerializeObject(new List<QOObject>(Enumerable.Repeat(new QOObject(), 9)), Formatting.Indented));

            if (File.Exists(pathAutostart)) cms1IsAutoStart.Checked = true;

            #endregion

            #region event

            foreach (PictureBox pbx in this.Controls.OfType<PictureBox>())
            {
                pbx.AllowDrop = true;
                pbx.MouseDown += new MouseEventHandler(pictureBox_MouseDown);
                pbx.DragEnter += new DragEventHandler(pictureBox_DragEnter);
                pbx.DragDrop += new DragEventHandler(pictureBox_DragDrop);
            }

            #endregion

            #region notify
            ni = new NotifyIcon { Icon = Properties.Resources.AppIcon, ContextMenuStrip = cms1, Visible = true };
            ni.DoubleClick += (s, e) => { this.Show(); this.TopMost = true; };

            cms1IsAutoStart.Click += IsAutoStart_Click;

            cms1Close.Click += (s, e) => { ni.Dispose(); Environment.Exit(0); };

            Load += (s, e) => { Task.Run(() => { this.Hide(); }); };
            FormClosing += (s, e) => { e.Cancel = true; this.Hide(); };
            #endregion

            #region register button

            for (int i = 0; i < modifirs.Count; i++)
                for (int j = 0; j < keys.Count; j++)
                    KeyboardManager.RegisterHotKey(keys[j], modifirs[i]);
            KeyboardManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyPressed);

            #endregion

            config = JsonConvert.DeserializeObject<List<QOObject>>(File.ReadAllText(fileSettings));
            Update();
        }

        private new void Update()
        {
            foreach (PictureBox pbx in this.Controls.OfType<PictureBox>())
                pbx.ImageLocation = config[TakeId((pbx).Name)].pathImage;
        }

        private int TakeId(string text)
        {
            return Convert.ToInt32(text.Substring(text.Length - 1)) - 1;
        }

        private void IsAutoStart_Click(object sender, EventArgs e)
        {
            if (cms1IsAutoStart.Checked) DeleteStartupShortcut();
            else CreateStartupShortcut($"For autostart.\nРасположение:{Application.ExecutablePath}");

            cms1IsAutoStart.Checked = !cms1IsAutoStart.Checked;
        }

        private void HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            int id = TakeId(e.Key.ToString());

            if (e.Modifiers == KeyModifiers.Control)
            {
                if (config[id].pathObj != null) OpenObj(config[id]);
            }

            if (e.Modifiers == KeyModifiers.Alt)
            {
                if (config[id].pathImage != null) File.Delete(config[id].pathImage);
                config[id] = new QOObject();
                File.WriteAllText(fileSettings, JsonConvert.SerializeObject(config, Formatting.Indented));
                Update();
            }
        }

        private void OpenObj(QOObject AS)
        {
            ProcessStartInfo psi = new ProcessStartInfo(AS.pathObj) { UseShellExecute = true };
            if (AS.isFolder == true) psi.Arguments = "explorer.exe";
            Process.Start(psi);
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if ((sender as PictureBox).ImageLocation != null)
            {
                temp = (sender as PictureBox);
                (sender as PictureBox).DoDragDrop((sender as PictureBox).Image, DragDropEffects.Move);
            }
        }

        private void pictureBox_DragEnter(object sender, DragEventArgs dea) { dea.Effect = DragDropEffects.Move; }

        private void pictureBox_DragDrop(object sender, DragEventArgs dea)
        {
            PictureBox here = (sender as PictureBox);
            if (dea.Data.GetDataPresent(DataFormats.Bitmap))
            {
                QOObject newObj = new QOObject(config[TakeId(temp.Name)]);
                config[TakeId(temp.Name)] = new QOObject(config[TakeId(here.Name)]);
                config[TakeId(here.Name)] = new QOObject(newObj);
            }
            else if (dea.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string pathobj = ((string[])dea.Data.GetData(DataFormats.FileDrop)).First();
                string nameobj = pathobj.Split('\\').Last();
                string ext = nameobj.Split('.').Last().ToLower();

                bool isfolder = true;
                string pathimage = "";
                if (extensionFile.Contains(ext) || extensionImage.Contains(ext))
                {
                    isfolder = false;
                    if (extensionFile.Contains(ext))
                    {
                        Icon q = Icon.ExtractAssociatedIcon(pathobj);
                        pathimage = $"{images}\\{nameobj.Split('.').First()}.png";
                        q.ToBitmap().Save(pathimage);
                    }
                    if (extensionImage.Contains(ext))
                    {
                        pathimage = $"{images}\\{nameobj.Split('.').First()}.{ext}";
                        temp.Image = Image.FromFile(pathobj);
                        temp.Image.Save(pathimage);
                        pathobj = pathimage;
                    }
                }
                else
                {
                    pathimage = $"{images}\\{nameobj.Split('.').First()}.png";
                    temp.Image = Properties.Resources.FolderIcon;
                    temp.Image.Save(pathimage);
                }

                if (config[TakeId(here.Name)].pathImage != null) File.Delete(config[TakeId(here.Name)].pathImage);
                config[TakeId(here.Name)] = new QOObject(pathobj, isfolder, pathimage);
            }
            File.WriteAllText(fileSettings, JsonConvert.SerializeObject(config, Formatting.Indented));
            Update();
        }
    }
}
