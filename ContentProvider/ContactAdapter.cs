using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentProvider
{
    class ContactAdapter : BaseAdapter<MyContact>
    {
        private readonly List<MyContact> lstContacts;
        private readonly Context context;
        public ContactAdapter(Context context, List<MyContact> lstContacts)
        {
            this.context = context;
            this.lstContacts = lstContacts;
        }
        public override MyContact this[int position] => lstContacts[position];
        public override int Count => lstContacts.Count;
        public override long GetItemId(int position)
        {
            return position;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater layoutInflater = ((MainActivity)context).LayoutInflater;
            View view = layoutInflater.Inflate(Resource.Layout.contactlistitemlayout, parent, false);
            TextView tvName = view.FindViewById<TextView>(Resource.Id.tvName);
            TextView tvPhone = view.FindViewById<TextView>(Resource.Id.tvPhone);
            tvName.Text = lstContacts[position].Name;
            tvPhone.Text = lstContacts[position].Phone;
            return view;
        }
    }
}