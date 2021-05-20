using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace 화면공유프로그램
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            x.IsEnabled = false;
            y.IsEnabled = false;
            z.IsEnabled = false;
            STO.IsEnabled = false;
        }

        public ImageStreamingServer _Server = new ImageStreamingServer();
        public static int YesNo = 0;
        public static int YN = 0;
        public static int Frame = 60;
        public static int Host = 0;

        public int yesno { get { return YesNo; } set { YesNo = value; } }

        public int frame { get { return Frame; } set { Frame = value; } }
        public int yn { get { return YN; } set { YN = value; } }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _Server.Stop();

            Process MyProc = Process.GetCurrentProcess();
            MyProc.Kill();

            return;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IPHostEntry IPHost = Dns.GetHostByName(Dns.GetHostName());
            StreamSV.Text = "http://" + IPHost.AddressList[0].ToString()+":8050";
            StreamSV.Foreground = System.Windows.Media.Brushes.Blue;
            Host = 1;
            x.IsEnabled = true;
            y.IsEnabled = true;
            z.IsEnabled = true;
            ST.IsEnabled = false;
            STO.IsEnabled = true;
            Button_Click_3(sender, e);
            //누를시 저,중,고 활성화 자동 저 사양 화면공유
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _Server.Stop();
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
        public ImageStreamingServer MSS = new ImageStreamingServer();

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            YesNo = 0;
            Frame = 200;
            MSS.FR(Frame);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            YesNo = 0;
            Frame = 60;
            MSS.FR(Frame);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            YesNo = 0;
            Frame = 1;
            MSS.FR(Frame);
        }

        private void x_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void StreamSV_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(Host == 1)
            {
                try
                {
                    Process.Start("chrome.exe", StreamSV.Text);
                }
                catch
                {
                    MessageBox.Show("크롬이 설치되어 있지 않습니다." + Environment.NewLine + "다운로드 페이지로 이동합니다.");
                    Process.Start("https://www.google.com/chrome/browser/desktop/index.html");
                }
            }
        }
    }

    public class ImageStreamingServer : IDisposable
    {
        private List<Socket> _Clients;
        private Thread _Thread;
        public static int fre = 200;
        public ImageStreamingServer() : this(Screen.Snapshots(1920, 1080, true))
        {

        }

        public ImageStreamingServer(IEnumerable<System.Drawing.Image> imagesSource)
        {
            _Clients = new List<Socket>();
            _Thread = null;


             this.ImagesSource = imagesSource;
            Interval = fre;
        }


        public void FR(int fr)
        {
            Stop();

            fre = fr;
            Interval = fre;
            //System.Windows.Forms.MessageBox.Show(Interval.ToString());
            //this.Interval = fre;

            //_Thread = null;
            MainWindow Mw = new MainWindow();
            if (Mw.yn == 1)
            {
                Mw.yesno = 1;
                Start(8050);
            }
            else
            {
                Start(8050);
                Mw.yn = 1;
            }
            
            

            //return;
        }


        public IEnumerable<System.Drawing.Image> ImagesSource { get; set; }
        public int Interval { get; set; }
        public IEnumerable<Socket> Clients { get { return _Clients; } }
        public bool IsRunning { get { return (_Thread != null && _Thread.IsAlive); } }
        public void Start(int port)
        {
            try
            {
                try
                {
                    MainWindow Mw = new MainWindow();

                    if (Mw.yesno == 0)
                    {
                        lock (this)
                        {
                            _Thread = new Thread(new ParameterizedThreadStart(ServerThread));
                            _Thread.IsBackground = true;
                            _Thread.Start(port);
                        }
                    }
                    else
                    {
                        _Thread.Resume();
                    }
                }
                catch
                {
                    _Thread.Resume();
                }
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public void Stop()
        {

            if (IsRunning)
            {
                try
                {
                    _Thread.Suspend();

                }
                finally
                {

                    lock (_Clients)
                    {

                        foreach (var s in _Clients)
                        {
                            try
                            {

                                s.Close();
                            }
                            catch { }
                        }
                        _Clients.Clear();

                    }

                    //_Thread = null;
                }
            }

        }

        private void ServerThread(object state)
        {

            try
            {
                Socket Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Server.Bind(new IPEndPoint(IPAddress.Any, (int)state));
                Server.Listen(10);

                System.Diagnostics.Debug.WriteLine(string.Format("Server started on port {0}.", state));

                foreach (Socket client in Server.IncommingConnectoins())
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), client);

            }
            catch { }

            this.Stop();
        }
        private void ClientThread(object client)
        {
            


            Socket socket = (Socket)client;

            System.Diagnostics.Debug.WriteLine(string.Format("New client from {0}", socket.RemoteEndPoint.ToString()));

            lock (_Clients)
                _Clients.Add(socket);

            try
            {
                using (MjpegWriter wr = new MjpegWriter(new NetworkStream(socket, true)))
                {

                    
                    wr.WriteHeader();

                    foreach (var imgStream in Screen.Streams(this.ImagesSource))
                    {
                        if(this.Interval > 0)

                            Thread.Sleep(this.Interval);


                        wr.Write(imgStream);
                    }

                }
            }
            catch { }
            finally
            {
                lock (_Clients)
                    _Clients.Remove(socket);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Stop();
        }

        #endregion
    }

    static class SocketExtensions
    {

        public static IEnumerable<Socket> IncommingConnectoins(this Socket server)
        {
            while (true)
                yield return server.Accept();
        }

    }

    static class Screen
    {
        public static IEnumerable<System.Drawing.Image> Snapshots()
        {
            return Screen.Snapshots(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height, true);
        }

        public static IEnumerable<System.Drawing.Image> Snapshots(int width, int height, bool showCursor)
        {
            System.Drawing.Size size = new System.Drawing.Size(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);

            Bitmap srcImage = new Bitmap(size.Width, size.Height);
            Graphics srcGraphics = Graphics.FromImage(srcImage);

            bool scaled = (width != size.Width || height != size.Height);

            Bitmap dstImage = srcImage;
            Graphics dstGraphics = srcGraphics;

            if (scaled)
            {
                dstImage = new Bitmap(width, height);
                dstGraphics = Graphics.FromImage(dstImage);
            }

            System.Drawing.Rectangle src = new System.Drawing.Rectangle(0, 0, size.Width, size.Height);
            System.Drawing.Rectangle dst = new System.Drawing.Rectangle(0, 0, width, height);
            System.Drawing.Size curSize = new System.Drawing.Size(32, 32);

            while (true)
            {
                srcGraphics.CopyFromScreen(0, 0, 0, 0, size);

                if (showCursor)
                    System.Windows.Forms.Cursors.Default.Draw(srcGraphics, new System.Drawing.Rectangle(System.Windows.Forms.Cursor.Position, curSize));

                if (scaled)
                    dstGraphics.DrawImage(srcImage, dst, src, GraphicsUnit.Pixel);

                yield return dstImage;

            }

            srcGraphics.Dispose();
            dstGraphics.Dispose();

            srcImage.Dispose();
            dstImage.Dispose();

            yield break;
        }

        internal static IEnumerable<MemoryStream> Streams(this IEnumerable<System.Drawing.Image> source)
        {
            MemoryStream ms = new MemoryStream();

            foreach (var img in source)
            {
                ms.SetLength(0);
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                yield return ms;
            }

            ms.Close();
            ms = null;

            yield break;
        }

    }

    public class MjpegWriter : IDisposable
    {

        private static byte[] CRLF = new byte[] { 13, 10 };
        private static byte[] EmptyLine = new byte[] { 13, 10, 13, 10 };

        private string _Boundary;

        public MjpegWriter(Stream stream)
            : this(stream, "--boundary")
        {

        }

        public MjpegWriter(Stream stream, string boundary)
        {

            this.Stream = stream;
            this.Boundary = boundary;
        }

        public string Boundary { get; private set; }
        public Stream Stream { get; private set; }

        public void WriteHeader()
        {

            Write(
                    "HTTP/1.1 200 OK\r\n" +
                    "Content-Type: multipart/x-mixed-replace; boundary=" +
                    this.Boundary +
                    "\r\n"
                 );

            this.Stream.Flush();
        }

        public void Write(System.Drawing.Image image)
        {
            MemoryStream ms = BytesOf(image);
            this.Write(ms);
        }

        public void Write(MemoryStream imageStream)
        {

            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine(this.Boundary);
            sb.AppendLine("Content-Type: image/jpeg");
            sb.AppendLine("Content-Length: " + imageStream.Length.ToString());
            sb.AppendLine();

            Write(sb.ToString());
            imageStream.WriteTo(this.Stream);
            Write("\r\n");

            this.Stream.Flush();

        }

        private void Write(byte[] data)
        {
            this.Stream.Write(data, 0, data.Length);
        }

        private void Write(string text)
        {
            byte[] data = BytesOf(text);
            this.Stream.Write(data, 0, data.Length);
        }

        private static byte[] BytesOf(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        private static MemoryStream BytesOf(System.Drawing.Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms;
        }

        public string ReadRequest(int length)
        {

            byte[] data = new byte[length];
            int count = this.Stream.Read(data, 0, data.Length);

            if (count != 0)
                return Encoding.ASCII.GetString(data, 0, count);

            return null;
        }

        #region IDisposable Members

        public void Dispose()
        {

            try
            {

                if (this.Stream != null)
                    this.Stream.Dispose();

            }
            finally
            {
                this.Stream = null;
            }
        }

        #endregion
    }


}

