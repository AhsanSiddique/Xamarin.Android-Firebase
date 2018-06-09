using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using FirebaseDatabase.Model;
using System.Collections.Generic;

namespace FirebaseDatabase
{
    public class ListViewAdapter : BaseAdapter
    {
        Activity activity;
        List<Account> lstAccounts;
        LayoutInflater inflater;

        public ListViewAdapter(Activity activity, List<Account> lstAccounts)
        {
            this.activity = activity;
            this.lstAccounts = lstAccounts;
        }
        public override int Count
        {
            get { return lstAccounts.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            inflater = (LayoutInflater)activity.BaseContext.GetSystemService(Context.LayoutInflaterService);
            View itemView = inflater.Inflate(Resource.Layout.list_Item, null);
            var txtuser = itemView.FindViewById<TextView>(Resource.Id.list_name);
            var txtemail = itemView.FindViewById<TextView>(Resource.Id.list_email);
            if(lstAccounts.Count > 0)
            {
                txtuser.Text = lstAccounts[position].name;
                txtemail.Text = lstAccounts[position].email;
            }
            return itemView;
        }
    }
}