using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using System.Collections.Generic;

namespace ContentProvider
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity,
Android.Views.View.IOnClickListener, ListView.IOnItemClickListener, ListView.IOnItemLongClickListener
    {
        Button btnFindByName, btnFindByPhone;
        EditText etName, etPhone;
        Switch swFind, swCall;
        ListView lvContacts;
        List<MyContact> lstContacts;
        ContactAdapter ca;
        Android.Net.Uri uri;
        string[] projection;
        int nameIndex, phoneIndex;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.layout3);
            InitViews();
        }
        private void InitViews()
        {
            btnFindByName = FindViewById<Button>(Resource.Id.btnFindByName);
            btnFindByPhone = FindViewById<Button>(Resource.Id.btnFindByPhone);
            swFind = FindViewById<Switch>(Resource.Id.swFind);
            swCall = FindViewById<Switch>(Resource.Id.swCall);
            etName = FindViewById<EditText>(Resource.Id.etName);
            etPhone = FindViewById<EditText>(Resource.Id.etPhone);
            lvContacts = FindViewById<ListView>(Resource.Id.lvContacts);

            lvContacts.OnItemClickListener = this;
            lvContacts.OnItemLongClickListener = this;

            btnFindByName.SetOnClickListener(this);
            btnFindByPhone.SetOnClickListener(this);

            // Init objects
            lstContacts = new List<MyContact>();
            uri = ContactsContract.CommonDataKinds.Phone.ContentUri; // using Android.Provider;

            projection = new string[]
            {
        ContactsContract.Contacts.InterfaceConsts.DisplayName,
        ContactsContract.CommonDataKinds.Phone.Number
            };

            string[] arrPermissions = new string[]
            {
        Manifest.Permission.ReadContacts,
        Manifest.Permission.CallPhone
            };

            ActivityCompat.RequestPermissions(this, arrPermissions, General.REQUEST_PERMISSIONS);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum]
    Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            List<string> lstMissingPermissions = new List<string>();
            string missingPremissions = string.Empty;

            if (requestCode == General.REQUEST_PERMISSIONS)
            {
                for (int i = 0; i < grantResults.Length; i++)
                {
                    if (grantResults[i] != Permission.Granted)
                    {
                        lstMissingPermissions.Add(permissions[i]);
                        missingPremissions += permissions[i] + "\n";
                    }
                }

                if (lstMissingPermissions.Count > 0)
                {
                    Toast.MakeText(this, "Missing permissions:\n" + missingPremissions, ToastLength.Long).Show();
                }
            }
        }

        public void OnClick(View v)
        {
            if (v == btnFindByName)
                FindByName(etName.Text);
            else if (v == btnFindByPhone)
                FindByPhone(etPhone.Text);

        }
        private void FindByPhone(string number)
        {
            ICursor cursor = ContentResolver.Query(uri, projection,
            ContactsContract.CommonDataKinds.Phone.Number + " = ?",

            new string[] { number }, null);

            LoadContacts(cursor);
        }
        private void LoadContacts(ICursor cursor)
        {
            lstContacts.Clear();
            nameIndex =
            cursor.GetColumnIndex(projection[General.DISPLAY_NAME_INDEX]);
            phoneIndex =
            cursor.GetColumnIndex(projection[General.PHONE_NUMBER_INDEX]);
            while (cursor.MoveToNext())
            {
                MyContact contact = new MyContact()
                {
                    Name = cursor.GetString(nameIndex),
                    Phone = cursor.GetString(phoneIndex)

                };
            lstContacts.Add(contact);
            }
            ca = new ContactAdapter(this, lstContacts);
            lvContacts.Adapter = ca;
        }
        private void FindByName(string name)
        {
            string find = swFind.Checked ? string.Empty : "%";
            ICursor cursor = ContentResolver.Query(uri, projection,
            ContactsContract.Contacts.InterfaceConsts.DisplayName + " LIKE '" + find + name +
            "%'", null, projection[General.DISPLAY_NAME_INDEX] + " DESC");
            LoadContacts(cursor);
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            CallContact(position);
        }
        private void CallContact(int position)
        {
            string action = swCall.Checked ? Intent.ActionCall : Intent.ActionDial;
            Intent intent = new Intent(action);
            intent.SetData(Android.Net.Uri.Parse("tel: " + lstContacts[position].Phone));
            StartActivity(intent);
        }
        public bool OnItemLongClick(AdapterView parent, View view, int position, long id)
        {
            SmsContact(position);
            return true;
        }
        private void SmsContact(int position)
        {
            Intent intent = new Intent(Intent.ActionSendto,
            Android.Net.Uri.Parse("smsto:" + lstContacts[position].Phone));
            intent.PutExtra("sms_body", "Please call me back");
            StartActivity(intent);
        }

    }
}