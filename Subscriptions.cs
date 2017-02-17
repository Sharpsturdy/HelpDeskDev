
namespace Help_Desk_2
{
    using System;
    public partial class Subscriptions
    {
        public int ID { get; set; }
        public byte kfType { get; set; }
        public string loginName { get; set; }
        public string emailAddress { get; set; }
        public string headerText { get; set; }
        public string userName { get; set; }
    }

    public class Subscription
    {
        public DateTime Date { get; internal set; }
        public string FaqHeader { get; internal set; }
        public int FaqId { get; internal set; }
        public string OriginatorName { get; internal set; }
        public int SubscribeFor { get; internal set; }
        
    }

    public class Subscriber
    {
        public string EmailAddress { get; internal set; }
        public string UserFirstName { get; internal set; }
        public string UserSurName { get; internal set; }
        public string LoginName { get; internal set; }
    }


}