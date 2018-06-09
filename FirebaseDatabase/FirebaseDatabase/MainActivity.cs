using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using System.Collections.Generic;
using FirebaseDatabase.Model;
using System;
using System.Threading.Tasks;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace FirebaseDatabase
{
    [Activity(Label = "FirebaseDatabase", MainLauncher = true , Theme ="@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        private EditText input_name, input_email;
        private ListView list_data;
        private ProgressBar circular_progress;
        private List<Account> list_users = new List<Account>();
        private ListViewAdapter adapter;
        private Account selectedAccount;
        private const string FirebaseURL = "https://fir-database-33529.firebaseio.com/"; //Firebase Database URL
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Add Toolbar

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "Firebase Database";
            SetSupportActionBar(toolbar);

            //View 

            circular_progress = FindViewById<ProgressBar>(Resource.Id.circularProgress);
            input_name = FindViewById<EditText>(Resource.Id.name);
            input_email = FindViewById<EditText>(Resource.Id.email);
            list_data = FindViewById<ListView>(Resource.Id.list_data);
            list_data.ItemClick += (s, e) =>
            {
                Account account = list_users[e.Position];
                selectedAccount = account;
                input_name.Text = account.name;
                input_email.Text = account.email;
            };

            await LoadData();
        }

        private async Task LoadData()
        {
            circular_progress.Visibility = ViewStates.Visible;
            list_data.Visibility = ViewStates.Invisible;

            var firebase =new FirebaseClient(FirebaseURL);
            var items = await firebase
                .Child("users")
                .OnceAsync<Account>();

            list_users.Clear();
            adapter = null;
            foreach (var item in items)
            {
                Account account = new Account();
                account.uid = item.Key;
                account.name = item.Object.name;
                account.email = item.Object.email;

                list_users.Add(account);
            }
            adapter = new ListViewAdapter(this, list_users);
            adapter.NotifyDataSetChanged();
            list_data.Adapter = adapter;

            circular_progress.Visibility = ViewStates.Invisible;
            list_data.Visibility = ViewStates.Visible;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if(id == Resource.Id.menu_add)
            {
                CreateUser();
            }
            else
            if(id == Resource.Id.menu_save) //Update
            {
                UpdateUser(selectedAccount.uid, input_name.Text, input_email.Text);
            }
            else
            if (id == Resource.Id.menu_delete) //Delete
            {
                DeleteUser(selectedAccount.uid);
            }
            return base.OnOptionsItemSelected(item);
        }

        private async void DeleteUser(string uid)
        {
            var firebase = new FirebaseClient(FirebaseURL);
            await firebase.Child("users").Child(uid).DeleteAsync();
            await LoadData();
        }

        private async void UpdateUser(string uid, string name, string email)
        {
            var firebase = new FirebaseClient(FirebaseURL);
            await firebase.Child("users").Child(uid).Child("name").PutAsync(name);
            await firebase.Child("users").Child(uid).Child("email").PutAsync(email);

            await LoadData();
        }

        private async void CreateUser()
        {
            Account user = new Account();
            user.uid = String.Empty;
            user.name = input_name.Text;
            user.email = input_email.Text;

            var firebase = new FirebaseClient(FirebaseURL);
            //Add Item
            var item = await firebase.Child("users").PostAsync<Account>(user);
            await LoadData();
        }
    }
}

