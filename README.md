# AWS-Parameters-and-Secrets-Lambda-Extension e .NET 6

Em meados de novembro de 2022 os arquiteto e engenheiro da AWS publicaram um artigo no blog oficial da Amazon sobre como contornar o problema proposto no caso de uso. Em resumo o artigo em questão sugere a implementação de cache em memória utilizando um lambda layer (AWS Parameter Store and Secrets Manager Lambda Extension) que expõe um servidor HTTP que abstrai a implementação de cache as chamadas para os recursos AWS que o SDK normalmente faria. Essa proposta sugere a diminuição de custo financeiro, redução nas chamadas para os recursos AWS e propõe uma possível redução de latência uma vez que o acesso dos para recursos estão cacheados na memória desse layer.

Aqui vamos propor a implementação dessa solução em .NET 6, utilizando Terraform para automação da infraestrutura da função lambda.

Veja meu artigo na integra: [aqui](https://victorldomingues.github.io/blog/aws-parameters-and-secrets-lambda-extension-e-.net-6/)
