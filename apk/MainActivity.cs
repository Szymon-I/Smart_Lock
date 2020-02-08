using Android.App;
using Android.Widget;
using Android.OS;
using SQLite;
using System.IO;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using Android.Content;
using Android.Views;

namespace listView
{
    [Activity(Label = "WebLock", Icon="@drawable/icon", MainLauncher = true)]
    public class MainActivity : Activity
    {

        Button buttonZaloguj;
        EditText editTextIp;
        EditText editTextLogin;
        EditText editTextPassword;
        string token = "";
        string sUrl = "";
        string sContentType = "application/json";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.Main);

            buttonZaloguj = FindViewById<Button>(Resource.Id.buttonZaloguj);
            editTextIp = FindViewById<EditText>(Resource.Id.editTextIp);
            editTextLogin = FindViewById<EditText>(Resource.Id.editTextLogin);
            editTextPassword = FindViewById<EditText>(Resource.Id.editTextPassword);

            buttonZaloguj.Click += async delegate {

                sUrl = "http://" + editTextIp.Text + ":8000/api/login";

                JObject oJsonObject = new JObject();
                oJsonObject.Add("email", editTextLogin.Text);
                oJsonObject.Add("password", editTextPassword.Text);

                HttpClient oHttpClient = new HttpClient();
                //oHttpClient.Timeout = TimeSpan.FromSeconds(1);
                try
                {
                    HttpResponseMessage httpResponse = await oHttpClient.PostAsync(sUrl, new StringContent(oJsonObject.ToString(), Encoding.UTF8, sContentType));
                    
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        oJsonObject = JObject.Parse(await httpResponse.Content.ReadAsStringAsync());

                        token = oJsonObject.GetValue("token").ToString();

                        Intent activityListaZamkow = new Intent(this, typeof(ListaZamkow));
                        activityListaZamkow.PutExtra("token", token);
                        activityListaZamkow.PutExtra("ip", editTextIp.Text);

                        StartActivity(activityListaZamkow);
                    }
                    else
                    {
                        Toast t = Toast.MakeText(Application.Context, "Pdano błędne dane", ToastLength.Long);
                        t.SetGravity(GravityFlags.Top, 0, 100);
                        t.Show();
                    }
                }
                catch
                {
                    Toast t = Toast.MakeText(Application.Context, "Błąd połączenia", ToastLength.Long);
                    t.SetGravity(GravityFlags.Top, 0, 100);
                    t.Show();
                }

            };

        }


    }
}

