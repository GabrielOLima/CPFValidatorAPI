using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using static CPFValidatorAPI.Models.CPFModelos;
using static CPFValidatorAPI.Helpers.CPFHelper;
using CPFValidatorAPI.Helpers;

namespace CPFValidatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CPFController : ControllerBase
    {
        private readonly string bancoDeDados = "autorizacao.json";
        // Recebe input vazio
        [HttpPost("validate")]
        public IActionResult ValidateCPF([FromBody] CPFRequest cpfRequest)
        {
            string cpf = cpfRequest.CPF;
            try
            {
                if (cpfRequest == null || string.IsNullOrEmpty(cpfRequest.CPF))
                {
                    FlowActionSendText flowActionSendText = new FlowActionSendText
                    {
                        text = "CPF não fornecido, favor tentar novamente.",
                        delay = 0,
                        type = 0
                    };

                    return Ok(flowActionSendText);
                }
                if (!IsCpfValid(ref cpf))
                {
                    FlowActionSendText flowActionSendText = new FlowActionSendText
                    {
                        text = "CPF inválido, aguarde o reinício do processo para tentar novamente...",
                        delay = 0,
                        type = 0
                    };
                    return Ok(flowActionSendText);
                }


                if (!IsCPFAuthorized(cpf, bancoDeDados))
                {
                    // Se o CPF é válido, mas não registrado
                    FlowActionSendText flowActionSendText = new FlowActionSendText
                    {
                        text = "CPF válido, porém não registrado. Entre em contato para obter registro. Aguarde o reinício do processo para tentar novamente...",
                        delay = 0,
                        type = 0
                    };
                    return Ok(flowActionSendText);
                }

                string url = GetURLFromJSON(bancoDeDados);

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
    }
}
