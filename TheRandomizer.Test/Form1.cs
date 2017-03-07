using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheRandomizer.Generators.Assignment;

namespace TheRandomizer.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var generator = new AssignmentGenerator();
            generator.Items = new List<LineItem>();
            generator.Items.Add(new LineItem());
            generator.Items[0].Name = "Start";
            generator.Items[0].Expression = "Hello [World[=s]]!";
            generator.Items.Add(new LineItem());
            generator.Items[1].Name = "Start";
            generator.Items[1].Expression = "Goodbye [World[=s]]! [=1+1]";
            generator.Items.Add(new LineItem());
            generator.Items[2].Name = "World";
            generator.Items[2].Expression = "Earth";
            generator.Items.Add(new LineItem());
            generator.Items[3].Name = "World";
            generator.Items[3].Expression = "Mars";
            generator.Items.Add(new LineItem());
            generator.Items[4].Name = "s";
            generator.Items[4].Expression = "s";
            generator.Items.Add(new LineItem());
            generator.Items[5].Name = "s";
            generator.Items[5].Expression = "";
            generator.Items.Add(new LineItem());
            generator.Items[6].Name = "Worlds";
            generator.Items[6].Expression = "Earth and Mars";
            //textBox1.Text = generator.Generate(1, null, null)[0];
            textBox1.Text = generator.Serialize();
        }
    }
}
