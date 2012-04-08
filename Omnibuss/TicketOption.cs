using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Omnibuss
{
    public class TicketOption
    {
        public TicketOption(String name, String code, String price)
        {
            Name = name;
            Code = code;
            Price = price;
        }

        public static List<TicketOption> getOptions()
        {
            List<TicketOption> list = new List<TicketOption>();
            list.Add(new TicketOption("Tunnipilet", "100", "0.96 €"));
            list.Add(new TicketOption("Päeva pilet", "106", "2.11 €"));
            list.Add(new TicketOption("10 päeva kaart", "115", "7.03 €"));
            list.Add(new TicketOption("30 päeva kaart", "135", "15.34 €"));
            list.Add(new TicketOption("90 päeva kaart", "155", "35.15 €"));
            list.Add(new TicketOption("10 päeva sooduskaart", "145", "3.52 €"));
            list.Add(new TicketOption("30 päeva sooduskaart (õ, II)", "165", "5.11 €"));
            list.Add(new TicketOption("30 päeva sooduskaart (üõ, p)", "195", "7.67 €"));
            return list;
        }

        public static TicketOption getOptionByCode(String code)
        {
            List<TicketOption> list = getOptions();
            foreach (TicketOption option in list)
            {
                if (option.Code.Equals(code))
                {
                    return option;
                }
            }
            return null;
        }

        public String Name { get; set; }
        public String Code { get; set; }
        public String Price { get; set; }
    }
}
