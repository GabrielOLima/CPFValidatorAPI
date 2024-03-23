API de Serviço de validação de CPF com integração ao Chatbot Suri. Esse API é a mesma sendo hosteada atualmente em https://cpfvalidatorapi.azurewebsites.net, podendo ser testada em Postman com a URL https://cpfvalidatorapi.azurewebsites.net/api/CPF/validate, recendo um Json Body com essa estrutura

{
  "cpf"="string"
}

O valor deve ser uma string, porém pode ser inserido completamente númerico, ou com os pontos e traços na estrutura de CPF Normal.
