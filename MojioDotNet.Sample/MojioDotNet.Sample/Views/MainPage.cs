using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MojioDotNet.Sample.Views
{
    public class MainPage : BaseContentPage
    {
        public MainPage()
        {
            MainText = "Hello world";
            this.BindingContext = this;

            this.Content = new Label()
            {
                Text = "Hello world"
            };

        }        
        public string MainText { get; set; }
      
    }
}
