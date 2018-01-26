using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MongoDB.Bson;
using pix_dtmodel.Connectors;
using pix_dtmodel.Managers;
using pix_dtmodel.Models;

namespace Pix_Backend
{
    class UserDataView : DataView
    {
        //Callbacks
        public static event EventHandler OnLoadingData, OnDataLoaded;

        //Control Bool 
        private static bool init = false;

        //Our Session.
        private static Session<User> collection;

        //Display
        private static ListView display;

        //Data
        private static List<User> loadedData;

        public UserDataView()
        {
            this.dataLabel = "This view uses a List View to load items from Mongo.";
          
            



            //Activate session, With users collection
            collection = new Session<User>(
                DataManager.Init(Defaults.ConnectionStrings.Localhost, Defaults.DatabaseNames.TestProd, null),
                Defaults.Collections.Users);

            //Attach View
            display = new ListView();
            //This sets a structure for the data.
            display.View = new UserLayoutView();

            //Attach double click
            display.MouseDoubleClick += (sender, args) =>
            {
                var item = ((ListViewItem) sender).Content as User;
                //Now we can get all the data from the item
                
                
            };
            
            //Fill Data
            FillData();

            



        }

        //Loads initial data
        public override async void FillData()
        {
            if (!init)
            {
                if (OnLoadingData != null)
                    OnLoadingData(null, new EventArgs());

                IQueryable<User> allData = await collection.GetAll();
                loadedData = allData.ToList();


                //Fill The content View
                display.ItemsSource = loadedData;




                //Set init as true after
                init = true;

                if (OnDataLoaded != null)
                    OnDataLoaded(null, new EventArgs());

            }


        }

        public static async void RefreshData()
        {
            if (init)
            {
                if (OnLoadingData != null)
                    OnLoadingData(null, new EventArgs());

                IQueryable<User> allData = await collection.GetAll();
                loadedData = allData.ToList();


                //Fill The content View
                display.ItemsSource = loadedData;




                //Set init as true after
                init = true;

                if (OnDataLoaded != null)
                    OnDataLoaded(null, new EventArgs());

            }
        }




        public ListView getDisplay()
        {
            display.Tag = "User View";
        
            return display;
        }
    }


    //This class specifies a grid view that structures the data as its loaded
    class UserLayoutView : GridView
    {
        public UserLayoutView()
        {
            this.AllowsColumnReorder = true; //Allow Reorder

            //Make all the columns
            
            GridViewColumn userColumn = new GridViewColumn();
            userColumn.Header = "User";
            userColumn.Width = 100;
            userColumn.DisplayMemberBinding = new Binding("Username");
            GridViewColumn emailCol = new GridViewColumn();
            emailCol.Header = "Email";
            emailCol.Width = 100;
            emailCol.DisplayMemberBinding = new Binding("Email");
            GridViewColumn uidColumn = new GridViewColumn();
            uidColumn.Header = "UID";
            uidColumn.Width = 100;
            uidColumn.DisplayMemberBinding = new Binding("Uid");
          

            //Add all columns 
            this.Columns.Add(emailCol);
            this.Columns.Add(userColumn);
            this.Columns.Add(uidColumn);
            
            
        }
    }

    
}
