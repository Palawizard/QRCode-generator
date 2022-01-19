using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using QRCoder;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QRCode_form
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //Vérification de Save.txt
            if (File.Exists(@"Save.txt"))
            {
                //Update l'historique
                List<string> History2 = File.ReadAllLines(@"Save.txt").ToList();
                History2.Reverse(); ;
                listBox1.DataSource = History2;
            }
            else
            {
                //Crée un nouveau fichier texte en fermant le stream pour éviter les crashs
                var fileStream = File.Create(@"Save.txt");
                fileStream.Close();
                
                //Création d'un QRCode Placeholder
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode("Placeholder", QRCodeGenerator.ECCLevel.Q))
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);

                    //Met le QRCode sur la picturebox
                    pictureBox1.Image = qrCodeImage;
                }
            }
        }

        //Désactivations pour éviter les crashs
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = !string.IsNullOrEmpty(textBox1.Text);
            label2.Enabled = !string.IsNullOrEmpty(textBox1.Text);
        }

        //Génération du QRCode
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //Création du QRCode
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(textBox1.Text, QRCodeGenerator.ECCLevel.Q))
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);

                    //Met le QRCode sur la picturebox
                    pictureBox1.Image = qrCodeImage;
                }
            }
            //Catch pour éviter les crashs
            catch 
            {
                MessageBox.Show("Trop de caractères", "Erreur",
                  MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Bouton Sauvegarder
        private void button1_Click(object sender, EventArgs e)
        {
            //Crée la fênetre
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Images|*.png;*.bmp;*.jpg";
            ImageFormat format = ImageFormat.Png;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(sfd.FileName);
                switch (ext)
                {
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    case ".bmp":
                        format = ImageFormat.Bmp;
                        break;
                }

                //Sauvegarde l'image
                pictureBox1.Image.Save(sfd.FileName, format);

                //Sauvegarde le texte pour l'historique
                TextWriter tsw = new StreamWriter(@"Save.txt", true);
                tsw.WriteLine(textBox1.Text);
                tsw.Close();

                //Update l'historique
                List<string> History2 = File.ReadAllLines(@"Save.txt").ToList();
                History2.Reverse(); ;
                listBox1.DataSource = History2;
            }
        }

        //Cliquer pour enregistrer
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //évite les crashs
            if (pictureBox1.Image == null) { return; }
            
            //Copie dans le presse-papier
            Clipboard.SetImage(pictureBox1.Image);

            //Sauvegarde le texte pour l'historique
            TextWriter tsw = new StreamWriter(@"Save.txt", true);
            tsw.WriteLine(textBox1.Text);
            tsw.Close();

            //Update l'historique
            List<string> History2 = File.ReadAllLines(@"Save.txt").ToList();
            History2.Reverse(); ;
            listBox1.DataSource = History2;

            //Texte rouge
            label3.Visible = true;
            Task.Delay(1000).Wait();
            label3.Visible = false;
        }

        //Event de l'Historique
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Convertion de l'objet en string
            object item = listBox1.SelectedItem;
            string str1 = Convert.ToString(item);

            //Regénère le QRCode avec la sélection
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(str1, QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                //Met le QRCode sur la picturebox
                pictureBox1.Image = qrCodeImage;
            }
        }

        //Réinitialisation
        private void button2_Click(object sender, EventArgs e)
        {
            //Supprime tout dans le .txt
            TextWriter tsw = new StreamWriter(@"Save.txt");
            tsw.WriteLine("");
            tsw.Close();

            //Update l'historique
            List<string> History2 = File.ReadAllLines(@"Save.txt").ToList();
            History2.Reverse(); ;
            listBox1.DataSource = History2;

            //Régénère le QRCode avec la textbox
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(textBox1.Text, QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                //Met le QRCode sur la picturebox
                pictureBox1.Image = qrCodeImage;
            }
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //Régénérer le QRCode quand on clique sur la textbox
            try
            {
                //Création du QRCode
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(textBox1.Text, QRCodeGenerator.ECCLevel.Q))
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);

                    //Met le QRCode sur la picturebox
                    pictureBox1.Image = qrCodeImage;
                }
            }
            //Catch pour éviter les crashs
            catch (QRCoder.Exceptions.DataTooLongException)
            {
                MessageBox.Show("Trop de caractères", "Erreur",
                  MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}