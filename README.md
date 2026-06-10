# 🌱 Jornada da Terra — DevOps & Cloud

> **Global Solution FIAP 2026/1 — DevOps Tools & Cloud Computing**
> 2 containers Docker integrados: API .NET 8 + Oracle XE 21c, rodando em VM na nuvem Azure.

---

## 🏗️ Arquitetura Macro

<img width="1919" height="607" alt="image" src="https://github.com/user-attachments/assets/4c7e9049-2551-43a9-9a39-7778209fa729" />

```

---

## 🚀 How To — Do zero até rodar na nuvem

### Pré-requisitos
- [Azure CLI](https://learn.microsoft.com/pt-br/cli/azure/install-azure-cli) instalado
- Conta Azure ativa e logada (`az login`)
- Terminal bash (Git Bash no Windows, Terminal no Linux/Mac)

---

### Passo 1 — Criar a VM no Azure

No terminal local, rode o comando abaixo para criar o grupo de recursos e a VM:

```bash
az group create --name rg-jornadadaterra --location canadacentral && az vm create --resource-group rg-jornadadaterra --name vm-jornadadaterra --location canadacentral --image Canonical:0001-com-ubuntu-server-jammy:22_04-lts:latest --size Standard_D2s_v4 --vnet-name vnet-linux --nsg nsgsr-linux --authentication-type password --admin-username admrodrigo --admin-password Manurodrigo2025%
```

Aguarde terminar e anote o IP público exibido no final.

---

### Passo 2 — Conectar na VM via SSH

```bash
ssh admrodrigo@<IP_DA_VM>
```

---

### Passo 3 — Criar o script de configuração da VM

Dentro da VM, crie o arquivo:

```bash
nano dependencias.sh
```

Cole o conteúdo abaixo, salve e feche:

```
CTRL + O  →  ENTER  →  CTRL + X
```

```bash
#!/bin/bash
# ============================================================
# Script de configuração da VM - Global Solution FIAP 2026
# ============================================================
RESOURCE_GROUP="rg-jornadadaterra"
VM_NAME="vm-jornadadaterra"

echo "=========================================="
echo " Abrindo portas na VM..."
echo "=========================================="
az vm open-port \
  --resource-group $RESOURCE_GROUP \
  --name $VM_NAME \
  --port 8080 \
  --priority 110

az vm open-port \
  --resource-group $RESOURCE_GROUP \
  --name $VM_NAME \
  --port 1521 \
  --priority 120

echo "=========================================="
echo " Instalando Docker e subindo containers..."
echo "=========================================="
az vm run-command invoke \
  --resource-group $RESOURCE_GROUP \
  --name $VM_NAME \
  --command-id RunShellScript \
  --scripts "
    apt-get update -y
    apt-get install -y git curl

    curl -fsSL https://get.docker.com -o get-docker.sh
    sh get-docker.sh

    curl -L 'https://github.com/docker/compose/releases/latest/download/docker-compose-linux-x86_64' -o /usr/local/bin/docker-compose
    chmod +x /usr/local/bin/docker-compose

    mkdir -p /home/admrodrigo/app
    git clone https://github.com/rodrigotiezzi/devops---Jornada-da-Terra.git /home/admrodrigo/app
    chown -R admrodrigo:admrodrigo /home/admrodrigo/app

    cd /home/admrodrigo/app
    sudo docker-compose up -d --build

    echo 'Aguardando Oracle XE inicializar (5 min)...'
    sleep 300

    echo '=========================================='
    echo ' Exibindo logs dos containers...'
    echo '=========================================='
    echo '========== LOGS: rm562975-api =========='
    sudo docker logs rm562975-api

    echo '========== LOGS: rm562975-db =========='
    sudo docker logs rm562975-db

    echo '=========================================='
    echo ' Acessando containers...'
    echo '=========================================='
    echo '========== EXEC: rm562975-api =========='
    sudo docker container exec rm562975-api whoami
    sudo docker container exec rm562975-api pwd
    sudo docker container exec rm562975-api ls -l

    echo '========== EXEC: rm562975-db =========='
    sudo docker container exec rm562975-db whoami
    sudo docker container exec rm562975-db pwd
    sudo docker container exec rm562975-db ls -l

    echo 'Evidencias coletadas com sucesso!'
  "

