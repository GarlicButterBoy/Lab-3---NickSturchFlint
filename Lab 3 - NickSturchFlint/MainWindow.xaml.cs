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
using System.Data.SqlClient;

namespace Lab_3___NickSturchFlint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //grab values
                string name = txtName.Text;
                int shares = int.Parse(txtShares.Text);
                string date = dtPicker.SelectedDate.ToString();
                string shareType = "";
                //Check the radio buttons for share type
                if(rdCommon.IsChecked == true)
                {
                    shareType = "Common";
                }
                else if (rdPreferred.IsChecked == true)
                {
                    shareType = "Preferred";
                }

                //connect to the db
                string connectString = Properties.Settings.Default.connect_string;
                SqlConnection conn = new SqlConnection(connectString);
                conn.Open();

                //Insert Query
                string insertString = "INSERT INTO buyers (name, shares, datePurchased, shareType) VALUES ('" + name + "', '" + shares + "', '" + date + "', '" + shareType + "')";
                SqlCommand command = new SqlCommand(insertString, conn);
                command.ExecuteNonQuery();

                //Depending on Radio Button Selection
                string selectionQuery = "";
                if (shareType == "Common")
                {
                    selectionQuery = "SELECT numCommonShares FROM shares";
                }
                else if (shareType == "Preferred")
                {
                    selectionQuery = "SELECT numPreferredShares FROM shares";
                }

                SqlCommand secondCommand = new SqlCommand(selectionQuery, conn);
                int availableShares = Convert.ToInt32(secondCommand.ExecuteScalar());

                availableShares = availableShares - shares;

                if(availableShares < 0) //there are not enough shares
                {
                    MessageBox.Show("Sorry, there aren't enough shares left.");
                }
                else //there are enough shares
                {
                    string updateQuery = "";
                    if(shareType == "Common")
                    {
                        updateQuery = "UPDATE shares SET numCommonShares = '" + availableShares + "' ";
                        SqlCommand thirdCommand = new SqlCommand(updateQuery, conn);
                        thirdCommand.ExecuteScalar();
                    }
                    else if (shareType == "Preferred")
                    {
                        updateQuery = "UPDATE shares SET numPreferredShares = '" + availableShares + "' ";
                        SqlCommand thirdCommand = new SqlCommand(updateQuery, conn);
                        thirdCommand.ExecuteScalar();
                    }
                    //Show that update was successful
                    MessageBox.Show("Successfully added share purchase!");
                    //Reset text boxes and connections
                    txtName.Text = string.Empty;
                    txtShares.Text = string.Empty;
                    dtPicker.SelectedDate = null;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.ToString());
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;

            switch (tabItem)
            {
                case "View Summary":
                    try
                    {
                        //Connect to the db
                        string connectString = Properties.Settings.Default.connect_string;
                        SqlConnection conn = new SqlConnection(connectString);
                        conn.Open();

                        //Prepare and Execute Query
                        string retrieveSharesSold = "SELECT SUM(shares) FROM buyers";
                        SqlCommand fourthCommand = new SqlCommand(retrieveSharesSold, conn);

                        int soldShares = Convert.ToInt32(fourthCommand.ExecuteScalar());
                        txtCommonSold.Text = soldShares.ToString();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    break;

                case "View Entries":

                    break;

                case "Create Entry":

                    break;
            }
        }
    }
}
