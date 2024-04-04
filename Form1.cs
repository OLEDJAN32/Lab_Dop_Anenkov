using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Xml.Linq;
using System.Timers;

namespace Lab_Dop_Anenkov
{
    public partial class Form1 : Form
    {
        private System.Timers.Timer timer;
        public Form1()
        {
            InitializeComponent();
            Database();
            timer = new System.Timers.Timer();
            timer.Interval = 3000;
            timer.Elapsed += checkComplete;
        }

        private void checkComplete(object sender, ElapsedEventArgs e)
        {
            DateTime currentDateAndTime = DateTime.Now;
            DateTime dateToCompare;
            DateTime timeToCompare;
            DataGridViewCell cellData;
            DataGridViewCell cellTime;
            string inputDate, inputTime;

            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (this.dataGridView1.Rows[i].Cells[3].Value == "Выполнено") continue;
                cellData = dataGridView1.Rows[i].Cells[1];
                cellTime = dataGridView1.Rows[i].Cells[2];
                inputDate = cellData.Value.ToString();
                inputTime = cellTime.Value.ToString();
                dateToCompare = DateTime.ParseExact(inputDate, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                timeToCompare = DateTime.ParseExact(inputTime, "H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                if (dateToCompare.Date > currentDateAndTime.Date)
                {
                    this.dataGridView1.Rows[i].Cells[3].Value = "Выполняется";
                    return;
                }
                else
                {
                    if (dateToCompare.Date == currentDateAndTime.Date && timeToCompare.TimeOfDay >= currentDateAndTime.TimeOfDay)
                    {
                        this.dataGridView1.Rows[i].Cells[3].Value = "Выполняется";
                        return;
                    }
                    else
                    {
                        this.dataGridView1.Rows[i].Cells[3].Value = "Просрочено";
                    }
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void Database()
        {
            using (StreamReader reader = new StreamReader("data.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] words = line.Split(' ');
                    words[0] = words[0].Replace("_", " ");
                    Add(words[0], words[1], words[2], words[3]);
                }
                reader.Close();
            }
        }

        private void Add(string node, string date, string time, string complete)
        {
            var index = this.dataGridView1.Rows.Add();
            this.dataGridView1.Rows[index].Cells[0].Value = node;
            this.dataGridView1.Rows[index].Cells[1].Value = date;
            this.dataGridView1.Rows[index].Cells[2].Value = time;
            this.dataGridView1.Rows[index].Cells[3].Value = complete;
            if(complete!="Выполнено")
            {
                checktime(index);
            }
        }
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBox1.Text=="")
            {
                return;
            }

            string textToAdd = textBox1.Text;
            var index = this.dataGridView1.Rows.Add();
            this.dataGridView1.Rows[index].Cells[0].Value=textToAdd;

            string textDate = dateTimePicker1.Text;
            string textTime = dateTimePicker2.Text;
            this.dataGridView1.Rows[index].Cells[1].Value = textDate;
            this.dataGridView1.Rows[index].Cells[2].Value = textTime;
            checktime(index);
            SaveDataToFile();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if(dataGridView1.Rows.Count>1)
            {
                int delet = dataGridView1.SelectedCells[0].RowIndex;
                dataGridView1.Rows.RemoveAt(delet);
            }
            SaveDataToFile();
        }

        private void checktime(int index)
        {
            DateTime currentDateAndTime = DateTime.Now;
            DateTime dateToCompare;
            DateTime timeToCompare;
            string inputDate, inputTime;

            DataGridViewCell cellData = dataGridView1.Rows[index].Cells[1];
            DataGridViewCell cellTime = dataGridView1.Rows[index].Cells[2];
            inputDate = cellData.Value.ToString();
            inputTime = cellTime.Value.ToString();
            dateToCompare = DateTime.ParseExact(inputDate, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
            timeToCompare = DateTime.ParseExact(inputTime, "H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            if (dateToCompare.Date > currentDateAndTime.Date)
            {
                this.dataGridView1.Rows[index].Cells[3].Value = "Выполняется";
                return;
            }
            else
            {
                if (dateToCompare.Date == currentDateAndTime.Date && timeToCompare.TimeOfDay >= currentDateAndTime.TimeOfDay)
                {
                    this.dataGridView1.Rows[index].Cells[3].Value = "Выполняется";
                    return;
                }
                else
                {
                    this.dataGridView1.Rows[index].Cells[3].Value = "Просрочено";
                }
            }
        }

        private void buttonCompleted_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.SelectedCells[0].RowIndex;
            if(this.dataGridView1.Rows[index].Cells[1].Value!=null)
            {
                this.dataGridView1.Rows[index].Cells[3].Value = "Выполнено";
            }
            SaveDataToFile();
        }

        private void SaveDataToFile()
        {
            DataGridViewCell cellNode, cellDate, cellTime, cellComplete;
            string node, date, time, complete;
            StreamWriter writer = new StreamWriter("data.txt");
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                cellNode = dataGridView1.Rows[i].Cells[0];
                cellDate = dataGridView1.Rows[i].Cells[1];
                cellTime = dataGridView1.Rows[i].Cells[2];
                cellComplete = dataGridView1.Rows[i].Cells[3];

                node = cellNode.Value.ToString();
                date = cellDate.Value.ToString();
                time = cellTime.Value.ToString();
                complete = cellComplete.Value.ToString();
                node = node.Replace(" ", "_");

                writer.Write(node + " ");
                writer.Write(date + " ");
                writer.Write(time + " ");
                writer.Write(complete + "\n");
            }
            writer.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
            timer.Dispose();
        }
    }
}