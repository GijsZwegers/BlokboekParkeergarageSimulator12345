using BlokboekParkeergarageSimulator.Functions;
using Newtonsoft.Json;
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

namespace BlokboekParkeergarageSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string url = null;

        ApiClass.ApiFuncionClass API = new ApiClass.ApiFuncionClass();
        JsonClasses.newtonsoftJSON Convert = new JsonClasses.newtonsoftJSON();
        Functions.CheckConnection Connection = new Functions.CheckConnection();
        public MainWindow()
        {
            InitializeComponent(); 
        }

        private void btInloggen_Click(object sender, RoutedEventArgs e)
        {
            //internet checken in de internetclass
            if (CheckConnection.CheckInternet())
            {
                if (tbUser.Text == "" && pbPass.Password == "")
                {
                    MessageBox.Show("Voer een gebruikersnaam en wachtwoord in.");
                }
                else if (tbUser.Text == "")
                {
                    MessageBox.Show("Voer een gebruikersnaam in.");
                }
                else if (pbPass.Password == "")
                {
                    MessageBox.Show("Voer een wachtwoord in.");
                }
                else if (tbUser.Text != "" && pbPass.Password != "")
                {
                    //url voor de authencatie via de api
                    url = "https://avondmtb.nl/middenpolder/auth";
                    string json = "{\"User_Name\":\"" + tbUser.Text + "\"," +
                            "\"User_Pass\":\"" + pbPass.Password + "\"}";
                    //gegevens plus url naar de api class sturen die de api call verwerkt
                    string result = API.Login(url, json);
                    JsonClasses.newtonsoftJSON.RootObject response = JsonConvert.DeserializeObject<JsonClasses.newtonsoftJSON.RootObject>(result);
                    //De JWT code meegeven voor controle en het ophalen van de voor en achternaam
                    

                    if (response.success == true)
                    {
                        if (response.rechten >= 2)
                        {
                            //CMS scherm openen
                            CMS.CMS CMS = new CMS.CMS();
                            CMS.Show();
                            //Dit scherm sluiten
                            this.Close();
                        }
                        else {
                            MessageBox.Show("U hebt niet voldoende rechten, neem contact op met uw systeembeheerder");
                        }
                    }
                    else {
                        MessageBox.Show(response.message);
                    }
                }
            }
            else {
                MessageBox.Show("Er kan geen connectie met de server gemaakt worden. \nCheck uw internetconnectie of neem contact op met de systeembeheerder");
            }
         }
        private void PressedEnter(object sender, KeyEventArgs e)
        {
            if (btInloggen.IsEnabled == true)
            {
                if (e.Key == Key.Enter)
                {
                    btInloggen_Click(sender, new RoutedEventArgs());
                }
            }
        }
        private void TbUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbUser.Text != "")
            {
                btInloggen.IsEnabled = true;
            }
            if (tbUser.Text == "")
            {
                btInloggen.IsEnabled = false;
            }
        }
    }
}