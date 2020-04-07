# Storage Account for Terraform state

Create a storage account for the terraform state files.  
Use Azure CLI authentication for deploying resources. 

```bash
$ az login
$ az account set --subscription="<YOUR_SUBSCRIPTION_ID>"
$ terraform init
$ terraform validate
$ terraform apply -auto-approve
```

