using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BulkPhotoRenamer
{
    public partial class GUI : Form
    {
        ImageManager imageManager;
        string selectedPath;

        public GUI()
        {
            InitializeComponent();

            imageManager = new ImageManager();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selectFiles();
        }

        private void selectFiles()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    selectedPath = fbd.SelectedPath;

                    string[] extensions = { "bmp", "jpg", "jpeg", "gif", "png" };

                    string[] filePaths = Directory.GetFiles(fbd.SelectedPath, "*.*")
                        .Where(f => extensions.Contains(f.Split('.').Last().ToLower())).ToArray();

                    // var filePaths = Directory.GetFiles(fbd.SelectedPath);

                    // Remove everything in our record
                    imageManager.clearAll();

                    // Iterate through every file path in the array
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        // Initialize variables
                        bool addTimestamp = false;
                        string filePath, prefix = null;

                        // Get the current file path
                        filePath = filePaths[i];

                        // Check if a prefix is given
                        if (checkBox1.Checked == true && textBox1.Text.All(char.IsLetterOrDigit))
                        {
                            prefix = textBox1.Text;
                        }

                        // Check if the add timestamp option has been selected
                        if (checkBox2.Checked == true)
                        {
                            addTimestamp = true;
                        }

                        imageManager.add(new PhotoInfo(filePath, prefix, addTimestamp));
                    }

                    drawListViewItems();

                    checkBox1.Enabled = false;
                    checkBox2.Enabled = false;
                    toolStripStatusLabel1.Text = "Selected directory: " + selectedPath;
                }
            }
        }

        private  void drawListViewItems()
        {
            listView1.Items.Clear();

            foreach (PhotoInfo i in imageManager.images)
            {
                listView1.Items.Add(i.originalFileName);
                listView1.Items[listView1.Items.Count - 1].SubItems.Add(i.dateTime.ToString());
                listView1.Items[listView1.Items.Count - 1].SubItems.Add("");
                listView1.Items[listView1.Items.Count - 1].SubItems.Add(i.newFileName);
                listView1.Items[listView1.Items.Count - 1].SubItems.Add(i.filePath);
            }

            for (var i = 0; i < listView1.Columns.Count; i++)
            {
                listView1.Columns[i].Width = -2;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                button1.Enabled = false;
                textBox1.Enabled = true;
            }
            else if (checkBox1.Checked == false)
            {
                button1.Enabled = true;
                textBox1.Enabled = false;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!textBox1.Text.All(char.IsLetterOrDigit))
            {
                errorProvider1.SetError(textBox1, "Invalid prefix");
                button1.Enabled = false;
            }
            else
            {
                errorProvider1.SetError(textBox1, "");
                button1.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            imageManager.renameAll();

            imageManager.clearAll();
            drawListViewItems();

            checkBox1.Enabled = true;
            checkBox2.Enabled = true;

            toolStripStatusLabel1.Text = "Selected directory: none ";

            MessageBox.Show("All photos have been renamed successfully!");
        }

        private void toolStripStatusLabel1_TextChanged(object sender, EventArgs e)
        {
            if (selectedPath != null)
            {
                if (listView1.Items.Count > 0)
                {
                    button2.Enabled = true;
                } else
                {
                    button2.Enabled = false;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            imageManager.clearAll();
            drawListViewItems();

            checkBox1.Enabled = true;
            checkBox2.Enabled = true;

            toolStripStatusLabel1.Text = "Selected directory: none ";

            MessageBox.Show("Reset successful!");
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(checkBox2, "test");
        }
    }
}
