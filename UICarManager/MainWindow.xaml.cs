using AppService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
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

namespace UICarManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<CarModel> models = new List<CarModel>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            models = await GetCars();

            dgCars.ItemsSource = models;
            
        }

        private void DgCars_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new CarModel
            {
                IsNew = true
            };
        }

        private void DgCars_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var carEditing = e.EditingElement.DataContext as CarModel;
            int id = e.Column.DisplayIndex;

            if (id == 1) 
            {
                RefWindow window = new RefWindow();
                window.GetData().Property = string.IsNullOrEmpty(carEditing.Image) ? "empty" : carEditing.Image;
                window.ShowDialog();
                var el = window.GetData();
                    
                if (string.IsNullOrEmpty(el.Error) && !string.IsNullOrEmpty(el.Property)) 
                {
                    carEditing.Image = el.Property;
                }
            }

            if (carEditing.Id > 0) 
            {
                Task.Run(async () => {
                    string anwers = await UpdateDBData(carEditing);
                });
                return;
            }
            //var model = dgCars.Items[dgCars.Items.Count - 2] as CarModel;
            
            //  MessageBox.Show(id.ToString() + "\n" + e.Row.GetIndex() + "\n" + dgCars.Items.Count);
            //if (e.Row.GetIndex() == dgCars.Items.Count - 2) 
            //{
             //}
        }

        private async Task<List<CarModel>> GetCars() 
        {
            return await Task.Run(() => {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        dgCars.CanUserAddRows = true;
                        dgCars.CellEditEnding += DgCars_CellEditEnding;
                        dgCars.AddingNewItem += DgCars_AddingNewItem;
                    });
                    List<CarModel> models_ = new List<CarModel>();
                    WebClient client = new WebClient();

                    models_ = JsonConvert.DeserializeObject<List<CarModel>>(client.DownloadString(Constants.GET));

                    return models_;
                }
                catch 
                {
                    return null;
                }
            });
        }

        private async void UpdateData_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => { 
                List<string> result = new List<string>();
                foreach (var item in this.dgCars.Items.SourceCollection) 
            {
                var CarModel = item as CarModel;
                if (CarModel.IsNew) 
                {
                    CarModel.IsNew = false;
                    WebRequest request = (HttpWebRequest)WebRequest.CreateHttp(Constants.ADD);

                    request.ContentType = "application/json";
                    request.Method = "POST";

                    using (StreamWriter sw = new StreamWriter(request.GetRequestStream())) 
                    {
                        sw.Write(JsonConvert.SerializeObject(new { 
                            Mark = string.IsNullOrEmpty(CarModel.Mark) ? "empty" : CarModel.Mark,
                            Model = string.IsNullOrEmpty(CarModel.Model) ? "empty" : CarModel.Model,
                            Image = string.IsNullOrEmpty(CarModel.Image) ? "empty" : CarModel.Image,
                            Age = CarModel.Age,
                            Capacity = CarModel.Capacity,
                            Fuel = string.IsNullOrEmpty(CarModel.Fuel) ? "empty" : CarModel.Fuel
                        }));
                    }

                    var response = (HttpWebResponse)request.GetResponse();

                    using (StreamReader sr = new StreamReader(response.GetResponseStream())) 
                    {
                        result.Add(sr.ReadToEnd());
                            CarModel.Id = int.Parse(result.Last().ToString());
                    }

                    }
            }
                      
                MessageBox.Show("Дані обновлено!");
                
            });

        }

        private Task<string> UpdateDBData(CarModel Car) 
        {
            return Task.Run(() => {

                string result = "";
                WebRequest request = (HttpWebRequest)WebRequest.CreateHttp(Constants.UPDATE);

                request.ContentType = "application/json";
                request.Method = "PUT";

                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(JsonConvert.SerializeObject(new
                    {
                        Id = Car.Id,
                        Mark = string.IsNullOrEmpty(Car.Mark) ? "empty" : Car.Mark,
                        Model = string.IsNullOrEmpty(Car.Model) ? "empty" : Car.Model,
                        Image = string.IsNullOrEmpty(Car.Image) ? "empty" : Car.Image,
                        Age = Car.Age,
                        Capacity = Car.Capacity,
                        Fuel = string.IsNullOrEmpty(Car.Fuel) ? "empty" : Car.Fuel
                    }));
                }

                var response = (HttpWebResponse)request.GetResponse();

                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
                return result;
            });
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => {
                Dispatcher.Invoke(() => { 
                    CarModel model = dgCars.SelectedItem as CarModel;
                    model.IsNew = false;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.CreateHttp(Constants.DELETE);
                    request.Method = "DELETE";
                    request.ContentType = "application/json";

                    using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                    {
                        sw.Write(JsonConvert.SerializeObject(model.Id));
                    }

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    using (StreamReader sr = new StreamReader(response.GetResponseStream())) 
                    {
                        var result = sr.ReadToEnd();
                    }

                    Dispatcher.Invoke(async () => {
                        List<CarModel> cars = new List<CarModel>();
                        cars.AddRange(models.Where(x => x.IsNew).ToList());
                        cars.AddRange(await GetCars());
                        models = cars;
                        this.dgCars.ItemsSource = cars;
                    });

                });
            });

        }

        private async void btnDoc_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => {
                Dispatcher.Invoke(() => { 
                    DocWindow window = new DocWindow();
                    window.ShowDialog();
                });
            });
            
        }
    }
}
