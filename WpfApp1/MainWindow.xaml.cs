using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public DataTable table;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateTableButton_Click(object sender, RoutedEventArgs e)
        {
            table = new DataTable();
            table.Columns.Add("Nome", typeof(string));
            table.Columns.Add("Data", typeof(string));
            table.Columns.Add("Acconto", typeof(string));
            DataTable.ItemsSource = table.DefaultView;

            AddRowButton.IsEnabled = true;
            RemoveRowButton.IsEnabled = true;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                table = new DataTable();
                using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                {
                    string[] headers = sr.ReadLine().Split('~');
                    foreach (string header in headers)
                    {
                        table.Columns.Add(header, typeof(string));
                    }
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split('~');
                        DataRow dr = table.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i];
                        }
                        table.Rows.Add(dr);
                    }
                }
                DataTable.ItemsSource = table.DefaultView;

                AddRowButton.IsEnabled = true;
                RemoveRowButton.IsEnabled = true;
            }
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                StringBuilder sb = new StringBuilder();

                IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName);
                sb.AppendLine(string.Join("~", columnNames));

                foreach (DataRow row in table.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    sb.AppendLine(string.Join("~", fields));
                }

                File.WriteAllText(saveFileDialog.FileName, sb.ToString());
            }
        }

        private void AddRowButton_Click(object sender, RoutedEventArgs e)
        {
            DataRow newRow = table.NewRow();
            table.Rows.Add(newRow);
        }

        private void RemoveRowButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataTable.SelectedItem != null && DataTable.SelectedItem is DataRowView rowView)
            {
                DataRow row = rowView.Row;
                table.Rows.Remove(row);
            }

        }

        private void AddColumnButton_Click(object sender, RoutedEventArgs e)
        {
            // Prompt the user for the column name
            string columnName = Microsoft.VisualBasic.Interaction.InputBox("Enter the name of the new column:", "Add Column");

            // Add the new column to the DataTable
            DataColumn newColumn = new DataColumn(columnName);
            table.Columns.Add(newColumn);

            // Set the DataTable's ItemsSource property to the new DataView
            DataView view = table.DefaultView;
            DataTable.ItemsSource = view;
        }

        private void RemoveColumnButton_Click(object sender, RoutedEventArgs e)
        {
            if (table.Columns.Count != 3)
            {
                if (table.Columns.Count > 0)
                {
                    table.Columns.RemoveAt(0);
                    DataTable.ItemsSource = table.DefaultView;
                }
            }
        }


    }
}
