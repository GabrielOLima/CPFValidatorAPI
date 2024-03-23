API de Serviço de validação de CPF com integração ao Chatbot Suri. Esse API é a mesma sendo hosteada atualmente em https://cpfvalidatorapi.azurewebsites.net, podendo ser testada em Postman com a URL https://cpfvalidatorapi.azurewebsites.net/api/CPF/validate, recendo um Json Body com essa estrutura, como localmente, no mesmo intuito, porém a Suri Chatbot utiliza a API hosteada no Azure.

{
  "cpf"="string"
}

O valor deve ser uma string, porém pode ser inserido completamente númerico, ou com os pontos e traços na estrutura de CPF Normal.


CPFs Autorizados para receber boleto de segunda via:

007.548.251-75 | 321.183.394-37 | 746.048.907-73 | 655.610.029-37 | 560.661.577-01
