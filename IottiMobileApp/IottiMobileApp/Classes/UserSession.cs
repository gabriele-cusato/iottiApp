using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbMobileModel.Models.IntermediateDb;

namespace IottiMobileApp.Classes
{
    public class UserSession
    {
        public static Utente? UtenteCorrente { get; set; }

        public static MissioneTes? FieraSelezionata { get; set; }
    }
}
