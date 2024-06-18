using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMT_Viewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void connect_btn_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(ip_txt.Text))
            {
                MessageBox.Show("ip정보가 입력되지 않았습니다.");
            }
            else
            {
                Viewer view = new Viewer();
                view.Passvalue = ip_txt.Text;

                view.Show();

                this.Hide();
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //재시작시 두번실행 버그 시작시 같은 이름 프로세스 있으면 하나 제거
            Process[] pri = Process.GetProcessesByName("SMT_Viewer");

            if(pri.Length == 2)
            {
                pri[0].Kill();
            }
        }
    }
}
