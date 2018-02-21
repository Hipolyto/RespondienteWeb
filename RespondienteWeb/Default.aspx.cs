using System;
using System.Web;
using System.Web.UI;

using static System.Console;

using FirebaseNet.Database;

namespace RespondienteWeb
{

    public partial class Default : System.Web.UI.Page
    {
        public void button1Clicked(object sender, EventArgs args)
        {
            // Instanciating with base URL  
            FirebaseManager.SendNotification();
        }
    }
}
