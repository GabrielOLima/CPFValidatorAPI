using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using static CPFValidatorAPI.Models.CPFModelos;

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
            cpf = new string(cpf.Where(char.IsDigit).ToArray());
            Console.WriteLine(cpf);
            // Input valido
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
                        caption = "Boleto de Segunda Via, agradeçemos a espera!",
                        delay = 0,
                        type = 1
                    };

                    return Ok(flowActionSendMedia);
                }
                else
                {
                    // Se o CPF é válido, mas não autorizado
                    FlowActionSendText flowActionSendText = new FlowActionSendText
                    {
                        text = "CPF válido, porém não autorizado. Entre em contato para obter autorização.",
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
                    text = "CPF inválido, favor tentar novamente.",
                    delay = 0,
                    type = 0
                };

                return Ok(flowActionSendText);
            }
        }
        // Validadores
        // Para fins de fazer na mão: 'https://www.macoratti.net/alg_cpf.htm'
        private bool IsCpfValid(string cpf)
        {
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            // Apenas números são permitidos em CPF
            foreach (char c in cpf)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            if (cpf.Length != 11)
                return false;

            bool allDigitsAreEqual = cpf.Distinct().Count() == 1;
            if (allDigitsAreEqual)
                return false;

            // Calcula o primeiro dígito
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += int.Parse(cpf[i].ToString()) * (10 - i);
            }
            int remainder = (sum * 10) % 11;
            if (remainder == 10)
                remainder = 0;

            // Verifica o primeiro dígito
            if (cpf[9] - '0' != remainder)
                return false;

            // Calcula o segundo dígito
            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += int.Parse(cpf[i].ToString()) * (11 - i);
            }
            remainder = (sum * 10) % 11;
            if (remainder == 10)
                remainder = 0;

            // Verifica o segundo dígito
            if (cpf[10] - '0' != remainder)
                return false;

            // CPF é válido
            return true;
        }

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
                return "https://www.example.com/default.pdf";
            }
        }
    }



}
