# Infrastructure デプロイメントスクリプト

Covid19Radar プロジェクトに必要なリソースをプロビジョニングするスクリプトです。

## インストールされるリソース

* Storage Account (terraform state の管理用)
* CosmosDB
* Notification Hub
* Application Insights
* Azure Functions 

CosmosDB, NotificationHub の PrimaryKey及び、Application Insgihts の INSTRUMENTATION_KEYは自動で、Azure Functions の AppSettingsに保存されます。

## 前提条件

* [Azure の利用可能な Subscription](https://azure.microsoft.com/ja-jp/free/)
* [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest)がインストールされている（もしくはCloudShellを使う）
* [Terraform](https://www.terraform.io/downloads.html)がインストールされている（もしくはCloudShellを使う）

## リソースのdeploy

### terraform state 格納用の Storage Account をプロビジョンする
リソースをdeployする前に、terraform の stateをメンバーとシェアするための Storage Account とコンテナをする必要があります。スクリプトが`01_init`の下に用意していますので、実行してください。これは、各人が実行する必要はなく、環境に付き１回実行するだけで結構です。

```bash
$ cd infrastructure/01_init
$ az login
$ az account set --subscription="<YOUR_SUBSCRIPTION_ID>"
$ terraform init -backend=true -backend-config=storage_account_name=<01_Init_STORAGE_ACCOUNT_NAME>
$ terraform validate
$ terraform apply -auto-approve
```
解説すると、この terraform スクリプトは、Azure CLIのログイン情報を使って認証します。最初に Azure CLIでログインした後、terraform のコマンドを実行します。CloudShellを使っている場合は必要ありません。
　`terraform init` は terraform の環境を初期化します。プロバイダをダウンロードしたります。
`terraform validate`により terraform のファイルに対して静的解析を実施します。もし、warningが出た場合は指示にしたがって修正してください。最後に、`terraform apply`により実際に環境が構築されます。スクリプトに修正を加えた時は、`terraform plan`で変更点を確認して、`terraform apply`を実行してください。インフラストラクチャが必要なもののみ変更されます。このインフラの状態は、ステートファイルと呼ばれるファイル`.terraform`以下に配置されて、保存されます。

### Runtime 環境の構築

すでに、`az login`を実行している前提で説明いたします。`02_runtime` には、環境を構築するスクリプトが含まれています。CosmosDB, NotificationHub, ApplicationInsights, Function Appが自動で作成せれ、コンフィグレーションされます。
`01_init`との違いは、`terraform init`の箇所です。`backend-config`が指定されています。`01_init`の時は、terraform のステートファイルが、ローカルに保存されましたが、今回は`01_init`で作成された Storage Account の blob に保存されるようになっています。それにより、他の人とステートのシェアが可能になります。後は、下記のコマンドを実行して、環境構築を行ってください。

```bash
$ cd ..
$ cd 02_runtime
$ terraform init -backend=true -backend-config=storage_account_name=<01_Init_STORAGE_ACCOUNT_NAME>
$ terraform validate
$ terraform apply -auto-approve
```

## 設定変更したいときは？
`variables.tf`に初期設定の変数が格納されています。いくつかの変数は、プレフィックスの扱いで、ランダムの変数が追加されます。Azureのリソースは、名前がGlobalユニークである必要のあるリソースがいくつかあるので、名前がダブらないように、そのような挙動になっています。

# おわりに
よい terraform ライフを。