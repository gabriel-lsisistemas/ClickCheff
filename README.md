# ClickCheff

Aplicação desktop desenvolvida em .NET para integração com o sistema CLICK, utilizada para controle de pedidos, comunicação com cozinha (Cheff) e processamento de informações locais, permitindo envio, recebimento e manipulação de dados em tempo real.

O ClickCheff funciona como módulo auxiliar do sistema CLICK, sendo utilizado para automação de processos, integração com banco de dados e comunicação com outros serviços.

---

## 📌 Objetivo

O sistema tem como finalidade:

* Integrar com sistema CLICK
* Processar pedidos
* Enviar dados para cozinha
* Ler dados do banco
* Executar rotinas locais
* Automatizar operações

Pode ser usado para:

* Restaurante
* PDV
* Integração com cozinha
* Integração com API
* Automação local

---

## 📌 Funcionamento geral

Fluxo do sistema:

```id="k4m2s7"
Inicia ClickCheff
↓
Conecta banco
↓
Lê pedidos
↓
Processa dados
↓
Envia para cozinha / serviço
↓
Atualiza status
↓
Registra log
```

O sistema pode rodar continuamente.

---

## 📌 Estrutura do projeto

Arquivos comuns:

```id="p7c1v9"
Program.cs
Form1.cs
Form1.Designer.cs
App.config
ClickCheff.cs
Models/
Dao/
Utils/
ClickCheff.csproj
Properties/
```

Descrição:

| Arquivo / Pasta | Função           |
| --------------- | ---------------- |
| Program.cs      | Inicialização    |
| Form1.cs        | Interface        |
| ClickCheff.cs   | Lógica principal |
| Dao             | Banco            |
| Models          | Entidades        |
| Utils           | Auxiliares       |
| App.config      | Configuração     |
| csproj          | Projeto          |

---

## 📌 Program.cs

Responsável por iniciar a aplicação.

```csharp id="m3q8n1"
Application.Run(new Form1());
```

Função:

* iniciar sistema
* abrir tela
* iniciar ciclo

---

## 📌 Form1.cs

Tela principal.

Responsável por:

* iniciar processamento
* mostrar status
* registrar logs
* controlar execução

Pode conter:

* timer
* botões
* lista de pedidos
* status

---

## 📌 ClickCheff.cs

Classe principal do processamento.

Responsável por:

* ler dados do banco
* processar pedidos
* enviar dados
* atualizar status
* controlar execução

Fluxo:

```id="z8r4m2"
Banco
 ↓
ClickCheff
 ↓
Processamento
 ↓
Atualização
```

---

## 📌 App.config

Arquivo de configuração.

Pode conter:

```id="s5v9k1"
ConnectionString
Servidor
Database
Intervalo
Porta
Config
```

Usado para definir banco e parâmetros.

---

## 📌 Models

Representam dados do sistema.

Exemplos:

```id="x2m7c6"
Pedido
ItemPedido
Produto
Mesa
Comanda
Status
```

Usados para transportar dados.

---

## 📌 Dao / Repository

Responsável por acesso ao banco.

Funções:

* consultar pedidos
* inserir registros
* atualizar status
* executar SQL

Pode usar:

* Firebird
* SQL Server
* PostgreSQL

---

## 📌 Utils

Funções auxiliares.

Pode conter:

* logs
* validações
* conversões
* helpers

---

## 📌 Processo de execução

Etapas:

1. Conectar banco
2. Buscar pedidos
3. Processar pedidos
4. Enviar para cozinha
5. Atualizar status
6. Registrar log

Fluxo:

```id="r1k6p8"
Banco → ClickCheff → Cozinha / Serviço
```

---

## 📌 Logs

Pode registrar:

* pedidos processados
* erros
* conexões
* execução

Pode mostrar em:

* tela
* arquivo
* banco

---

## 📌 Requisitos

* Windows
* .NET instalado
* Banco configurado
* Permissão de rede

---

## 📌 Como executar

1. Abrir solução

```id="t9n2c5"
ClickCheff.sln
```

2. Compilar

3. Executar

ou rodar o exe.

---

## 📌 Uso recomendado

* Restaurante
* PDV
* Integração com cozinha
* Integração com CLICK
* Automação local
* Processamento de pedidos

---

## 📌 Autor

Gabriel Rebouças
LSI Sistemas

---
