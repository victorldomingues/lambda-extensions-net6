locals {
  lambda_name = "lambda_extensions_net6"
  runtime     = "dotnet6"
  filename    = "${path.module}/../src/LambdaExtensions/bin/Release/net6.0/LambdaExtensions.zip" # dotnet lambda package
  file_exists = fileexists(local.filename)
}
