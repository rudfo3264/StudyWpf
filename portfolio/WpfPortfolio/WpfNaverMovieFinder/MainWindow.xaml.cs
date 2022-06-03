using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfNaverMovieFinder.models;


namespace WpfNaverMovieFinder
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// 검색버튼 클릭 이벤트 핸들러
    /// 네이버 OPENAPI 검색
    /// </summary>
    /// param
    /// param
    public partial class MainWindow : MetroWindow
    {
        bool IsFavorite = false; //네이버api로 검색한건지, 즐겨찾기DB에서 온것인지 확인할 값
        //IsFavorite = true -> DB에서 온 값 / IsFavorite = false -> 네이버 api

        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtSearchName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
       {
            if (e.Key == System.Windows.Input.Key.Enter) btnSearch_Click(sender, e);
        }

        private void btnSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            stsResult.Content = string.Empty;

            

            if (string.IsNullOrEmpty(txtSearchName.Text))
            {
                stsResult.Content = "검색할 영화명을 입력, 검색버튼을 눌러주세요.";
                //MessageBox.Show("검색할 영화명을 입력. 검색버튼을 눌러주세요");
                Commons.ShowMessageAsync("검색", "검색할 영화명을 입력, 검색버튼을 눌러주세요.");
                return; 
            }
            //검색시작
            //Commons.ShowMessageAsync("결과, ${txtSearchName.Text}");
            try
            {
                searchNaverOpenApi(txtSearchName.Text);
                Commons.ShowMessageAsync("검색", "영화검색 완료!");
            }
            catch (System.Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외 발생 : {ex}");
                IsFavorite = false;
            }
        }

        /// <summary>
        /// 네이버 실제 검색 메서드
        /// </summary>
        /// <param name="searchName"></param>
        private void searchNaverOpenApi(string searchName)
        {
            string clientID = "5pTgIN5bfCbodvdqWEgU";
            string clientSecret = "ot73vRkU8h";
            string openApiUri = $"https://openapi.naver.com/v1/search/movie?start=1&display=30&query={searchName}";
            string result = string.Empty; //빈값 초기화

            WebRequest request = null;
            StreamReader reader = null;
            Stream stream = null;
            WebResponse response = null;

            // Naver OpenAPI 실제 요청
            try
            {
                request = WebRequest.Create(openApiUri);
                request.Headers.Add("X-Naver-Client-id", clientID) ;    //중요
                request.Headers.Add("X-Naver-Client-Secret", clientSecret);  // 중요

                response = request.GetResponse();
                stream = response.GetResponseStream();
                reader = new StreamReader(stream);

                result = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                stream.Close();
                response.Close();
            }

            var parsedJson = JObject.Parse(result);

            int total = Convert.ToInt32(parsedJson["total"]);
            int display = Convert.ToInt32(parsedJson["display"]);

            stsResult.Content = $"{total} 중 {display} 호출 성공!";

            //데이터 그리드에 검색결과 할당
            var items = parsedJson["items"];
            var json_array = (JArray)items;

            List<MovieItem> movieItems = new List<MovieItem>();

            foreach (var item in json_array)
            {
                MovieItem movie = new MovieItem(
                    Regex.Replace(item["title"].ToString(), @"<(.|\n)*?>", string.Empty),
                    //item["title"].ToString(),                    
                    item["image"].ToString(),
                    item["link"].ToString(),
                    item["subtitle"].ToString(),
                    item["pubDate"].ToString(),
                    item["director"].ToString().Replace("|",","),
                    item["actor"].ToString(),
                    item["userRating"].ToString());
                movieItems.Add(movie);
            }
            this.DataContext = movieItems;
        }

        private void btnAddWatchList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (grdResult.SelectedItems.Count ==0)
            {
                Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 영화를 선택(복수선택 가능)");
                return;
            }
            if (IsFavorite==false)
            {
                Commons.ShowMessageAsync("오류", "이미 즐겨찾기 항목에 있습니다.");
                return;
            }

            List<tblFavoriteMovies> list = new List<tblFavoriteMovies>(); //FavoriteMovieItem(X)
            foreach (MovieItem item in grdResult.SelectedItems)
            {
                tblFavoriteMovies temp = new tblFavoriteMovies()
                {
                    Title = item.Title,
                    Link = item.Link,
                    image = item.Image,
                    SubTitle = item.SubTitle,
                    PubDate = item.PubDate,
                    Director = item.Director,
                    Actor = item.Actor,
                    UserRating = item.UserRating,
                    RegDate = DateTime.Now
                };

                list.Add(temp);
            }

            //EF 데이블에 데이터 입력(Insert)
            try
            {
                using (var ctx = new OpenApiLabEntities1())
                {
                    foreach (var item in list)
                    {
                        ctx.Set<tblFavoriteMovies>().Add(item);
                    }
                    ctx.SaveChanges(); //commit
                }
                Commons.ShowMessageAsync("저장", "즐겨찾기 추가 성공!!");
                IsFavorite = true;
            }
            catch (Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외 발생 : {ex}");
                IsFavorite = false;
            }
        }

        private void btnDeleteWatchList_Click(object sender, RoutedEventArgs e)
        {
            if (IsFavorite == false)
            {
                Commons.ShowMessageAsync("오류","즐겨찾기 내용이 아니면 삭제할 수 없습니다.");
                return;
            }
            if (grdResult.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("오류", "삭제할 영화를 선택하세요.");
                return;
            }
            foreach (tblFavoriteMovies item in grdResult.SelectedItems)
            {
                using (var ctx = new OpenApiLabEntities1())
                {
                    //삭제 처리
                    var delItem = ctx.tblFavoriteMovies.Find(item.Idx); // PK
                    ctx.Entry(delItem).State = System.Data.EntityState.Deleted; // 객체상태를 삭제상태 변경
                    ctx.SaveChanges(); // commit

                }
            }
            btnViewWatchList_Click(sender, e); // 즐겨찾기보기 버튼클릭 이벤트 실행
        }
        /// <summary>
        /// 유튜브 예고편
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWatchTrailer_Click(object sender, RoutedEventArgs e)
        {
            if (grdResult.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("유튜브영화", "영화를 선택하세요.");
                return;
            }
            if (grdResult.SelectedItems.Count > 1)
            {
                Commons.ShowMessageAsync("유튜브영화", "영화를 하나만 선택하세요.");
                return;
            }

            string movieName = "";  //string.Empty;
            if (IsFavorite ==true)
            {
                movieName = (grdResult.SelectedItem as tblFavoriteMovies).Title; 
            }
            else
            {
                movieName = (grdResult.SelectedItem as MovieItem).Title;     //한글 영화 제목
            }
            
            var trailerWindow = new TrailerWindow(movieName);   //영화제목 받는 생성자 변경!
            trailerWindow.Owner = this;     //MainWindow
            trailerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            trailerWindow.ShowDialog(); //모달창
        }

        /// <summary>
        /// 선택한 영화의 포스터 보이기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdResult_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            if (grdResult.SelectedItem is MovieItem) 
            {
                var movie = grdResult.SelectedItem as MovieItem; // 타입 변환
                if (string.IsNullOrEmpty(movie.Image)) // IsNullOrEmpty. 값이 있는지 없는지
                {
                    ImgPoster.Source = new BitmapImage(new Uri("/resource/No_Pictures.jpg",UriKind.RelativeOrAbsolute));
                }
                else
                {
                    ImgPoster.Source = new BitmapImage(new Uri(movie.Image, UriKind.RelativeOrAbsolute));
                }
            }
            if(grdResult.SelectedItem is tblFavoriteMovies) // 즐겨찾기
            {
                var movie = grdResult.SelectedItem as tblFavoriteMovies; // 타입 변환
                if (string.IsNullOrEmpty(movie.image)) // IsNullOrEmpty. 값이 있는지 없는지
                {
                    ImgPoster.Source = new BitmapImage(new Uri("/resource/No_Pictures.jpg", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    ImgPoster.Source = new BitmapImage(new Uri(movie.image, UriKind.RelativeOrAbsolute));
                }
            }
        }

        /// <summary>
        /// 네이버 영화 웹브라우저 열기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNaverMovie_Click(object sender, RoutedEventArgs e)
        {
            if (grdResult.SelectedItems.Count ==0)
            {
                Commons.ShowMessageAsync("네이버영화", "영화를 선택하세요.");
                return;
            }
            if (grdResult.SelectedItems.Count > 1)
            {
                Commons.ShowMessageAsync("네이버영화", "영화를 하나만 선택하세요.");
                return;
            }
            string linkUrl = (grdResult.SelectedItem as MovieItem).Link;
            if (IsFavorite==true)
            {
                linkUrl=(grdResult.SelectedItem as MovieItem).Link;
                
            }
            else
            {
                linkUrl = (grdResult.SelectedItem as tblFavoriteMovies).Link;
            }
            Process.Start(linkUrl);
        }

        private void btnViewWatchList_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            txtSearchName.Text = string.Empty;
            List<tblFavoriteMovies> list = new List<tblFavoriteMovies>();
            try
            {
                using (var ctx = new OpenApiLabEntities1())
                {
                    list = ctx.tblFavoriteMovies.ToList();
                }

                this.DataContext = list;
                stsResult.Content = $"즐겨찾기 {list.Count}개 조회";
                Commons.ShowMessageAsync("즐겨찾기", "즐겨찾기 조회 완료!");
                IsFavorite = true;
            }
            catch (Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외 발생 : {ex}");
                IsFavorite = false;
            }
        }
    }
}
