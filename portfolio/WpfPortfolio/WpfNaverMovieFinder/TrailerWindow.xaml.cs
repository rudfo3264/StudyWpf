﻿using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfNaverMovieFinder.models;

namespace WpfNaverMovieFinder
{
    /// <summary>
    /// TrailerWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TrailerWindow : MetroWindow
    {
        List<YoutubeItem> youtubeItems; //Youtube API 검색결과 담을 리스트
        public TrailerWindow()
        {
            InitializeComponent();
        }
        public TrailerWindow(string movieName) : this()
        {
            lblMovieName.Content = $"{movieName} 예고편";  // 매트릭스:리저렉션
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            youtubeItems = new List<YoutubeItem>();     //초기화
            SearchYoutubeApi();
        }

        private async void SearchYoutubeApi()
        {
            await LoadDataCollection();
            IsvYoutubeSearch.ItemsSource = youtubeItems;
        }

        private async Task LoadDataCollection()
        {
            var youtubeService = new YouTubeService(
                new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyAoO4ZhG3ILaZKOV2lZ3NLWpJ6DiLD1C2w",
                    ApplicationName =this.GetType().ToString()
                });

            var request = youtubeService.Search.List("snippet");
            request.Q = lblMovieName.Content.ToString();
            request.MaxResults = 10;

            var response = await request.ExecuteAsync();

            //MessageBox.Show(response.ToString());

            foreach (var item in response.Items)
            {
                if (item.Id.Kind.Equals("youtube#video"))
                {
                    YoutubeItem youtube = new YoutubeItem(
                        item.Snippet.Title,
                        item.Snippet.ChannelTitle,
                        $"https://www.youtube.com/watch?v={item.Id.VideoId}");

                    //섬네일 이미지
                    youtube.Thumbnail = new BitmapImage(new Uri(item.Snippet.Thumbnails.Default__.Url,
                                                        UriKind.RelativeOrAbsolute));
                    youtubeItems.Add(youtube);
                }
            }
        }

        private void IsvYoutubeSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsvYoutubeSearch.SelectedItems.Count==0)
            {
                Commons.ShowMessageAsync("유튜브", "예고편을 볼 영화를 선택하세요");
            }
            if (IsvYoutubeSearch.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("유튜브", "예고편을 하나만 선택하세요");
            }
            if (IsvYoutubeSearch.SelectedItem is YoutubeItem)
            {
                var video = IsvYoutubeSearch.SelectedItem as YoutubeItem;
                BrsYoutubeWatch.Address = video.URL;
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BrsYoutubeWatch.Address = string.Empty;
            BrsYoutubeWatch.Dispose();  //리소스 해제
        }
    }
}
