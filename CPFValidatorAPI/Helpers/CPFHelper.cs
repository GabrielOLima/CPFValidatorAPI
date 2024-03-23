using Newtonsoft.Json;
using System;
using System.Linq;
using static CPFValidatorAPI.Models.CPFModelos;

namespace CPFValidatorAPI.Helpers
{
    public static class CPFHelper
    {

        public static string bancoDeDados { get; set; }

        public static bool IsCpfValid(ref string cpf)
        {
            // O algoritmo observará se o CPF veio completamente em números, ou no formato correto, e tamanho correto. Se não, já devolverá.
            if (!cpf.All(char.IsDigit) && !(cpf.Length == 14 && cpf[3] == '.' && cpf[7] == '.' && cpf[11] == '-'))
                return false;

            // Se sim, converterá em uma string sem . ou -, porém mantendo possíveis caracteres não numéricos para as próximas verificações
            // Usando ref para poder refletir alterações no CPF no controller sem repetir código
            string tempCpf = new string(cpf.Where(c => c != '.' && c != '-').ToArray());

            // Apenas números são permitidos em CPF
            foreach (char c in tempCpf)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            // Tamanho do CPF
            if (tempCpf.Length != 11)
                return false;
            // Se o CPF é distinto
            bool allDigitsAreEqual = tempCpf.Distinct().Count() == 1;
            if (allDigitsAreEqual)
                return false;

            // Calcula o primeiro dígito
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += int.Parse(tempCpf[i].ToString()) * (10 - i);
            }
            int remainder = (sum * 10) % 11;
            if (remainder == 10)
                remainder = 0;

            // Verifica o primeiro dígito
            if (tempCpf[9] - '0' != remainder)
                return false;

            // Calcula o segundo dígito
            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += int.Parse(tempCpf[i].ToString()) * (11 - i);
            }
            remainder = (sum * 10) % 11;
            if (remainder == 10)
                remainder = 0;

            // Verifica o segundo dígito
            if (tempCpf[10] - '0' != remainder)
                return false;


            // CPF é válido
            cpf = tempCpf;
            return true;
        }
    }
}
