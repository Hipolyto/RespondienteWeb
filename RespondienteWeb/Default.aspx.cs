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
            FirebaseDB firebaseDB = new FirebaseDB("https://primerrespondiente-2c016.firebaseio.com/");  
  
            // Referring to Node with name "Teams"  
            FirebaseDB firebaseDBTeams = firebaseDB.Node("Registro_de_Emergencia");  
  
            var data = @"{ 'nueva_emergencia': 1 }";
  
            WriteLine("POST Request");  
            FirebaseResponse postResponse = firebaseDBTeams.Post(data);  
            WriteLine(postResponse.Success);  
            WriteLine();  


  
            WriteLine(firebaseDBTeams.ToString());  
            ReadLine();  
        }
    }
}
