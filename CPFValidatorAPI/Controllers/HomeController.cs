using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using static CPFValidatorAPI.Models.CPFModelos;
using static CPFValidatorAPI.Helpers.CPFHelper;

namespace CPFValidatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CPFController : ControllerBase
    {
        private readonly string authorizedCPFFilePath = "autorizacao.json";
        // Recebe input vazio
        [HttpPost("validate")]
        public IActionResult ValidateCPF([FromBody] CPFRequest cpfRequest)
        {
            string cpf = cpfRequest.CPF;
            try
            {
                if (!IsCpfValid(ref cpf))
                {
                    FlowActionSendText flowActionSendText = new FlowActionSendText
                    {
                        text = "CPF inválido, o mesmo deve ser completamente numérico, ou no formato tradicional 'XXX.XXX.XXX-YY', aguarde o reinicío do processo para tentar novamente...",
                        delay = 0,
                        type = 0
                    };
                    return Ok(flowActionSendText);
                }


                if (!IsCPFAuthorized(cpf))
                {
                    // Se o CPF é válido, mas não registrado
                    FlowActionSendText flowActionSendText = new FlowActionSendText
                    {
                        text = "CPF válido, porém não registrado. Entre em contato para obter registro. Aguarde o reinicío do processo para tentar novamente...",
                        delay = 0,
                        type = 0
                    };
                    return Ok(flowActionSendText);
                }

                string url = GetURLFromJSON();

                FlowActionSendMedia flowActionSendMedia = new FlowActionSendMedia
                {
                    mediaType = 1,
                    url = url,
                    fileName = "Boleto Dummy",
                    caption = "Boleto de Segunda Via, agradecemos a espera!",
                    delay = 0,
                    type = 1
                };

                return Ok(flowActionSendMedia);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);

                FlowActionSendText flowActionSendText = new FlowActionSendText
                {
                    text = "Ocorreu um erro interno no procedimento, favor tentar novamente mais tarde",
                    delay = 0,
                    type = 0
                };

                return Ok(flowActionSendText);
            }


        }
        // Validadores
        // Para fins de fazer na mão: 'https://www.macoratti.net/alg_cpf.htm'

        private bool IsCPFAuthorized(string cpf)
        {
            try
            {
                string jsonText = System.IO.File.ReadAllText(authorizedCPFFilePath);
                var authorizedCPFs = JsonConvert.DeserializeObject<AuthorizedCPFs?>(jsonText);

                return authorizedCPFs.CPFs.Contains(cpf);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao verificar CPF autorizado: " + ex.Message);
                return false;
            }
        }

        private string GetURLFromJSON()
        {
            try
            {
                string jsonText = System.IO.File.ReadAllText(authorizedCPFFilePath);
                var jsonObject = JsonConvert.DeserializeObject<AuthorizedCPFs>(jsonText);

                return jsonObject.url;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao obter URL do JSON: " + ex.Message);
                //Se retornar isso, deu problema. É apenas para não quebrar o output.
                return "https://clickdimensions.com/links/TestPDFfile.pdf";
            }
        }
    }



}
