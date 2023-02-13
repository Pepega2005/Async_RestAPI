using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
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
using RestAPI_Library;

namespace RestAPI_Comics
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int maxNum = 0;
        int currentNum = 0;
        string imgPath = @"C:\Users\elite\Documents\GitHub\Async_RestAPI\RestAPI_Library\Images";

    public MainWindow()
        {
            InitializeComponent();

            APIHelper.Init();
        }

        private async Task LoadImage(int num = 0)
        {
            // API
            var comic = await WorkProcess.Load(num);

            currentNum = comic.Num;

            Info.Content = $"Title: {comic.Title}, Comic number: {currentNum}, Date: {comic.Day}/{comic.Month}/{comic.Year}";

            if (num == 0)
            {
                maxNum = comic.Num;
            }

            // comic.Img

            //imageComic.Source = new BitmapImage(new Uri(comic.Img));

            string fname = comic.Img.Split('/')[comic.Img.Split('/').Length - 1];

            DirectoryInfo d = new DirectoryInfo(imgPath);

            FileInfo[] Files = d.GetFiles("*.png");

            foreach (FileInfo file in Files)
            {
                if (file.Name == fname)
                {
                    imageComic.Source = new BitmapImage(new Uri($@"{imgPath}\{fname}"));
                    return;
                }
            }

            imageComic.Source = new BitmapImage(new Uri(comic.Img));

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(comic.Img), $@"{imgPath}\{fname}");
            }
        }

        private async void btnRandom_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();

            currentNum = random.Next(1, maxNum);

            await LoadImage(currentNum);

            if (currentNum == maxNum)
            {
                btnNext.IsEnabled = false;
            }
            else
            {
                btnNext.IsEnabled = true;
            }

            if (currentNum == 1)
            {
                btnPrev.IsEnabled = false;
            }
            else
            {
                btnPrev.IsEnabled = true;
            }
        }

        private async void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (currentNum > 1)
            {
                currentNum--;
                btnNext.IsEnabled = true;

                await LoadImage(currentNum);

                if (currentNum == 1)
                {
                    btnPrev.IsEnabled = false;
                }
            }
        }

        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (currentNum < maxNum)
            {
                currentNum++;

                btnPrev.IsEnabled = true;

                await LoadImage(currentNum);

                if (currentNum == maxNum)
                {
                    btnNext.IsEnabled = false;
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadImage();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(imgPath);
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {

                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            imageComic.Source = null;
        }
    }
}
