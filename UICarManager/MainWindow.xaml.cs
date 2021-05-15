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
        //  Колекція, яка містить інформацію про усі автомобілі з БД
        List<CarModel> models = new List<CarModel>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                dgCars.CanUserAddRows = true;
                dgCars.CellEditEnding += DgCars_CellEditEnding;
                dgCars.AddingNewItem += DgCars_AddingNewItem;
            });
            //  Ініціалізація колекції автомобілів і присвоєння цих автомобілів до колекції models асинхронно
            models = await GetCars();
            //  Формування ДагаГріда
            dgCars.ItemsSource = models;
        }

        private void DgCars_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            //  Присвоєння новому елементу ДатаГріда "прапорця"
            e.NewItem = new CarModel
            {
                IsNew = true
            };
        }

        private void DgCars_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //  Діставання елемента, який редагуєтсья
            var carEditing = e.EditingElement.DataContext as CarModel;
            //  індекс редагуємого елемента
            int id = e.Column.DisplayIndex;

            //  Перевірка чи колонка - це колонка з фотографією
            if (id == 1) 
            {
                //  Відкриття вікна у якому задається посилання
                RefWindow window = new RefWindow();
                //  Присвоєння початкового значення поля діалогового вікна
                window.GetData().Property = string.IsNullOrEmpty(carEditing.Image) ? "empty" : carEditing.Image;
                //  Відкривання діалогового вікна
                window.ShowDialog();
                //  Повернення значення із діалогового вікна, а саме посилання
                var el = window.GetData();
                //  Перевірка чи посилання не містить помилок та чи воно не пусте
                if (string.IsNullOrEmpty(el.Error) && !string.IsNullOrEmpty(el.Property)) 
                {
                    //  Встановлення посилання на зображення для елемента ДатаГріда
                    carEditing.Image = el.Property;
                }
            }

            //  Перевірка чи елемент не новостворений, а уже існучий в БД
            if (carEditing.Id > 0) 
            {
                //  Запуск оновлення данних, який оновлює дані у БД
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
            //  Отримання данних з БД
            return await Task.Run(() => {
                try
                {
                    
                    //  Формування нової колекції яка повертатиметься
                    List<CarModel> models_ = new List<CarModel>();
                    //  Створення обєкта WebClient, який отримує запити з веб-сервісу
                    WebClient client = new WebClient();
                    //  Десеріалізація з формата json у тип List<CarModel>
                    models_ = JsonConvert.DeserializeObject<List<CarModel>>(client.DownloadString(Constants.GET));
                    //  Повернення колекції
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
                //  Формування колекції, яка буде містити дані про результат доданих елементів
                List<string> result = new List<string>();
                foreach (var item in this.dgCars.Items.SourceCollection) 
            {
                    //  Ініціалізація обєкта а саме приведення до типу
                    var CarModel = item as CarModel;
                    //  Перевірка чи елемент новий
                    if (CarModel.IsNew) 
                {
                        //  Відключення прапорця
                        CarModel.IsNew = false;
                        //  Ініціалізація обєкта, який відправляє запити на веб-сервіс
                        WebRequest request = (HttpWebRequest)WebRequest.CreateHttp(Constants.ADD);
                        //  Встановлення формату відправки данних
                        request.ContentType = "application/json";
                        //  Встановлення типу відправки данних
                        request.Method = "POST";

                        //  Формування потоку, який відправляє запит до веб-сервісу
                        using (StreamWriter sw = new StreamWriter(request.GetRequestStream())) 
                        {
                                //  Відправка данних на веб-сервіс
                                sw.Write(JsonConvert.SerializeObject(new { 
                                Mark = string.IsNullOrEmpty(CarModel.Mark) ? "empty" : CarModel.Mark,
                                Model = string.IsNullOrEmpty(CarModel.Model) ? "empty" : CarModel.Model,
                                Image = string.IsNullOrEmpty(CarModel.Image) ? "empty" : CarModel.Image,
                                Age = CarModel.Age,
                                Capacity = CarModel.Capacity,
                                Fuel = string.IsNullOrEmpty(CarModel.Fuel) ? "empty" : CarModel.Fuel
                            }));
                        }

                        //  Отримання результату роботи запиту
                        var response = (HttpWebResponse)request.GetResponse();
                        //  Зчитування результату роботи запиту
                        using (StreamReader sr = new StreamReader(response.GetResponseStream())) 
                        {
                        result.Add(sr.ReadToEnd());
                            //  Присвоєння новому елементу виданого з БД ідентифікатора
                            CarModel.Id = int.Parse(result.Last().ToString());
                        }

                    }
            }
                //  Виведення повідомлення про успішне оновлення данних 
                MessageBox.Show("Дані обновлено!");
                
            });

        }

        private Task<string> UpdateDBData(CarModel Car) 
        {
            return Task.Run(() => {
                //  Ініціалізація строки результату
                string result = "";
                //  Ініціалізація обєкту, який відправляє запити на веб-сервіс
                WebRequest request = (HttpWebRequest)WebRequest.CreateHttp(Constants.UPDATE);
                //  Встановлення типу відправки данних
                request.ContentType = "application/json";
                //  Встановлення способу відправки данних  
                request.Method = "PUT";
                //  Ініціалізація обєкту, який відправляє запит до веб-сервісу
                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    //  Відправка запиту на веб-сервіс
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
                //  Отримання відповіді на запит
                var response = (HttpWebResponse)request.GetResponse();
                //  Зчитування результату відправки запиту
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
                    //  Витягування активного елемента ДатаГріда
                    CarModel model = dgCars.SelectedItem as CarModel;
                    //  Відключення прапорця
                    model.IsNew = false;
                    //  Ініціалізація обєкта, який відправлятиме запит на веб-сервіс
                    HttpWebRequest request = (HttpWebRequest)WebRequest.CreateHttp(Constants.DELETE);
                    //  Встановлення методу відправки данних
                    request.Method = "DELETE";
                    //  Встановлення типу відправки данних
                    request.ContentType = "application/json";
                    //  Ініціалізація обєкту, який відправляє дані на веб-сервіс
                    using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                    {
                        //  Відправка данних
                        sw.Write(JsonConvert.SerializeObject(model.Id));
                    }
                    //  Отримання результату запиту
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    //  Зчитування результату запиту
                    using (StreamReader sr = new StreamReader(response.GetResponseStream())) 
                    {
                        var result = sr.ReadToEnd();
                    }

                    //  Переініціалізація ДатаГріда
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
                    //  Відкриття діалогового вікна з документацією
                    DocWindow window = new DocWindow();
                    window.ShowDialog();
                });
            });
            
        }
    }
}
