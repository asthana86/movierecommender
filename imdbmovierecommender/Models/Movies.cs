using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace imdbmovierecommender.Models
{
    public class Movies
    {
        public static int moviestorecommend = 6; 

        public List<MovieList> MoviesList { get; set; }
        public class MovieList
        {
            public int MovieID { get; set; }
            public string MovieName { get; set; }
        }

        public Movies()
        {
            MoviesList = new List<MovieList>();
            var fileReader = File.OpenRead(HttpContext.Current.Server.MapPath("/Content/IMDB Movie Titles.csv"));
            var reader = new StreamReader(fileReader);
            bool header = true;
            int index = 0;
            var line = "";
            while (!reader.EndOfStream)
            {
                if (header)
                {
                    line = reader.ReadLine();
                    header = false;
                }
                line = reader.ReadLine();
                string[] fields = line.Split(',');
                int MovieID = Int32.Parse(fields[0].ToString().TrimStart(new char[] { '0' }));
                string MovieName = fields[1].ToString();
                MoviesList.Add(new MovieList() {MovieID=MovieID, MovieName=MovieName});
                index++;
            }
        }
    }
}