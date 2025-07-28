using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.IO.Compression;



namespace PALLWWW
{
    public partial class Form1 : Form
    {
        string a = "1";
        string b = "1";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void HandleFabricOutput(string line)
        {
            if (line.Contains("Loading Fabric Installer")) progressBar1.Value = 20;
            else if (line.Contains("Installing 1.21.7")) progressBar1.Value = 40;
            else if (line.Contains("Scaricando libreria org.ow2")) progressBar1.Value = 60;
            else if (line.Contains("profile")) progressBar1.Value = 90;
            else if (line.Contains("Done") || line.Contains("successfully")) progressBar1.Value = 100;
            // you can expand this logic as needed
        }

        private async Task InstallFabricSilent()
        {
            while (!this.IsHandleCreated)
            {
                await Task.Delay(50);
            }

            progressBar1.Value = 0;

            this.Invoke(new Action(() =>
            {
                textBox1.Text = ("Installazione di fabric 1.21.7 in corso...");
            }));


            string tempJarPath = ExtractJar(); // your method that returns path of temp jar
            string version = "1.21.7";
            string args = $" -jar \"{tempJarPath}\" client -mcversion {version} -downloadMinecraft";

            var psi = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = args,

                RedirectStandardOutput = true,
                RedirectStandardError = true,  // <<< DEVE ESSERE TRUE
                UseShellExecute = false,       // <<< DEVE ESSERE FALSE
                CreateNoWindow = true
            };


            var proc = new Process();
            proc.StartInfo = psi;
            proc.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == null) return;

