using System;
using Newtonsoft.Json.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Threading;

namespace Meteo
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string base_api_weather = "https://goweather.herokuapp.com/weather/";
        public string base_api_news = "https://newsapi.org/v2/everything?apiKey=e5abe5c137e44c38947ea1dd7469b91f&language=fr&pageSize=6&q=";
        public string jsonString;
        public string jsonStringNews;
        public string city_default = "Lubumbashi";
        public MainWindow()
        {
            InitializeComponent();
            Thread thread_news = new Thread(new ThreadStart(GetNews));
            thread_news.Start();
            Thread thread_weather = new Thread(new ThreadStart(GetApiResponse));
            thread_weather.Start();
           
            init_date();
            //GetApiResponse();
            
        }

        public void init_date()
        {   
            date.Text = DateTime.Now.ToString("dddd dd MMMM yyyy").ToUpper();
            day1.Content = DateTime.Now.AddDays(1).ToString("ddd dd").ToUpper();
            day2.Content = DateTime.Now.AddDays(2).ToString("ddd dd").ToUpper();
            day3.Content = DateTime.Now.AddDays(3).ToString("ddd dd").ToUpper();
        }

        public void GetApiResponse()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {

            using (WebClient wc = new WebClient())
            {
                // Telechargement du JSON depuis l'api
                try
                { 

                    if (string.IsNullOrEmpty(city_to_search.Text))
                    {
                        jsonString = wc.DownloadString(base_api_weather + city_default);

                    }
                    else
                    {
                        jsonString = wc.DownloadString(base_api_weather + city_to_search.Text);

                    }
                    byte []bytes = Encoding.Default.GetBytes(jsonString);
                    jsonString = Encoding.UTF8.GetString(bytes);
                    SetInfo();
                  
                }
                catch (Exception ex)
                {

                }
                
            }

            }));

        }

        public void SetInfo()
        {
            try
            {
                JObject jsobj = JObject.Parse(jsonString);
                temperature.Text = jsobj["temperature"].ToString();
                description.Text = jsobj["description"].ToString();
                temperature1.Text = jsobj["forecast"][0]["temperature"].ToString();
                temperature2.Text = jsobj["forecast"][1]["temperature"].ToString();
                temperature3.Text = jsobj["forecast"][2]["temperature"].ToString();

                if (jsobj["description"].ToString().Contains("Rain"))
                {
                    header_img.Source = new BitmapImage(new Uri(@"/rain.png", UriKind.Relative));

                }
                else if (jsobj["description"].ToString().Contains("Sunny"))
                {
                    header_img.Source = new BitmapImage(new Uri(@"/sun1.png", UriKind.Relative));
                }

                else if (jsobj["description"].ToString().Contains("cloudy"))
                {
                    header_img.Source = new BitmapImage(new Uri(@"/clouds.png", UriKind.Relative));
                }


            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }


        }

        public void GetNews()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {

                using (WebClient wc = new WebClient())
            {
                // Telechargement du JSON depuis l'api
                try
                {

                    if (string.IsNullOrEmpty(city_to_search.Text))
                    {
                        jsonStringNews = wc.DownloadString(base_api_news + city_default);
                        MessageBox.Show(jsonStringNews);
                    }
                    else
                    {
                        jsonStringNews = wc.DownloadString(base_api_news + city_to_search.Text);
                        MessageBox.Show(jsonStringNews);
                    }
                    byte[] bytes = Encoding.Default.GetBytes(jsonStringNews);
                    jsonStringNews = Encoding.UTF8.GetString(bytes);
                    

                }
                catch (Exception ex)
                {

                }


            }
                SetNews();
            }));

        }

        public void SetNews()
        {
            try
            {

                JObject jsonobj = JObject.Parse(jsonStringNews);
                news.Text = "";

                for (int i = 0; i <= 6; i++)
                {
                    news.Text += jsonobj["articles"][i]["description"].ToString();
                }
            }catch(Exception ex)
            {
                
            }
        }
            
        private void SearchCity(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(city_to_search.Text))
            {
                MessageBox.Show("Veuillez remplir ce champ", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                city_name.Text = city_to_search.Text;
                GetApiResponse();
                SetInfo();
                GetNews();
                SetNews();
            }
            
        }

        
    }
}
