using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics;
using System.Text;
using System;
using Android.Views;

namespace listView
{
    [Activity(Label = "Zamki")]
    public class ListaZamkow : Activity
    {
        List<string> linkedList = new List<string>();
        LinearLayout ll;

        string token = "";
        string ip = "";
        string sUrl = "";
        string sUrlLock = "";
        string sContentType = "application/json";

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ListaZamkow);

            ll = FindViewById<LinearLayout>(Resource.Id.linearLayoutZamki);
            
            token = Intent.GetStringExtra("token");
            ip = Intent.GetStringExtra("ip");
            sUrl = "http://" + ip + ":8000/api/get_locks";
            sUrlLock = "http://" + ip + ":8000/api/lock_action";

            HttpClient oHttpClient = new HttpClient();
            oHttpClient.DefaultRequestHeaders.Add("Authorization", "Token " + token);
            HttpResponseMessage httpResponse = await oHttpClient.GetAsync(sUrl);

            if (httpResponse.IsSuccessStatusCode)
            {
                string json = await httpResponse.Content.ReadAsStringAsync();
                JArray oJsonArray = JArray.Parse(json);

                for (int i = 0; i < oJsonArray.Count; i++)
                {
                    TextView textView = new TextView(this);
                    textView.Text = "Id: " + oJsonArray[i]["id"];
                    textView.SetTextColor(Color.White);
                    ll.AddView(textView);

                    textView = new TextView(this);
                    textView.Text = "Hardware: " + oJsonArray[i]["hardware"];
                    textView.SetTextColor(Color.White);
                    ll.AddView(textView);

                    textView = new TextView(this);
                    textView.Text = "Lokalizacja: " + oJsonArray[i]["location"];
                    textView.SetTextColor(Color.White);
                    ll.AddView(textView);

                    textView = new TextView(this);
                    textView.Text = "Typ: " + oJsonArray[i]["type"];
                    textView.SetTextColor(Color.White);
                    ll.AddView(textView);

                    GridLayout gl = new GridLayout(this);
                    ll.AddView(gl);

                    Button button = new Button(this);
                    button.Background = new ColorDrawable(new Color(0x50FFFFFF));
                    button.SetWidth(400);
                    bool open = (bool)oJsonArray[i]["open_status"];
                    if (!open)
                    {
                        button.Text = "Otwórz: " + oJsonArray[i]["name"];
                        button.SetTextColor(Color.White);
                    }
                    else
                    {
                        button.Text = "Zamknij: " + oJsonArray[i]["name"];
                        button.SetTextColor(Color.OrangeRed);
                    }
                    button.Id = Convert.ToInt32(oJsonArray[i]["id"]);
                    button.Click += async delegate
                    {
                        if (button.Text.Substring(0, 6) == "Otwórz")
                        {
                            JObject oJsonObject = new JObject();
                            oJsonObject.Add("action", "open");
                            oJsonObject.Add("id", button.Id);

                            HttpClient httpClient = new HttpClient();
                            httpClient.DefaultRequestHeaders.Add("Authorization", "Token " + token);
                            httpResponse = await httpClient.PostAsync(sUrlLock, new StringContent(oJsonObject.ToString(), Encoding.UTF8, sContentType));

                            if (httpResponse.IsSuccessStatusCode)
                            {
                                button.Text = "Zamknij" + button.Text.Substring(6);
                                button.SetTextColor(Color.OrangeRed);
                            }
                            else
                            {
                                string jsonError = await httpResponse.Content.ReadAsStringAsync();
                                JObject oJsonError = JObject.Parse(jsonError);

                                if (oJsonError.Count != 3)
                                {
                                    Toast t = Toast.MakeText(Application.Context, "Otwieranie nieudane", ToastLength.Long);
                                    t.SetGravity(GravityFlags.Top, 0, 100);
                                    t.Show();
                                }
                                else
                                {
                                    Toast t = Toast.MakeText(Application.Context, oJsonError["error"].ToString(), ToastLength.Long);
                                    t.SetGravity(GravityFlags.Top, 0, 100);
                                    t.Show();
                                }
                            }
                        }
                        else
                        {
                            JObject oJsonObject = new JObject();
                            oJsonObject.Add("action", "close");
                            oJsonObject.Add("id", button.Id);

                            HttpClient httpClient = new HttpClient();
                            httpClient.DefaultRequestHeaders.Add("Authorization", "Token " + token);
                            httpResponse = await httpClient.PostAsync(sUrlLock, new StringContent(oJsonObject.ToString(), Encoding.UTF8, sContentType));

                            if (httpResponse.IsSuccessStatusCode)
                            {
                                button.Text = "Otwórz" + button.Text.Substring(7);
                                button.SetTextColor(Color.White);
                            }
                            else
                            {
                                string jsonError = await httpResponse.Content.ReadAsStringAsync();
                                JObject oJsonError = JObject.Parse(jsonError);

                                if (oJsonError.Count != 3)
                                {
                                    Toast t = Toast.MakeText(Application.Context, "Zamykanie nieudane", ToastLength.Long);
                                    t.SetGravity(GravityFlags.Top, 0, 100);
                                    t.Show();
                                }
                                else
                                {
                                    Toast t = Toast.MakeText(Application.Context, oJsonError["error"].ToString(), ToastLength.Long);
                                    t.SetGravity(GravityFlags.Top, 0, 100);
                                    t.Show();
                                }
                            }
                        }
                    };
                    gl.AddView(button);

                    textView = new TextView(this);
                    textView.SetWidth(100);
                    gl.AddView(textView);

                    Button button2 = new Button(this);
                    button2.Background = new ColorDrawable(new Color(0x50FFFFFF));
                    button2.Text = "Wyświetl logi";
                    button2.SetTextColor(Color.White);

                    button2.Id = Convert.ToInt32(oJsonArray[i]["id"]);
                    button2.SetWidth(400);
                    button2.Click += async delegate
                    {
                        JObject oJsonObject = new JObject();
                        oJsonObject.Add("action", "info");
                        oJsonObject.Add("id", button2.Id);

                        HttpClient httpClient = new HttpClient();
                        httpClient.DefaultRequestHeaders.Add("Authorization", "Token " + token);
                        httpResponse = await httpClient.PostAsync(sUrlLock, new StringContent(oJsonObject.ToString(), Encoding.UTF8, sContentType));

                        if (httpResponse.IsSuccessStatusCode)
                        {
                            string jsonString = await httpResponse.Content.ReadAsStringAsync();

                            Intent activityLogi = new Intent(this, typeof(Logi));
                            activityLogi.PutExtra("jsonString", jsonString);

                            StartActivity(activityLogi);
                        }
                        else
                        {
                            Toast t = Toast.MakeText(Application.Context, "Błąd wyświetlania logów", ToastLength.Long);
                            t.SetGravity(GravityFlags.Top, 0, 100);
                            t.Show();
                        }
                    };
                    gl.AddView(button2);

                    textView = new TextView(this);
                    ll.AddView(textView);
                }

            }

        }

    }
}