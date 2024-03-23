using System.ComponentModel.DataAnnotations;

namespace CPFValidatorAPI.Models
{
    public class CPFModelos
    {
        public class AuthorizedCPFs
        {
            public List<string> CPFs { get; set; }
            public string url { get; set; }
        }

        public class CPFRequest
        {
            [Required(ErrorMessage = "HTTP falhou ou veio sem quesitos necessários")]
            public string CPF { get; set; }
        }

        public class FlowActionSendMedia
        {
            public int mediaType { get; set; }
            public string url { get; set; }
            public string fileName { get; set; }
            public string caption { get; set; }
            public int delay { get; set; }
            public int type { get; set; }
        }

        public class FlowActionSendText
        {
            public string text { get; set; }
            public int delay { get; set; }
            public int type { get; set; }
        }

    }

}
