using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NetBpm.Test.Workflow.Example;

namespace TestForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HolidayTest holidayTest = new HolidayTest();
            holidayTest.SetContainer();

            holidayTest.TestHolidayProcessApproval();
        }
    }
}
