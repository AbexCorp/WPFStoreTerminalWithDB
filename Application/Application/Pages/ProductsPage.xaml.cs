﻿using System;
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
using Microsoft.EntityFrameworkCore;
using StoreApp.DataAccess;
using StoreApp.Model;

namespace StoreApp.Pages
{
    /// <summary>
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        public ProductsPage()
        {
            InitializeComponent();
            using(DatabaseContext db = new())
            {
                //List
                IQueryable<Product> products = db.Products
                    .Include(p => p.Category)
                    .OrderBy(p => p.IdProduct);

                foreach (var product in products)
                {
                    productsGrid.Items.Add(product);
                }

                //Adding and Filters
                IQueryable<Category> categories = db.Categories
                    .OrderBy(p => p.IdCategory);
                foreach(var category in categories)
                {
                    selectCategory.Items.Add(category.Name);
                    filterCategory.Items.Add(category.Name);
                }
            }
        }

        private void addProductButton_Click(object sender, RoutedEventArgs e)
        {
            string name = productNameBox.Text;
            if(name == null || name == "")
            {
                MessageBox.Show("Product name can't be empty");
                return;
            }
            if(selectCategory.Text == null)
            {
                MessageBox.Show("Choose category for the new product");
                return;
            }
            string chosenCategory = selectCategory.Text;


            using (DatabaseContext db = new())
            {
                IQueryable<Category> categoryToAdd = db.Categories
                    .Where(p => p.Name == chosenCategory);
                Category category = categoryToAdd.FirstOrDefault();

                Product newProduct = new Product { Name = name, CategoryId = category.IdCategory };
                db.Products.Add(newProduct);
                db.SaveChanges();
                productsGrid.Items.Add(newProduct);
            }
            productNameBox.Clear();
            selectCategory.SelectedIndex = -1;
        }

        private void removeProductButton_Click(object sender, RoutedEventArgs e)
        {
            Product product = productsGrid.SelectedItem as Product;
            using (DatabaseContext db = new())
            {
                db.Products.RemoveRange(product);
                db.SaveChanges();
            }
            productsGrid.Items.Remove(product);
        }


        private void filterProductsButton_Click(object sender, RoutedEventArgs e)
        {
            if(filterCategory.Text == null)
            {
                MessageBox.Show("Choose category for the new product");
                return;
            }
            productsGrid.Items.Clear();

            using (DatabaseContext db = new())
            {
                IQueryable<Product> products = db.Products
                    .Include(p => p.Category)
                    .Where(p => p.Category.Name == filterCategory.Text)
                    .OrderBy(p => p.IdProduct);

                foreach (var product in products)
                {
                    productsGrid.Items.Add(product);
                }
            }
        }

        private void resetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            productsGrid.Items.Clear();
            filterCategory.SelectedIndex = -1;

            using (DatabaseContext db = new())
            {
                IQueryable<Product> products = db.Products
                    .Include(p => p.Category)
                    .OrderBy(p => p.IdProduct);

                foreach (var product in products)
                {
                    productsGrid.Items.Add(product);
                }
            }
        }
    }
}
