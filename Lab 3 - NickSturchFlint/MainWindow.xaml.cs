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
using System.Data;

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

                //Validations
                //If any of the textboxes are empty, do not add it to the database
                if (txtName.Text == string.Empty || txtShares.Text == string.Empty || dtPicker.SelectedDate == null)
                {
                    MessageBox.Show("None of the fields can be empty! Please fill in the required information.");
                    //Reset text boxes and connections
                    txtName.Text = string.Empty;
                    txtShares.Text = string.Empty;
                    dtPicker.SelectedDate = null;
                    rdCommon.IsChecked = false;
                    rdPreferred.IsChecked = false;
                }
                else if (rdCommon.IsChecked == false && rdPreferred.IsChecked == false)
                {
                    MessageBox.Show("Must select a share type!");
                    rdCommon.IsChecked = false;
                    rdPreferred.IsChecked = false;
                }
                else
                {

                    //Make sure Shares textbox is a whole number
                    int shares;
                    if (int.TryParse(txtShares.Text, out shares))
                    {
                        //grab values
                        string name = txtName.Text;
                        string date = dtPicker.SelectedDate.ToString();
                        string shareType = "";
                        //Check the radio buttons for share type
                        if (rdCommon.IsChecked == true)
                        {
                            shareType = "Common";
                            CommonShare normalShare = new CommonShare(name, date, shares);
                            MessageBox.Show(normalShare.ToString());
                        }
                        else if (rdPreferred.IsChecked == true)
                        {
                            shareType = "Preferred";
                            PreferredShare expensiveShare = new PreferredShare(name, date, shares);
                            MessageBox.Show(expensiveShare.ToString());
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

                        if (availableShares < 0) //there are not enough shares
                        {
                            MessageBox.Show("Sorry, there aren't enough shares left.");
                        }
                        else //there are enough shares
                        {
                            string updateQuery = "";
                            if (shareType == "Common")
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
                            rdCommon.IsChecked = false;
                            rdPreferred.IsChecked = false;
                            conn.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Shares MUST be a WHOLE Number!");
                        txtShares.Text = string.Empty;
                    }

                }

            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.ToString());
            }
        }

        private void FillDataGrid()
        {
            try
            {
                //Connect to the db
                string connectString = Properties.Settings.Default.connect_string;
                SqlConnection conn = new SqlConnection(connectString);
                conn.Open();

                //Query the db
                string selectQuery = "SELECT * FROM buyers";
                SqlCommand commandQuery = new SqlCommand(selectQuery, conn);

                //Retrieve the data
                SqlDataAdapter sda = new SqlDataAdapter(commandQuery);
                DataTable dt = new DataTable("Buyers");

                //Set the data
                sda.Fill(dt);
                viewGrid.ItemsSource = dt.DefaultView;
            }
            catch(Exception ex)
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
                        //Common Shares
                        string retrieveCommonSold = "SELECT SUM(shares) FROM buyers WHERE shareType = 'Common'";
                        SqlCommand fourthCommand = new SqlCommand(retrieveCommonSold, conn);

                        //Preferred Shares
                        string retrievePreferredSolde = "SELECT SUM(shares) FROM buyers WHERE shareType = 'Preferred'";
                        SqlCommand fifthCommand = new SqlCommand(retrievePreferredSolde, conn);


                        //Revenue Generated
                        string retrieveDate = "SELECT shares, DATEDIFF(DAY, '1990-01-01', datePurchased) AS days FROM buyers WHERE shareType = 'Common' OR shareType = 'Preferred'"; //So far just common
                        //Creating a hidden table to pull the data from later
                        SqlDataAdapter dateCommand = new SqlDataAdapter(retrieveDate, conn);
                        DataTable dt = new DataTable();
                        dateCommand.Fill(dt);

                        //variables to store data in foreach loop
                        string date = "";
                        int revenue = 0;
                        int numShares;
                        //Run through each row on the hidden table
                        foreach (DataRow row in dt.Rows)
                        {
                            //Stores an int based on the date in the row
                            date = row["days"].ToString();
                            //Stores the number of shares in a row
                            numShares = (int)row["shares"];
                            //Generates random price for stocks
                            Random rand = new Random();
                            int money = rand.Next(int.Parse(date));
                            //Calculates the money made
                            revenue += money * numShares;
                        }

                        //Common Available
                        string availableCommon = "SELECT numCommonShares FROM shares";
                        SqlCommand seventhCommand = new SqlCommand(availableCommon, conn);

                        //Preferred Available
                        string availablePreferred = "SELECT numPreferredShares FROM shares";
                        SqlCommand eigthCommand = new SqlCommand(availablePreferred, conn);

                        //Updating the Text Blocks
                        //Common Sold
                        int commonShares = Convert.ToInt32(fourthCommand.ExecuteScalar());
                        txtCommonSold.Text = commonShares.ToString();
                        //Preferred Sold
                        int preferredShares = Convert.ToInt32(fifthCommand.ExecuteScalar());
                        txtPreferredSold.Text = preferredShares.ToString();
                        //Revenue
                        txtRevenue.Text = revenue.ToString();
                        //Common Available
                        int commonSharesAvailable = Convert.ToInt32(seventhCommand.ExecuteScalar());
                        commonSharesAvailable = commonSharesAvailable - int.Parse(txtCommonSold.Text);
                        txtCommonAvailable.Text = commonSharesAvailable.ToString();
                        //Preferred Available
                        int preferredSharesAvailable = Convert.ToInt32(eigthCommand.ExecuteScalar());
                        preferredSharesAvailable = preferredSharesAvailable - int.Parse(txtPreferredSold.Text);
                        txtPreferredAvailable.Text = preferredSharesAvailable.ToString();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    break;

                case "View Entries":

                    FillDataGrid();

                    break;

                case "Create Entry":

                    break;
            }
        }
    }
}
