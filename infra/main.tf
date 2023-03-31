
resource "aws_iam_role" "lambda_role" {
  name               = "lambda_role"
  assume_role_policy = data.aws_iam_policy_document.AWSLambdaTrustPolicy.json
}

resource "aws_iam_role_policy_attachment" "terraform_lambda_policy" {
  role       = aws_iam_role.lambda_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

resource "aws_lambda_function" "lambda_extensions_net6" {
  function_name    = local.lambda_name
  handler          = "LambdaExtensions::LambdaExtensions.Function::FunctionHandler"
  runtime          = local.runtime
  role             = aws_iam_role.lambda_role.arn
  filename         = local.filename
  source_code_hash = filebase64sha256(local.filename)
  timeout          = 30
  memory_size      = 128
  environment {
    variables = {
      PARAMETERS_SECRETS_EXTENSION_CACHE_ENABLED = "TRUE"
      PARAMETERS_SECRETS_EXTENSION_HTTP_PORT     = "2773"
      # PARAMETERS_SECRETS_EXTENSION_LOG_LEVEL     = "DEBUG"
      # veja todas as variaveis de ambiente
      # https://docs.aws.amazon.com/systems-manager/latest/userguide/ps-integration-lambda-extensions.html

    }
  }
  layers = ["arn:aws:lambda:sa-east-1:933737806257:layer:AWS-Parameters-and-Secrets-Lambda-Extension:4"]
  # Escolha a ARN de sua região
  # https://docs.aws.amazon.com/systems-manager/latest/userguide/ps-integration-lambda-extensions.html
}

output "lambda_file_exists" {
  value       = local.file_exists
  description = "Filename exists"
}
