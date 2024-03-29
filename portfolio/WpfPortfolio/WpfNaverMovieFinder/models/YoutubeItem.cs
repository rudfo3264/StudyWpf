﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfNaverMovieFinder.models
{
    class YoutubeItem
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string URL { get; set; }
        public BitmapImage Thumbnail { get; set; }

        public YoutubeItem(string title, string author, string uRL)// BitmapImage thumbnail)
        {
            Title = title;
            Author = author;
            URL = uRL;
            //Thumbnail = thumbnail;
        }
    }
}
