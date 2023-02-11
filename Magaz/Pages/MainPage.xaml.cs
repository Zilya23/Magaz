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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Magaz.DataBase;

namespace Magaz.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public List<Product> Products { get; set; }
        public List<ProductType> Types { get; set; }
        public List<string> Filters { get; set; }

        public int countProduct = 0;
        public int page = 0;
        public MainPage()
        {
            InitializeComponent();
            DataContext = this;

            Types = BDConnection.connection.ProductType.ToList();

            Types.Insert(0, new ProductType() 
            { 
                Id = 0, Name = "Все" 
            });
            cbType.ItemsSource = Types;
            cbType.DisplayMemberPath = "Name";

            Filters = new List<string>()
            {
                "Все",
                "По возрастанию цены",
                "По убыванию цены",
                "От А до Я",
                "От Я до А"
            };
            cbFiltr.ItemsSource = Filters;

            Filter();
        }

        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void cbFiltr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void Filter()
        {
            Products = BDConnection.connection.Product.ToList();

            if (cbType.SelectedItem != null && (cbType.SelectedItem as ProductType).Id != 0)
            {
                var types = (cbType.SelectedItem as ProductType).Id;
                Products = Products.Where(x => x.ProductType.Id == types).ToList();
            }
            else if (cbType.SelectedItem != null && (cbType.SelectedItem as ProductType).Id == 0)
            {
                Products = BDConnection.connection.Product.ToList();
            }

            if (cbFiltr.SelectedIndex == 1)
            {
                Products = Products.OrderBy(x => x.MinPrice).ToList();
            }
            else if (cbFiltr.SelectedIndex == 2)
            {
                Products = Products.OrderByDescending(x => x.MinPrice).ToList();
            }
            else if (cbFiltr.SelectedIndex == 3)
            {
                Products = Products.OrderBy(x => x.Name).ToList();
            }
            else if (cbFiltr.SelectedIndex == 4)
            {
                Products = Products.OrderByDescending(x => x.Name).ToList();
            }
            else if (cbFiltr.SelectedIndex == 0)
            {
                Products = Products.OrderBy(x => x.Id).ToList();
            }

            if (tbSearch.Text.Trim().Length != 0)
            {
                tbPage.Text = $"{(20 * page)}/{Products.Count}";
                Products = Products.Where(x => x.Name.Contains(tbSearch.Text)).Skip(20 * page).Take(20).ToList();
            }
            else
            {
                tbPage.Text = $"{(20 * page)}/{Products.Count}";
                Products = Products.Skip(20 * page).Take(20).ToList();
            }

            
            lvProd.ItemsSource = Products;
        }

        private void tbSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Filter();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if(Products.Count != 0)
            {
                if(page < (int)Math.Round((double)(BDConnection.connection.Product).Count() / 20) - 1)
                {
                    page++;
                }
            }
            Filter();
        }

        private void btnLeave_Click(object sender, RoutedEventArgs e)
        {
            if(page > 0)
            {
                page--;
            }
            Filter();
        }
    }
}
