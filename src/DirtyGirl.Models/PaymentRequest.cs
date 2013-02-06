using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DirtyGirl.Models
{
    [Serializable]
    [XmlRoot ("CreateTransactionRequest")]
    public class PaymentRequest
    {
        [XmlElement("name")]
        public string MerchantName { get; set; }

        [XmlElement("transactionKey")]
        public string MerchantKey { get; set; }

        [XmlElement("transactionType")]
        public string TransactionType { get; set; }

        [XmlElement("amount")]
        public decimal TransactionTotal { get; set; }

        [XmlElement("cardNumber")]
        public string CardNumber { get; set; }

        [XmlElement("expirationDate")]
        public string ExpirationDate { get; set; }

        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("address")]
        public string Address { get; set; }

        [XmlElement("city")]
        public string City { get; set; }

        [XmlElement("state")]
        public string State { get; set; }

        [XmlElement("zip")]
        public string Zip { get; set; }

        [XmlElement("country")]
        public string Country { get; set; }
    }
}
