﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfNaverMovieFinder.models
{
    internal class MovieItem
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
        public string SubTitle { get; set; }
        public string PubDate {get; set;}
        public string Director { get; set; }
        public string Actor { get; set; }
        public string UserRating { get; set; }

        public MovieItem(string title, string image, string link, string subTitle, string pubDate, string director, string actor, string userRating)
        {
            Title = title;
            Image = image;
            Link = link;
            SubTitle = subTitle;
            PubDate = pubDate;
            Director = director;
            Actor = actor;
            UserRating = userRating;
        }
    }
}
