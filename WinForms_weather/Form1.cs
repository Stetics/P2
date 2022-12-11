using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Net.Http;
using System.Globalization;

namespace WinForms_weather
{
    public partial class Form1 : Form
    {   
        Cities cit = new Cities();

        class Cities
        {
            public List<string> C_Names = new List<string>(200);
            public List<string> C_coord_x = new List<string>(200);
            public List<string> C_coord_y = new List<string>(200);

            public Cities()
            {
                List<string> C_Names = new List<string>(200);
                List<string> C_coord_x = new List<string>(200);
                List<string> C_coord_y = new List<string>(200);
            }

            public void Reading()
            {
                var rows = File.ReadAllLines("city.txt");
                foreach(var row in rows)
                {
                    C_Names.Add(row.Split('\t')[0]);
                    C_coord_x.Add(row.Split('\t')[1].Split(',')[0]);
                    C_coord_y.Add(row.Split('\t')[1].Split(',')[1]);

                }
            }
        }

        class Weather
        {
            public string Country;
            public string Name;
            public double Temp;
            public string Description;
            public Weather(string _country, string _name, double _temp, string _desc)
            {
                Country = _country;
                Name = _name;
                Temp = _temp;
                Description = _desc;
            }

            public string Text()
            {
                string t = ("----------------------\n" +
                    $"Country: {this.Country}\n" +
                    $"Name: {this.Name}\n" +
                    $"Temp: {this.Temp}\n" +
                    $"Description: {this.Description}\n");
                return t;
            }
        }


        public Form1()
        {
            InitializeComponent();
            cit.Reading();
            foreach(var elem in cit.C_Names)
            {
                LIST_SHOW.Items.Add(elem);
            }

            button1.Click += button1_Click;
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            //MessageBox.Show("nice");
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            cit.Reading();
            string str = LIST_SHOW.Text;
            int index = cit.C_Names.FindIndex(x => x == str);
            double lat = double.Parse(cit.C_coord_x[index]);
            double lon = double.Parse(cit.C_coord_y[index]);
            string api = "0dc6b1b1624705d01cec0d6dac131320";
            await Task.Run(async () =>
            {
                try
                {
                    var client = new HttpClient();
                    var content = await client.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={api}");
                    JsonElement jsonObject = JsonSerializer.Deserialize<JsonElement>(content);
                    Weather w = new Weather(jsonObject.GetProperty("sys").GetProperty("country").GetString(), jsonObject.GetProperty("name").GetString(), jsonObject.GetProperty("main").GetProperty("temp").GetDouble(), jsonObject.GetProperty("weather")[0].GetProperty("description").GetString());
                    MessageBox.Show(w.Text());
                }
                catch
                { MessageBox.Show("Error while downloading"); }
            });
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(LIST_SHOW.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}