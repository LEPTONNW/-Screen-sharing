# -Screen-sharing
화면공유 프로그램 입니다

이번에는 예쁘게 만들고 싶어서 기본윈폼이아니라 WPF로 만들었습니다

원리는 스크린을 JPEG로 연속적으로 찍어서 그걸 http로 무한하게 전송하게 만들어서 움직이는 영상처럼 보이게 만들었습니다.
그래서 인터넷익스플로러 같은걸로 보면 이미지가 계속해서 다운로드 되니 화면공유를 보는 클라이언트 쪽에서는 반드시 Chrome 으로 봐주시길 바랍니다.



# 화면공유 포트변경방법

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
        
 기본포트는 8050으로 되어있습니다 "StreamSV.Text = "http://" + IPHost.AddressList[0].ToString()+":8050";" 부분을 수정하시면 됩니다.


# SMT_Viwer 
SMT Viwer 라는 이름의 뷰어가 추가 되었습니다. 화면공유중인 PC의 IP주소를 입력한 후 연결 버튼을 누르면
최상위 화면에 고정되는 Chrome 기반의 뷰어가 열립니다.