                this.Invoke(new MethodInvoker(() =>
                {
                    textBox1.Text = (e.Data + Environment.NewLine);
                    HandleFabricOutput(e.Data);
                }));
            };


            proc.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null) return;

                this.Invoke(new MethodInvoker(() =>
                {
                    textBox1.Text = ("[ERR] " + e.Data + Environment.NewLine);
                    HandleFabricOutput(e.Data);
                }));
            };


            proc.Start();
            proc.BeginOutputReadLine();

            await Task.Run(() => proc.WaitForExit());
            await Task.Delay(1000);
            if (proc.ExitCode == 0)
            {
                this.Invoke(new MethodInvoker(() => textBox1.Text = (" Installazione completata con successo." + Environment.NewLine)));
            }
            else
            {
                this.Invoke(new MethodInvoker(() => textBox1.Text = (" Errore durante l’installazione di Fabric. Codice: " + proc.ExitCode + Environment.NewLine)));
            }

            progressBar1.Value = 100; // done



        }

        private string ExtractJar()
        {
            string outputPath = Path.Combine(Path.GetTempPath(), "fabric-installer-1.0.3.jar");

            using (var stream = System.Reflection.Assembly.GetExecutingAssembly()
                       .GetManifestResourceStream("PALLWWW.fabric-installer-1.0.3.jar")) // ATTENZIONE a questo nome!
            using (var file = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(file);
            }

            return outputPath;

            foreach (var res in System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                Console.WriteLine(res);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (b == "1")
            {
                textBox1.Text = ("Allora chiudi quest'app e fallo");
                button1.Visible = false;

            }
            else if (b == "2")
            {
                progressBar1.Value = 0;
                textBox1.Text = ("Installare cartella mods?");
                a = "3";
                button3.Visible = false;
                button1.Visible = true;
                button2.Visible = true;

                b = "3";

            }
            else if (b == "3")
            {
                textBox1.Text = ("Allora perché hai aperto quest'app patacchio?");
                button1.Visible = false;
                return;
            }

            else if (b == "4")
            {
                button1.Visible = false;
                textBox1.Text = ("bene ciao");
            }
            else if (b == "5")
            {
                this.Close();
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (a == "1")
            {

                pictureBox1.Visible = true;
                progressBar1.Visible = true;
                textBox1.Text = ("Installare fabric?");
                a = "2";
                b = "2";
            }
            else if (a == "2")
            {
                await InstallFabricSilent();
                textBox1.Text = ("Installazione di Fabric completata!");
                progressBar1.Value = 0;
                button1.Visible = false;
                button2.Visible = false;
                button3.Visible = true;
                a = "3";
                b = "4";
            }
            else if (a == "3")
            {
                await Mods();



            }



        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        void MoveDirectory(string sourceDir, string destinationDir, bool move = true)
        {
            if (!Directory.Exists(destinationDir))
                Directory.CreateDirectory(destinationDir);

            // Copy all files
            foreach (string filePath in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(filePath));
                File.Move(filePath, destFile);
            }

            // Copy all subdirectories recursively
            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                MoveDirectory(subDir, destSubDir);
            }
        }

        async Task Mods()
        {
            this.Invoke(new Action(() =>
            {
                textBox1.Text = ("Ricerca cartella .Minecraft...");
                progressBar1.Value = 25;
                Thread.Sleep(100);
                progressBar1.Value = 50;
            }));

            string userName = Environment.UserName;
            string minecraftPath = $@"C:\Users\{userName}\AppData\Roaming\.minecraft";
            // Check both "Mods" and "mods"
            string[] modFolders = { "Mods", "mods" };
            await Task.Delay(500);
            this.Invoke(new Action(() =>
            {
                textBox1.Text = ("Controllo esistenza di cartella mods precedente...");
                progressBar1.Value = 75;
                Thread.Sleep(100);
                progressBar1.Value = 100;

            }));

            await Task.Delay(1000);
            progressBar1.Value = 0;
            string modpath = Path.Combine(minecraftPath, "mods");

            if (!Directory.Exists(modpath))
                Directory.CreateDirectory(modpath);
            else
            {
                foreach (string modFolderName in modFolders)
                {
                    string modPath = Path.Combine(minecraftPath, modFolderName);

                    if (Directory.Exists(modPath))
                    {
                        bool isEmpty = Directory.GetFiles(modPath).Length == 0 && Directory.GetDirectories(modPath).Length == 0;

                        if (!isEmpty)
                        {
                            this.Invoke(new Action(() =>
                            {
                                textBox1.Text = ($"{modFolderName} è piena, selezionare una cartella dove spostarla");
                            }));

                            await Task.Delay(1000);

                            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                            {
                                folderDialog.Description = "Seleziona una cartella dove spostarle";
                                folderDialog.ShowNewFolderButton = true;
                                folderDialog.RootFolder = Environment.SpecialFolder.Desktop;

                                DialogResult result = folderDialog.ShowDialog();

                                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        textBox1.Text = ("cartella selezionata: " + folderDialog.SelectedPath);
                                    }));

                                    string destinationPath = Path.Combine(folderDialog.SelectedPath, modFolderName);
                                    MoveDirectory(modPath, destinationPath);
                                    await Task.Delay(1000);
                                    this.Invoke(new Action(() =>
                                    {
                                        textBox1.Text = ($"la tua cartella {modFolderName} è stata spostata in" + folderDialog.SelectedPath);
                                    }));


                                }
                                else
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        textBox1.Text = ("nessuna cartella selezionata, procedere all'installazione (potrebbero esserci problemi)");
                                    }));

                                }
                            }
                            break; // stop checking after first found non-empty mod folder
                        }
                        else
                        {
                            textBox1.Text = ($" Trovata {modFolderName}, ma è vuota, procedere all'installazione...");
                        }
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            textBox1.Text = ("Nessuna cartella mods trovata, procedere all'installazione...");
                        }));
                    }


                }
            }
            Scompatto(minecraftPath);
            Thread.Sleep(5000);
            textBox1.Text = ("Fine!");
            button1.Visible = false;
            button2.Visible = true;
            button2.Text = ("chiudi");
            b = "5";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = ("Installare cartella mods?");
            button3.Visible = false;
            button1.Visible = true;
            button2.Visible = true;
            b = "4";
        }

        void Scompatto(string modpath)
        {
            string tempZipPath = Path.Combine(modpath, "temp.zip");

            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PALLWWW.mods.zip"))
            {
                if (resourceStream == null)
                {
                    Console.WriteLine("❌ Resource not found.");
                    return;
                }

                using (FileStream fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }

            ZipFile.ExtractToDirectory(tempZipPath, modpath);

            File.Delete(tempZipPath);

        }
    }
}