echo "=========================================="
echo " Obtendo IP público da VM..."
echo "=========================================="
PUBLIC_IP=$(az vm show \
  --resource-group $RESOURCE_GROUP \
  --name $VM_NAME \
  --show-details \
  --query publicIps \
  --output tsv)

echo ""
echo "=========================================="
echo " Configuração concluída!"
echo " IP Público: $PUBLIC_IP"
echo ""
echo " Acesse a API em:"
echo " http://$PUBLIC_IP:8080"
echo ""
echo " Acesse o Swagger em:"
echo " http://$PUBLIC_IP:8080"
echo "=========================================="
```

Dê permissão de execução e rode:

```bash
chmod 755 dependencias.sh
./dependencias.sh
```

> ⏳ O script leva cerca de **8 minutos** para concluir. O Oracle XE precisa de ~5 minutos para inicializar na primeira execução.

---

### Passo 4 — Verificar os containers rodando

```bash
sudo docker ps
```

Você deve ver os dois containers com status `Up`:

```
NAMES            STATUS          PORTS
rm562975-api     Up X minutes    0.0.0.0:8080->8080/tcp
rm562975-db      Up X minutes    0.0.0.0:1521->1521/tcp
```

---

### Passo 5 — Acessar o Swagger

Abra no navegador:

```
http://<IP_DA_VM>:8080
```

---

### Passo 6 — Testar o CRUD pelo Swagger

Acesse `http://<IP_DA_VM>:8080` e siga o fluxo na ordem abaixo:

#### 1. Criar um Produtor — `POST /api/Produtores`
```json
{
  "nome": "João da Silva",
  "email": "joao@fazenda.com.br"
}
```
> Anote o `id` retornado na resposta (ex: `1`)

#### 2. Criar uma Fazenda — `POST /api/Fazendas`
```json
{
  "nome": "Fazenda Boa Esperança",
  "municipio": "Ribeirão Preto",
  "estado": "SP",
  "areaHectares": 320,
  "latitude": -21.17,
  "longitude": -47.81,
  "produtorId": 1
}
```

#### 3. Criar um Setor — `POST /api/Setores`
```json
{
  "nome": "Setor Sul",
  "cultura": "Soja",
  "areaHectares": 120,
  "fazendaId": 1
}
```

#### 4. Registrar Leitura de Satélite (gera missão automática) — `POST /api/LeiturasSatelite`
```json
{
  "setorId": 1,
  "temperaturaC": -1.5,
  "umidadeRelativa": 88,
  "ndvi": 0.62,
  "precipitacaoMm": 0
}
```

#### 5. Listar Missões geradas — `GET /api/Missoes?setorId=1`

#### 6. Concluir uma Missão (credita pontos) — `PATCH /api/Missoes/{id}/status`
```json
{
  "status": "Concluida"
}
```

#### 7. Verificar pontos do Produtor — `GET /api/Produtores/1`

---

### Passo 7 — Evidências de persistência no banco (SELECT)

Conectar diretamente no container do Oracle:

```bash
sudo docker container exec -it rm562975-db sqlplus rm562975/080606@XEPDB1
```

Dentro do SQL*Plus, execute os SELECTs:

```sql
SELECT * FROM PRODUTORES;

SELECT * FROM FAZENDAS;

SELECT * FROM SETORES;

SELECT * FROM LEITURAS_SATELITE;

SELECT * FROM MISSOES;

EXIT
```

---

### Passo 8 — Derrubar os containers

```bash
sudo docker-compose down
```

---

## 📁 Arquivos do projeto

```
devops---Jornada-da-Terra/
├── Dockerfile
├── docker-compose.yml
├── dependencias.sh
├── README.md
└── src/JornadaDaTerra.Api/
    ├── Domain/
    ├── Infrastructure/
    ├── Application/
    ├── Controllers/
    ├── Migrations/
    └── Program.cs
```

---

## 🎬 Vídeo Demonstrativo

- _adicionar link do YouTube_

---

## 👥 Integrantes

| Nome | RM | Turma |
|---|---|---|
| Pedro Pereira Biasolli | RM562521 | 2TDSPO |
| Rodrigo Tiezzi | RM562975 | 2TDSPO |
| Bruno Zanateli | RM563736 | 2TDSPO |
| Christian Freitas | RM566098 | 2TDSPO |
| Matheus Enrico | RM562532 | 2TDSPO |
