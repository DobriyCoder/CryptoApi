﻿namespace CryptoApi.Models.DB
{
    public class CCoinDataM
    {
        public uint id { get; set; }
        public string donor { get; set; }
        public string donor_id { get; set; }
        public string name_full { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public decimal usd { get; set; }
        public string image { get; set; }
        public string market_cap { get; set; }
        public string change_day { get; set; }
        public string change_week { get; set; }
        public string change_month { get; set; }
        public string change_price { get; set; }
        public ICollection<CCoinsMetaDataM> meta { get; set; }

        public CCoinDataM ()
        {
            meta = new List<CCoinsMetaDataM> ();
        }

        public IEnumerable<CCoinsMetaDataM> this[string group]
        {
            get
            {
                return meta.Where(x => x.group == group);
            }
        }
        public CCoinsMetaDataM this[string group, string option]
        {
            get
            {
                return meta.Where(x => x.group == group && x.option == option).First();
            }
        }
    }
}
