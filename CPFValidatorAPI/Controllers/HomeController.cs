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

        public CPFController()
        {
            CPFHelper.bancoDeDados = bancoDeDados;
        }
        // Recebe input vazio
        [HttpPost("validate")]
        public IActionResult ValidateCPF([FromBody] CPFRequest cpfRequest)
        {
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

                string cpf = cpfRequest.CPF;
                if (IsCpfValid(cpf))
                {
                    if (IsCPFAuthorized(cpf))
                    {
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
                    else
                    {
                        // Se o CPF é válido, mas não registrado
                        FlowActionSendText flowActionSendText = new FlowActionSendText
                        {
                            text = "CPF válido, porém não registrado. Entre em contato para obter registro.",
                            delay = 0,
                            type = 0
                        };

                        return Ok(flowActionSendText);
                    }
                }
                else
                {
                    // Se o CPF é inválido
                    FlowActionSendText flowActionSendText = new FlowActionSendText
                    {
                        text = "CPF inválido, favor enviá-lo completamente numérico, ou no formato tradicional 'XXX.XXX.XXX-YY'",
                        delay = 0,
                        type = 0
                    };

                    return Ok(flowActionSendText);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao validar CPF: " + ex.Message);
                // Retornar uma mensagem de erro genérica em caso de exceção
                FlowActionSendText flowActionSendText = new FlowActionSendText
                {
                    text = "Ocorreu um erro ao validar o CPF. Por favor, tente novamente mais tarde.",
                    delay = 0,
                    type = 0
                };

                return Ok(flowActionSendText);
            }
        }
    }

}
