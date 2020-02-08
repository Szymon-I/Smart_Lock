using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;

namespace listView
{
    [Activity(Label = "Logi")]
    public class Logi : Activity
    {
        LinearLayout ll;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Logi);

            ll = FindViewById<LinearLayout>(Resource.Id.linearLayoutLogi);

            string jsonString = Intent.GetStringExtra("jsonString");
            JArray jsonArray = JArray.Parse(jsonString);

            for (int ii = 0; ii < jsonArray.Count; ii++)
            {
                TextView textView = new TextView(this);
                textView.Text = "Akcja: " + jsonArray[ii]["action"];
                textView.SetTextColor(Color.White);
                ll.AddView(textView);

                textView = new TextView(this);
                textView.Text = "Data: " + jsonArray[ii]["date"];
                textView.SetTextColor(Color.White);
                ll.AddView(textView);

                textView = new TextView(this);
                textView.Text = "Użytkownik: " + jsonArray[ii]["user"];
                textView.SetTextColor(Color.White);
                ll.AddView(textView);

                textView = new TextView(this);
                ll.AddView(textView);

            }


            // Create your application here
        }
    }
}