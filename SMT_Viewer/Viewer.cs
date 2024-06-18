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
using CefSharp;
using CefSharp.WinForms;
using SMT_Viewer.Properties;

namespace SMT_Viewer
{
    public partial class Viewer : Form
    {
        private string value;
        public static string temp = "";
        private ChromiumWebBrowser _chrome = null;
        public Viewer()
        {
            InitializeComponent();

        }

        public string Passvalue
        {
            get { return this.value;  }
            set { this.value = value; }
        }


        private void Viewer_Load(object sender, EventArgs e)
        {
            temp = Passvalue;
            //MessageBox.Show(temp);
            InitializeCefSharp();
        }
        public CefSettings settings = new CefSettings();
        private void InitializeCefSharp()
        {
            //쿠키 데이터 사용하는 방법
            settings.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF";
            Cef.Initialize(settings);

            //웹 사이트 이동
            _chrome = new ChromiumWebBrowser(temp);
            //Main Form에 CefSharp컨트롤 추가
            this.Controls.Add(_chrome);
            //Main Form 전체 영역에 붙이기
            _chrome.Dock = DockStyle.Fill;
            //페이지 로딩 완료 이벤트
            _chrome.LoadingStateChanged += OnLoadingStateChanged;


        }

        private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            if (!args.IsLoading)
            {
                //MessageBox.Show("페이지 로딩 완료!");
            }
        }

        private void Viewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Restart(); //이상하게 두개가 재시작 되는 버그가있음
        }
    }
}
