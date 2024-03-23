using Newtonsoft.Json;
using System;
using System.Linq;
using static CPFValidatorAPI.Models.CPFModelos;

namespace CPFValidatorAPI.Helpers
{
    public static class CPFHelper
    {

        public static string bancoDeDados { get; set; }

        public static bool IsCpfValid(string cpf)
        {
            // O algoritmo observará se o CPF veio completamente em números, ou no formato correto, e tamanho correto. Se não, já devolverá.
            if (!cpf.All(char.IsDigit) && !(cpf.Length == 14 && cpf[3] == '.' && cpf[7] == '.' && cpf[11] == '-'))
                return false;

            // Se sim, converterá em uma string sem . ou -, porém mantendo possíveis caracteres não numéricos para as próximas verificações
            cpf = new string(cpf.Where(c => c != '.' && c != '-').ToArray());

            // Apenas números são permitidos em CPF
            foreach (char c in cpf)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            // Tamanho do CPF
            if (cpf.Length != 11)
                return false;
            // Se o CPF é distinto
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

            Console.WriteLine(cpf);

            // CPF é válido
            return true;
        }
    }
}
