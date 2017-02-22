using Postal;

namespace Help_Desk_2.Models.EmailMessageModels
{
    public class FaqKbEmail:Email
    {
        public FaqKbEmail(string template) : base(template) { }

        public string ArticleUrl { get; set; }
        public string Cc { get; set; }
        public string From { get; set; }
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Title { get; set; }
        public string To { get; set; }
        public string Type { get; set; }
        public string User { get; set; }        
    }
}